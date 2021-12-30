using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class DevolverProveedores : IConsulta<BusquedaHome>
    {
        private readonly string filtro;
        private readonly int corredor;
        private readonly List<int> equipo;

        public DevolverProveedores(string filtro, int corredor, List<int> equipo)
        {
            this.filtro = filtro;
            this.corredor = corredor;
            this.equipo = equipo;
        }

        private static List<BusquedaHome> Query(DbContext contexto, string filtro, int corredor, List<int> equipo)
        {
            var resultado = from Proveedor in contexto.Set<Proveedor>()
                            join p in contexto.Set<ProveedorComercial>() on Proveedor.ProveedorId equals p.ProveedorId into rgs
                            from p in rgs.DefaultIfEmpty()
                            join c in contexto.Set<ContactoComercial>() on Proveedor.ProveedorId equals c.ProveedorId into rg
                            from c in rg.DefaultIfEmpty()
                            where ((Proveedor.CUIT.Contains(filtro) || Proveedor.RazonSocial.Contains(filtro) || Proveedor.Alias.Contains(filtro) ||
                            c.Nombres.Contains(filtro) || c.Apellido.Contains(filtro)) &&
                            (corredor.Equals(0) ? Proveedor.SegmentacionId != 5 && Proveedor.SegmentacionId != 7
                            : corredor.Equals(1) ? (Proveedor.SegmentacionId == 5 || Proveedor.SegmentacionId == 7) : Proveedor.SegmentacionId > 0))
                            group c by Proveedor into provs
                            select new BusquedaHome
                            {
                                Id = provs.Key.ProveedorId,
                                Cuit = provs.Key.CUIT,
                                RazonSocial = !string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial,
                                Alias = provs.Key.Alias,
                                ClasificacionId = provs.Key.ClasificacionCompraNetId,
                                RiesgoComercialSap = provs.Key.RiesgoComercialSap,
                                Deshabilitado = provs.Key.Deshabilitado,
                                Consignatario = provs.Key.Consignatario,
                                PlanCanje = provs.Key.PlanCanje,
                                Deshabilitar = false,
                                Color = "",
                                Filtro = filtro + "|" + (!string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial) + " (" + provs.Key.CUIT + ")",
                                ComisionistaId = provs.Key.ComisionistaId
                            };
            var lista = DevolverEstadoSisa(contexto, resultado.ToList(), corredor);
            return lista.Distinct().Take(15).ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro, corredor, equipo);
            }
        }

        private static List<BusquedaHome> DevolverEstadoSisa(DbContext contexto, List<BusquedaHome> lista, int corredor)
        {
            if (lista.Count > 0)
            {
                foreach (var item in lista)
                {
                    if (corredor == 0)
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
                                        where s.CUIT == item.Cuit && s.CodCategoria != 1 && s.CodCategoria != 6 && s.SituacionCategoria == "AL"
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

                }
                return lista;
            }
            return lista;
        }
    }
}
