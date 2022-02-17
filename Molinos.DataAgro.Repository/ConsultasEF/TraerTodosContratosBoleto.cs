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
    public class TraerTodosContratosBoleto : IConsultaEscalar<IQueryable<BasicoContrato>>
    {
        private readonly List<string> contratos;
        private readonly List<int> equipo;
        private readonly bool corredor;
        private readonly List<int> corredoresComercial;

        public TraerTodosContratosBoleto(List<string> contratos, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            this.contratos = contratos;
            this.equipo = equipo;
            this.corredor = corredor;
            this.corredoresComercial = corredoresComercial;
        }

        private static IQueryable<BasicoContrato> Query(DbContext contexto, List<string> contratos, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, equipo);
            //GridHelper.TruncateTime(request.Filter, ref queryContratos);
            //return queryContratos.Where(x => (!(string.IsNullOrEmpty( x.Negocio)) ? contratos.All(c => x.Negocio.Contains(c)) : contratos.All(c => x.ContratoSAP.Contains(c)))  && x.Estado == 5);
            return queryContratos.Where(x=> contratos.Contains(x.Negocio) && x.Estado == 5);
        }

        public virtual IQueryable<BasicoContrato> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, contratos, equipo);
            }
        }
    }
}