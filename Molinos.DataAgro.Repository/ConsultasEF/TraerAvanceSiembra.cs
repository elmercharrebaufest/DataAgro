using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerAvanceSiembra : IConsultaEscalar<KendoGrid<ResearchAvanceSiembraDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerAvanceSiembra(KendoGridMvcRequest request)
        {
            this.request = request;
            
        }

        private static KendoGrid<ResearchAvanceSiembraDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var query =
                from research in contexto.Set<ResearchAvanceSiembra>()                
                select new ResearchAvanceSiembraDto()
                {
                    Id = research.Id,
                    Localidad = research.Localidad.Nombre + " ("+research.Localidad.Provincia.Nombre+")",
                    Partido = research.Localidad.Partido.Descripcion,
                    Material = research.Material.Descripcion,
                    IntencionSiembra = research.IntencionSiembra,
                    CambioAA = research.CambioAA,
                    Avance = research.Avance,
                    Comercial = research.Comercial.Nombres + " " + research.Comercial.Apellido,
                    FechaHora = DbFunctions.TruncateTime(research.FechaHora).Value,
                    Observaciones = research.Observaciones
                };
            
            return new KendoGrid<ResearchAvanceSiembraDto>(request, query);
        }

        public virtual KendoGrid<ResearchAvanceSiembraDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
