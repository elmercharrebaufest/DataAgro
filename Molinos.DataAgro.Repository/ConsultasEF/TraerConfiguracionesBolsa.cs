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
    public class TraerConfiguracionesBolsa : IConsultaEscalar<KendoGrid<ConfiguracionBolsaDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerConfiguracionesBolsa(KendoGridMvcRequest request)
        {
            this.request = request;            
        }

        private static KendoGrid<ConfiguracionBolsaDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var hoy = DateTime.Today;
            var query =
                from bolsa in contexto.Set<ConfiguracionBolsa>()               
                select new ConfiguracionBolsaDto()
                {
                    Id = bolsa.Id,                    
                    Bolsa = bolsa.Bolsa.Descripcion,
                    Destino = bolsa.Destino.Descripcion,
                    Provincia = bolsa.Provincia.Nombre,
                    BolsaId = bolsa.BolsaId,
                    DestinoId = bolsa.DestinoId,
                    ProvinciaId = bolsa.ProvinciaId
                };
            
            return new KendoGrid<ConfiguracionBolsaDto>(request, query);
        }

        public virtual KendoGrid<ConfiguracionBolsaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
