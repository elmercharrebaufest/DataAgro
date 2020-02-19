using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ActualizarComercialHome: IConsulta<Datos>
    {
        private readonly List<int> equipo;
        private readonly int comercialId;
        private readonly string filtro;
        private readonly List<int> corredoresComercial;

        public ActualizarComercialHome(List<int> equipo, int comercialId)
        {
            this.equipo = equipo;
            this.comercialId = comercialId;
        }

        private static List<Datos> Query(DbContext contexto, List<int> equipo, int comercialId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from proveedorComercial in contexto.Set<ProveedorComercial>()
                
                where equipo.Contains(proveedorComercial.Comercial.ComercialId)
                group new { proveedorComercial } by new
                {
                    proveedorComercial.Proveedor.CUIT,
                    proveedorComercial.Comercial.IdActiveDirectory
                };
            var query = resultado.Select(x => new Datos
            {
                CUIT = x.Key.CUIT,
                UsuarioDirectory = x.Key.IdActiveDirectory
            });
            return query.ToList();
        }

        public virtual List<Datos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, equipo, comercialId);
            }
        }
    }
}
