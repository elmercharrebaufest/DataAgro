using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosCupoNoPropio : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodosCupoNoPropio(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            var externo = PermisosHelper.Is(PermisosDataAgro.IngresoExterno);
            var nombreUsuario = PermisosHelper.ObtenerUsuario();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryCupos =
                from cupo in contexto.Set<CupoNoPropio>()
                join e in contexto.Set<EstadoCupo>() on cupo.Estado equals e.Id into est
                from e in est.DefaultIfEmpty()
                select new CupoNoPropioDto
                {
                    Id = cupo.Id,
                    Centro = cupo.Centro.Descripcion,
                    CentroId = cupo.CentroId,
                    Codigo = cupo.Codigo,
                    CupoId = cupo.CupoId,
                    Disponible = cupo.Disponible,
                    Estado = e.Descripcion,
                    EstadoId = cupo.Estado,
                    FechaAlta = cupo.FechaAlta,
                    FechaIngreso = cupo.FechaIngreso,
                    Material = cupo.Material.Descripcion,
                    MaterialId = cupo.MaterialId,
                    Utilizado = cupo.CupoId == null ? false : true
                };
            queryCupos = queryCupos.OrderByDescending(c => c.FechaIngreso);
            GridHelper.TruncateTime(request.Filter, ref queryCupos);

            return queryCupos.ToDataSourceResult<CupoNoPropioDto>(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
