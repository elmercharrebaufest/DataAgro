using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosLogsDataAgro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodosLogsDataAgro(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }
        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryLogs =
                from log in contexto.Set<LogDataAgro>()
                where /*equipo.Contains(cupo.ComercialId != null ? cupo.ComercialId.Value : 0)*/
                log.Usuario != null

                select new LogDataAgroDto
                {
                    Id = log.Id,
                    Usuario = log.Usuario,
                    Fecha = log.Fecha,
                    Clase = log.Clase,
                    AccionRealizada = log.AccionRealizada,
                    DatoModificado = log.DatoModificado,
                    ClaseId = log.ClaseId,
                    Descripcion = log.Descripcion,
                    ProveedorId = log.ProveedorId,
                    CorredorId = log.CorredorId,
                };
            GridHelper.TruncateTime(request.Filter, ref queryLogs);

            return queryLogs.ToDataSourceResult<LogDataAgroDto>(request);
        }
    }
}



