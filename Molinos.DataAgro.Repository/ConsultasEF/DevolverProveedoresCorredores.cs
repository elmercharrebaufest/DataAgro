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

        private static List<BusquedaHome> Query(DbContext contexto, string filtro, bool esComisionista, bool? validarSisa = true)
        {
            var resultado = from Proveedor in contexto.Set<Proveedor>()
                            join p in contexto.Set<ProveedorComercial>() on Proveedor.ProveedorId equals p.ProveedorId into rgs
                            from p in rgs.DefaultIfEmpty()
                            join c in contexto.Set<ContactoComercial>() on Proveedor.ProveedorId equals c.ProveedorId into rg
                            from c in rg.DefaultIfEmpty()
                            where (Proveedor.CUIT.Contains(filtro) || Proveedor.RazonSocial.Contains(filtro) || Proveedor.Alias.Contains(filtro) ||
                            c.Nombres.Contains(filtro) || c.Apellido.Contains(filtro)) && (esComisionista == false || Proveedor.Comisionista == esComisionista)
                            group c by Proveedor into provs
                            select new BusquedaHome
                            {
                                Id = provs.Key.ProveedorId,
                                Cuit = provs.Key.CUIT,
                                Alias = provs.Key.Alias,
                                RazonSocial = provs.Key.SegmentacionId == 5 || provs.Key.SegmentacionId == 7 ? "COR - " + (!string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial) : !string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial,
                                Corredor = provs.Key.SegmentacionId == 5 || provs.Key.SegmentacionId == 7 ? "COR" : "",
                                Filtro = filtro + "|" + (!string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial) + " (" + provs.Key.CUIT + ")",
                                ClasificacionId = provs.Key.ClasificacionCompraNetId,
                                RiesgoComercialSap = provs.Key.RiesgoComercialSap,
                                Deshabilitado = provs.Key.Deshabilitado,
                                Consignatario = provs.Key.Consignatario,
                                PlanCanje = provs.Key.PlanCanje,
                                Deshabilitar = false,
                                Color = "",
                                CuposConRiesgo = provs.Key.CuposConRiesgo
                            };
            var lista = resultado.Distinct().Take(15).ToList();
            if (validarSisa == true)
            {
                return DevolverEstadoSisa(contexto, lista);
            }
            return lista;
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
