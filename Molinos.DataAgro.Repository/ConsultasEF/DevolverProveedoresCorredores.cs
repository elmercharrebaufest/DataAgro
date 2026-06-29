using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverProveedoresCorredores : IConsulta<BusquedaHome>
    {
        private readonly string filtro;
        private readonly bool esComisionista;
        private readonly bool? validarSisa;

        public DevolverProveedoresCorredores(string filtro, bool esComisionista = false, bool? validarSisa = true)
        {
            this.filtro = filtro;
            this.esComisionista = esComisionista;
            this.validarSisa = validarSisa;
        }

        private static List<BusquedaHome> Query(
            DbContext contexto,
            string filtro,
            bool esComisionista,
            bool? validarSisa = true)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto)
                .ObjectContext.CommandTimeout = 180;

            var espacioFiltro = " " + filtro;

            var resultado =
                (from proveedor in contexto.Set<Proveedor>()
                 join contactoComercial in contexto.Set<ContactoComercial>()
                     on proveedor.ProveedorId equals contactoComercial.ProveedorId into contactos
                 from contactoComercial in contactos.DefaultIfEmpty()

                 where
                     (
                         proveedor.CUIT.Contains(filtro) ||
                         proveedor.RazonSocial.Contains(filtro) ||
                         proveedor.Alias.StartsWith(filtro) ||
                         proveedor.Alias.Contains(espacioFiltro) ||
                         contactoComercial.Nombres.StartsWith(filtro) ||
                         contactoComercial.Nombres.Contains(espacioFiltro) ||
                         contactoComercial.Apellido.StartsWith(filtro) ||
                         contactoComercial.Apellido.Contains(espacioFiltro)
                     )
                     &&
                     (!esComisionista || proveedor.Comisionista == esComisionista)

                 select new BusquedaHome
                 {
                     Id = proveedor.ProveedorId,
                     Cuit = proveedor.CUIT,
                     Alias = proveedor.Alias,

                     RazonSocial =
                         proveedor.SegmentacionId == 5 ||
                         proveedor.SegmentacionId == 7
                             ? "COR - " +
                               (!string.IsNullOrEmpty(proveedor.Alias)
                                   ? proveedor.Alias + " - " + proveedor.RazonSocial
                                   : proveedor.RazonSocial)
                             : !string.IsNullOrEmpty(proveedor.Alias)
                                 ? proveedor.Alias + " - " + proveedor.RazonSocial
                                 : proveedor.RazonSocial,

                     Corredor =
                         proveedor.SegmentacionId == 5 ||
                         proveedor.SegmentacionId == 7
                             ? "COR"
                             : "",

                     Filtro =
                         filtro + "|" +
                         (!string.IsNullOrEmpty(proveedor.Alias)
                             ? proveedor.Alias + " - " + proveedor.RazonSocial
                             : proveedor.RazonSocial)
                         + " (" + proveedor.CUIT + ")",

                     ClasificacionId = proveedor.ClasificacionCompraNetId,
                     RiesgoComercialSap = proveedor.RiesgoComercialSap,
                     Deshabilitado = proveedor.Deshabilitado,
                     Consignatario = proveedor.Consignatario,
                     PlanCanje = proveedor.PlanCanje,

                     Deshabilitar = false,
                     Color = "",

                     CuposConRiesgo = proveedor.CuposConRiesgo,
                     Segmentacion = proveedor.Segmentacion.Descripcion,
                     Grupo = proveedor.Segmentacion.Grupo,
                     SegmentacionId = proveedor.SegmentacionId,

                     prioridad =
                         proveedor.RazonSocial.Contains(filtro) ? 0 :
                         contactoComercial.Nombres.Contains(filtro) ? 1 :
                         contactoComercial.Apellido.Contains(filtro) ? 2 : 3
                 })
                 .Distinct()
                 .Take(15)
                 .ToList();

            if (validarSisa == true)
            {
                return DevolverEstadoSisa(contexto, resultado);
            }

            return resultado;
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro, esComisionista, validarSisa);
            }
        }

        private static List<BusquedaHome> DevolverEstadoSisa(DbContext contexto, List<BusquedaHome> lista)
        {
            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    if (item.Deshabilitado.HasValue && item.Deshabilitado.Value != false)
                    {
                        item.Estado = "Deshabilitado";
                        item.Deshabilitar = true;
                        item.Color = "red";
                        continue;
                    }

                    if (!string.IsNullOrEmpty(item.RiesgoComercialSap))
                    {
                        if (item.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                        {
                            item.Estado = "No Operable por Riesgo Comercial Alto";
                            item.Deshabilitar = true;
                            item.Color = "red";
                            continue;
                        }
                    }
                    var sisa = new SISA();

                    sisa = (from s in contexto.Set<SISA>()
                            where s.CUIT == item.Cuit
                            select s).OrderBy(x => x.SituacionCategoria).FirstOrDefault();

                    if (sisa != null)
                    {
                        if (sisa.EstadoCuit == 3 && item.RiesgoComercialSap != "E")
                        {
                            item.Estado = "No Operable por Estado de CUIT 3";
                            item.Color = "red";
                            item.Deshabilitar = true;
                            continue;
                        }
                        else if (sisa.EstadoCuit == 0)
                        {
                            item.Estado = "No Operable por Estado de CUIT Inactivo";
                            item.Color = "red";
                            item.Deshabilitar = true;
                            continue;
                        }
                        if (sisa.SituacionCategoria != "AL")
                        {
                            item.Estado = "No Operable por Situación Categoría";
                            item.Color = "red";
                            item.Deshabilitar = true;
                            continue;
                        }
                    }
                    else
                    {
                        item.Estado = "No Operable por CUIT o Categoria Inactivo";
                        item.Color = "red";
                        item.Deshabilitar = true;
                        continue;
                    }
                    //}


                    var estadoProveedor = (from provEstado in contexto.Set<ProveedorEstado>()
                                           where provEstado.ProveedorId == item.Id && provEstado.EstadoId == 4
                                           select provEstado).FirstOrDefault();

                    if (estadoProveedor != null)
                    {
                        item.Estado = "No Operable por Estado BAJA";
                        item.Deshabilitar = true;
                        item.Color = "red";
                        continue;
                    }

                    // TODO: Se comenta por falta de definición en ticket DAT-265
                    //var estadoHomeProveedor = (from provEstadoHome in contexto.Set<Proveedor>()
                    //                           where provEstadoHome.ProveedorId == item.Id &&
                    //                                 provEstadoHome.EstadoHomeId == (int)EnumEstadoHome.NO_HABILITADO
                    //                           select provEstadoHome).FirstOrDefault();

                    //if (estadoHomeProveedor != null)
                    //{
                    //    item.Estado = "No Operable por Estado Home NO HABILITADO";
                    //    item.Deshabilitar = true;
                    //    item.Color = "red";
                    //    continue;
                    //}

                    var facacop = (from f in contexto.Set<FACACOP>()
                                   where f.CUIT == item.Cuit
                                   select f).FirstOrDefault();
                    if (facacop != null)
                    {
                        item.Estado = "No Operable por ser Apócrifo";
                        item.Deshabilitar = true;
                        item.Color = "red";
                        continue;
                    }

                }
                return lista;
            }
            return lista;
        }
    }
}
