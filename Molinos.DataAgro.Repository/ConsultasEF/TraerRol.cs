using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerRol : IConsultaEscalar<RolDto>
    {
        private readonly int rolId;
        public TraerRol(int rolId)
        {
            this.rolId = rolId;
        }
        private static RolDto Query(DbContext contexto, int rolId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var temp = contexto.Set<RolPermiso>().Where(x => x.RolId == rolId).GroupBy(x => new { x.RolId, x.Rol.Descripcion })
                .Select(rol => new
            {
                Id = rol.Key.RolId,
                rol.Key.Descripcion,
                Permisos = rol.AsEnumerable()
            }).First();
            var rolPermiso = new RolDto
            {
                Id = temp.Id,
                Descripcion = temp.Descripcion,
                Permisos = string.Join(", ", temp.Permisos.Select(x => x.Permiso.DisplayEnum())),
                PermisosEnum = temp.Permisos.Select(x => x.Permiso)

            };
               return rolPermiso;
        }


        RolDto IConsultaEscalar<RolDto>.Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, rolId);
            }
        }
    }
}
