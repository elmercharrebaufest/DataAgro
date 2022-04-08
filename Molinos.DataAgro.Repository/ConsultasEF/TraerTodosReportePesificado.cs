using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Molinos.DataAgro.Entities.Helpers;
using System.Linq;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Repository.ConsultasEF
{

    public class TraerTodosReportePesificado : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodosReportePesificado(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult TraerTodoReporte(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryRango = TraerTodoPesificadoSinFiltro.Query(contexto, request, equipo);
            return queryRango.ToDataSourceResult<ReportePesificadoDto>(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return TraerTodoReporte(contexto, request, equipo);
            }
        }
    }

}