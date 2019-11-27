using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Data.Entity;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosContratos : IConsultaEscalar<KendoGrid<BasicoContrato>>
    {
        private readonly KendoGridMvcRequest request;
        private readonly List<int> equipo;
        private readonly bool corredor;
        private readonly List<int> corredoresComercial;

        public TraerTodosContratos(KendoGridMvcRequest request, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            this.request = request;
            this.equipo = equipo;
            this.corredor = corredor;
            this.corredoresComercial = corredoresComercial;
        }

        private static KendoGrid<BasicoContrato> Query(DbContext contexto, KendoGridMvcRequest request, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, corredor, equipo, corredoresComercial);
            return new KendoGrid<BasicoContrato>(request, queryContratos);
        }
        
        public virtual KendoGrid<BasicoContrato> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, corredor, equipo, corredoresComercial);
            }
        }
    }
}
