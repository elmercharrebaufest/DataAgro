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
        private readonly int perfilId;

        public ConsultaBusquedaHome(List<int> equipo, int comercialId, string filtro, List<int> corredoresComercial, int perfilId)
        {
            this.equipo = equipo;
            this.comercialId = comercialId;
            this.filtro = filtro;
            this.corredoresComercial = corredoresComercial;
            this.perfilId = perfilId;
        }

        private static List<BusquedaHome> Query(DbContext contexto, List<int> equipo, int comercialId, string filtro, List<int> corredoresComercial, int perfilId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from proveedorComercial in contexto.Set<ProveedorComercial>()
                join contactoComercial in contexto.Set<ContactoComercial>() on proveedorComercial.Proveedor.ProveedorId equals contactoComercial.Proveedor.ProveedorId into cons
                from contactoComercial in cons.DefaultIfEmpty()
                where (equipo.Contains(proveedorComercial.Comercial.ComercialId)
                    || (perfilId == (int)EnumPerfil.CorredoresComercial && corredoresComercial.Contains(proveedorComercial.Comercial.ComercialId)))  &&
                    (proveedorComercial.Proveedor.CUIT.StartsWith(filtro) ||
                    (contactoComercial.Apellido + " " + contactoComercial.Nombres).StartsWith(filtro) ||
                    (contactoComercial.Nombres + " " + contactoComercial.Apellido).StartsWith(filtro) ||
                    contactoComercial.Proveedor.RazonSocial.Contains(filtro))

                group new { contactoComercial } by new
                {
                    proveedorComercial.ProveedorId,
                    proveedorComercial.Proveedor.RazonSocial,
                    proveedorComercial.Proveedor.CUIT
                };
            var query = resultado.Select(x => new BusquedaHome
            {
                Id = x.Key.ProveedorId,
                RazonSocial = x.Key.RazonSocial,
                Cuit = x.Key.CUIT,
                Filtro = filtro
            });
            return query.ToList();
        }

        public virtual List<BusquedaHome> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, equipo, comercialId, filtro, corredoresComercial, perfilId);
            }
        }
    }
}
