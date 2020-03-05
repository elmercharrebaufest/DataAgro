using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;
using Molinos.DataAgro.Entities.Helpers;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosContratos : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;
        private readonly bool corredor;
        private readonly List<int> corredoresComercial;

        public TraerTodosContratos(DataSourceRequest request, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            this.request = request;
            this.equipo = equipo;
            this.corredor = corredor;
            this.corredoresComercial = corredoresComercial;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request,  List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto,  equipo);
            GridHelper.TruncateTime(request.Filter,ref queryContratos);
            return queryContratos.ToDataSourceResult(request);
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
