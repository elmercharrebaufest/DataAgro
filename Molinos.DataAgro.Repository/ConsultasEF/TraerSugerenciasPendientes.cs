using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerSugerenciasPendientes : IConsultaEscalar<bool>
    {
      
        private readonly int comercial;

        public TraerSugerenciasPendientes(int comercial)
        {            
            this.comercial = comercial;            
        }

        private static bool Query(DbContext contexto, int comercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var hoy = DateTime.Today;
            var query =
                from cupo in contexto.Set<SugerenciaCupo>()               
                where cupo.ComercialId == comercial && cupo.FechaSugerida >= hoy                
                select new SugerenciaCupoDto { Id = cupo.Id};
            return query.Any();
        }

        public virtual bool Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, comercial);
            }
        }
    }
}
