using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerComerciales : IConsultaEscalar<IList<ComercialIni>>
    {        
        private static IList<ComercialIni> Query(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var temp = contexto.Set<Comercial>()
                .Select(x => new
                {
                    x.ComercialId,
                    x.Apellido,
                    x.Nombres,
                    PerDescripcion = x.Perfil.Descripcion,
                    Roles = x.RolesAsociados.AsEnumerable()
                }).ToList().OrderBy(x=>x.Apellido);

               var list = temp.Select(q => new ComercialIni {ComercialId= q.ComercialId,
                   Apellido = q.Apellido,
                   Nombres=q.Nombres,
                   PerDescripcion= q.PerDescripcion,
                   Rol = string.Join(", ", q.Roles.Select(x => x.Descripcion)) }).ToList();

            return list;
        }

        public virtual IList<ComercialIni> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
