using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
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
                from Proveedor in contexto.Set<Proveedor>()
                join proveedorComercial in contexto.Set<ProveedorComercial>() on Proveedor.ProveedorId equals proveedorComercial.ProveedorId
                join contactoComercial in contexto.Set<ContactoComercial>() on proveedorComercial.Proveedor.ProveedorId equals contactoComercial.Proveedor.ProveedorId into cons
                from contactoComercial in cons.DefaultIfEmpty()
                where (equipo.Contains(proveedorComercial.ComercialId) || corredor) &&
                    (Proveedor.CUIT.Contains(filtro) ||
                    contactoComercial.Nombres.Contains(filtro) ||
                    contactoComercial.Apellido.Contains(filtro) ||
                    contactoComercial.Proveedor.RazonSocial.Contains(filtro) ||
                    Proveedor.RazonSocial.Contains(filtro) ||
                    Proveedor.Alias.Contains(filtro))

                group new { proveedorComercial } by new
                {
                    proveedorComercial.ProveedorId,
                    proveedorComercial.Proveedor.RazonSocial,
                    proveedorComercial.Proveedor.CUIT,
                    proveedorComercial.Proveedor.Alias
                };
            var query = resultado.Select(provs => new BusquedaHome
            {
                Id = provs.Key.ProveedorId,
                RazonSocial = !string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial,
                Alias = provs.Key.Alias,
                Cuit = provs.Key.CUIT,
                Filtro = filtro + "|" + (!string.IsNullOrEmpty(provs.Key.Alias) ? (provs.Key.Alias + " - " + provs.Key.RazonSocial) : provs.Key.RazonSocial) + " (" + provs.Key.CUIT + ")"
            });
            return query.ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, equipo, comercialId, filtro, corredoresComercial, corredor);
            }
        }
    }
}
