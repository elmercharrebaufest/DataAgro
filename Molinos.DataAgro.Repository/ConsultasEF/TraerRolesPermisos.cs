using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerRolesPermisos : IConsultaEscalar<IList<RolDto>>
    {        
        private static IList<RolDto> Query(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var temp = contexto.Set<RolPermiso>().GroupBy(x => new { x.RolId, x.Rol.Descripcion })
                .Select(rol => new
                {
                    Id = rol.Key.RolId,
                    rol.Key.Descripcion,
                    Permisos = rol.AsEnumerable()
                }).ToList();
               return temp.Select(q => new RolDto { Id = q.Id, Descripcion = q.Descripcion, Permisos = string.Join(", ", q.Permisos.Select(x => x.Permiso.DisplayEnum())) }).ToList();
        }

        public virtual IList<RolDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
