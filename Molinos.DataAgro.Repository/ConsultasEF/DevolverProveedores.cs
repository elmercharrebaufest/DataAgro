using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
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
                            : corredor.Equals(1) ? (Proveedor.SegmentacionId == 5 || Proveedor.SegmentacionId == 7): Proveedor.SegmentacionId>0))
                            group c by Proveedor into provs
                            select new BusquedaHome
                            {
                                Id = provs.Key.ProveedorId,
                                Cuit = provs.Key.CUIT,
                                RazonSocial = !string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial): provs.Key.RazonSocial,
                                Alias = provs.Key.Alias,
                                Filtro = filtro + "|" + (!string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial) + " (" + provs.Key.CUIT + ")"
                            };

            return resultado.Distinct().Take(15).ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtro, corredor, equipo);
            }
        }
    }
}
