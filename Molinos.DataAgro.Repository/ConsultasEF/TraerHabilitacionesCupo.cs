using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerHabilitacionesCupo : IConsultaEscalar<KendoGrid<HabilitacionCupoDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerHabilitacionesCupo(KendoGridMvcRequest request)
        {
            this.request = request;            
        }

        private static KendoGrid<HabilitacionCupoDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {          
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query =
                from cupo in contexto.Set<HabilitacionCupo>()
                select new HabilitacionCupoDto()
                {
                    Id = cupo.Id,
                    FechaDesde = cupo.FechaDesde,
                    FechaHasta = cupo.FechaHasta,
                    ZonaCupoId = cupo.ZonaCupoId,
                    Zona = string.IsNullOrEmpty(cupo.ZonaCupo.Descripcion) ? "TODAS" : cupo.ZonaCupo.Descripcion,
                    Material = cupo.Material.Descripcion
                    
                };
            
            return new KendoGrid<HabilitacionCupoDto>(request, query);
        }

        public virtual KendoGrid<HabilitacionCupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
