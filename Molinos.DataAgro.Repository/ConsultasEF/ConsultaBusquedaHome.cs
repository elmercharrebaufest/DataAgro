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
    public class ConsultaBusquedaHome : IConsulta<BusquedaHome>
    {
        private readonly List<int> equipo;
        private readonly int comercialId;
        private readonly string filtro;
        private readonly List<int> corredoresComercial;
        private readonly bool corredor;

        public ConsultaBusquedaHome(List<int> equipo, int comercialId, string filtro, List<int> corredoresComercial, bool corredor)
        {
            this.equipo = equipo;
            this.comercialId = comercialId;
            this.filtro = filtro;
            this.corredoresComercial = corredoresComercial;
            this.corredor = corredor;
        }

        private static List<BusquedaHome> Query(DbContext contexto, List<int> equipo, int comercialId, string filtro, List<int> corredoresComercial, bool corredor)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                (from Proveedor in contexto.Set<Proveedor>()
                     //join proveedorComercial in contexto.Set<ProveedorComercial>() on Proveedor.ProveedorId equals proveedorComercial.ProveedorId
                 join contactoComercial in contexto.Set<ContactoComercial>() on Proveedor.ProveedorId equals contactoComercial.Proveedor.ProveedorId into cons
                 from contactoComercial in cons.DefaultIfEmpty()
                     //where (equipo.Contains(proveedorComercial.ComercialId) || corredor) &&
                 where
                     (Proveedor.CUIT.Contains(filtro) ||
                     contactoComercial.Nombres.Contains(filtro) ||
                     contactoComercial.Apellido.Contains(filtro) ||
                     contactoComercial.Proveedor.RazonSocial.Contains(filtro) ||
                     Proveedor.RazonSocial.Contains(filtro) ||
                     Proveedor.Alias.Contains(filtro))

                 //group proveedorComercial by Proveedor into provs
                 select new BusquedaHome

                 {
                     Id = Proveedor.ProveedorId,
                     Cuit = Proveedor.CUIT,
                     Alias = Proveedor.Alias,
                     RazonSocial = Proveedor.SegmentacionId == 5 || Proveedor.SegmentacionId == 7 ? "COR - " + (!string.IsNullOrEmpty(Proveedor.Alias) ? (Proveedor.Alias + " - " + Proveedor.RazonSocial) : Proveedor.RazonSocial) : !string.IsNullOrEmpty(Proveedor.Alias) ? (Proveedor.Alias + " - " + Proveedor.RazonSocial) : Proveedor.RazonSocial,
                     Corredor = Proveedor.SegmentacionId == 5 || Proveedor.SegmentacionId == 7 ? "COR" : "",
                     Filtro = filtro + "|" + (!string.IsNullOrEmpty(Proveedor.Alias) ? (Proveedor.Alias + " - " + Proveedor.RazonSocial) : Proveedor.RazonSocial) + " (" + Proveedor.CUIT + ")",
                     ClasificacionId = Proveedor.ClasificacionCompraNetId,
                     RiesgoComercialSap = Proveedor.RiesgoComercialSap,
                     Deshabilitado = Proveedor.Deshabilitado,
                     Consignatario = Proveedor.Consignatario,
                     PlanCanje = Proveedor.PlanCanje,
                     OperaConMATBA = Proveedor.OperaConMATBA,
                 }).Distinct().Take(15).ToList();

            var lista = DevolverEstadoSisa(contexto, resultado.ToList(), equipo, corredor);
            return lista.ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, equipo, comercialId, filtro, corredoresComercial, corredor);
            }
        }
        private static List<BusquedaHome> DevolverEstadoSisa(DbContext contexto, List<BusquedaHome> lista, List<int> equipo, bool corredor)
        {
            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    VerificarSiEstaAsignado(contexto, item, equipo, corredor);
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
                                        where s.CUIT == item.Cuit && s.CodCategoria == 1 && s.SituacionCategoria == "AL"
                                        select s).FirstOrDefault();
                            }
                            else if (item.ClasificacionId == 2)
                            {
                                sisa = (from s in contexto.Set<SISA>()
                                        where s.CUIT == item.Cuit && s.CodCategoria == 6 && s.SituacionCategoria == "AL"
                                        select s).FirstOrDefault();
                            }
                            else
                            {
                                sisa = (from s in contexto.Set<SISA>()
                                        where s.CUIT == item.Cuit && s.CodCategoria != 1 && s.CodCategoria != 6 && s.CodCategoria != 19 && s.SituacionCategoria == "AL"
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
                                if (sisa.CodCategoria == 19)
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
                                where s.CUIT == item.Cuit && s.CodCategoria == 2 && s.SituacionCategoria == "AL"
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
                    if (item.OperaConMATBA == true)
                    {
                        item.Estado += ". Opera solo MATBA";
                    }
                }
                return lista;
            }
            return lista;
        }
        private static void VerificarSiEstaAsignado(DbContext contexto, BusquedaHome proveedor, List<int> equipo, bool corredor)
        {
            if (proveedor.Corredor == "COR" && corredor)
            {
                proveedor.EstaAsignado = true;
                return;
            }
            var proveedorComercialAsignado = (from s in contexto.Set<ProveedorComercial>()
                                              where s.ProveedorId == proveedor.Id
                                              select s).Select(a => a.ComercialId).ToList();

            proveedor.EstaAsignado = equipo.Intersect(proveedorComercialAsignado).Any();
        }
    }
}
