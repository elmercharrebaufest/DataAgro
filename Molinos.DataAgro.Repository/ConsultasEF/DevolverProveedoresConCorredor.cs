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
    public class DevolverProveedoresConCorredor : IConsulta<BusquedaHome>
    {
        private readonly string filtro;
        private readonly string cuitCorredor;

        public DevolverProveedoresConCorredor(string filtro, string cuitCorredor)
        {
            this.filtro = filtro;
            this.cuitCorredor = cuitCorredor;
        }

        private static List<BusquedaHome> Query(DbContext contexto, string filtro, string cuitCorredor)
        {
            var resultado = (from corredorProveedor in contexto.Set<CorredorProveedor>()
                             join c in contexto.Set<ContactoComercial>() on corredorProveedor.ProveedorId equals c.ProveedorId into rgs
                             from c in rgs.DefaultIfEmpty()
                             where (corredorProveedor.Corredor.CUIT.Contains(cuitCorredor))
                            && (corredorProveedor.Proveedor.CUIT.Contains(filtro) || c.Nombres.Contains(filtro) || c.Apellido.Contains(filtro) || corredorProveedor.Proveedor.Alias.Contains(filtro)
                            || corredorProveedor.Proveedor.RazonSocial.Contains(filtro))
                             group c by corredorProveedor into provs

                             select new BusquedaHome
                             {
                                 Id = provs.Key.ProveedorId,
                                 Cuit = provs.Key.Proveedor.CUIT,
                                 RazonSocial = !string.IsNullOrEmpty(provs.Key.Proveedor.Alias) ? (provs.Key.Proveedor.Alias + " - " + provs.Key.Proveedor.RazonSocial) : provs.Key.Proveedor.RazonSocial,
                                 Alias = provs.Key.Proveedor.Alias,
                                 Filtro = filtro + "|" + (!string.IsNullOrEmpty(provs.Key.Proveedor.Alias) ? (provs.Key.Proveedor.Alias + " - " + provs.Key.Proveedor.RazonSocial) : provs.Key.Proveedor.RazonSocial) + " (" + provs.Key.Proveedor.CUIT + ")",
                                 ClasificacionId = provs.Key.Proveedor.ClasificacionCompraNetId,
                                 RiesgoComercialSap = provs.Key.Proveedor.RiesgoComercialSap,
                                 Deshabilitado = provs.Key.Proveedor.Deshabilitado,
                                 Consignatario = provs.Key.Proveedor.Consignatario,
                                 PlanCanje = provs.Key.Proveedor.PlanCanje,
                                 Deshabilitar = false,
                                 Color = "",
                                 Corredor = provs.Key.Proveedor.SegmentacionId == 5 || provs.Key.Proveedor.SegmentacionId == 7 ? "COR" : "",
                             }).Distinct().Take(15).ToList();

            return DevolverEstadoSisa(contexto, resultado);
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro, cuitCorredor);
            }
        }
        private static List<BusquedaHome> DevolverEstadoSisa(DbContext contexto, List<BusquedaHome> lista)
        {
            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    if (item.Corredor == "")
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
                        if (item.ClasificacionId.HasValue)
                        {
                            if (item.ClasificacionId == 1)
                            {
                                sisa = (from s in contexto.Set<SISA>()
                                        where s.CUIT == item.Cuit && s.CodCategoria == (int)EnumEstadoSisa.PRODUCTOR && s.SituacionCategoria == "AL"
                                        select s).FirstOrDefault();
                            }
                            else if (item.ClasificacionId == 2)
                            {
                                sisa = (from s in contexto.Set<SISA>()
                                        where s.CUIT == item.Cuit && s.CodCategoria == (int)EnumEstadoSisa.ACOPIADOR && s.SituacionCategoria == "AL"
                                        select s).FirstOrDefault();
                            }
                            else
                            {
                                sisa = (from s in contexto.Set<SISA>()
                                        where s.CUIT == item.Cuit && s.CodCategoria != (int)EnumEstadoSisa.PRODUCTOR && s.CodCategoria != (int)EnumEstadoSisa.ACOPIADOR && s.CodCategoria != (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS && s.SituacionCategoria == "AL"
                                        select s).FirstOrDefault();
                            }
                            if (sisa != null)
                            {
                                if (sisa.EstadoCuit == 3 && item.RiesgoComercialSap != "E")
                                {
                                    item.Estado = "No Operable por Estado de CUIT 3";
                                    item.Color = "red";
                                    continue;
                                }
                                else if (sisa.EstadoCuit == 0)
                                {
                                    item.Estado = "No Operable por Estado de CUIT Inactivo";
                                    item.Color = "red";
                                    continue;
                                }
                                if (sisa.SituacionCategoria != "AL")
                                {
                                    item.Estado = "No Operable por Situación Categoría BA";
                                    item.Color = "red";
                                    continue;
                                }
                                if (sisa.CodCategoria == (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS)
                                {
                                    item.Estado = "No operable por categoría Operador de Derivados Granarios";
                                    item.Color = "red";
                                    continue;
                                }
                            }
                            else
                            {
                                item.Estado = "No Operable por CUIT o Categoria Inactivo";
                                item.Color = "red";
                                continue;
                            }
                        }

                    }
                    else
                    {
                        var sisa = new SISA();
                        sisa = (from s in contexto.Set<SISA>()
                                where s.CUIT == item.Cuit && s.CodCategoria == (int)EnumEstadoSisa.CORREDOR && s.SituacionCategoria == "AL"
                                select s).FirstOrDefault();

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

                                item.Estado = "Corredor No Operable por Estado de CUIT Inactivo";
                                item.Color = "red";
                                item.Deshabilitar = true;
                                continue;
                            }
                            if (sisa.SituacionCategoria != "AL")
                            {
                                item.Estado = "Corredor No Operable por Situación Categoría BA";
                                item.Color = "red";
                                item.Deshabilitar = true;
                                continue;
                            }
                        }
                        else
                        {
                            item.Estado = "Corredor No Operable por CUIT o Categoria Inactivo";
                            item.Color = "red";
                            item.Deshabilitar = true;
                            continue;
                        }
                    }
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
