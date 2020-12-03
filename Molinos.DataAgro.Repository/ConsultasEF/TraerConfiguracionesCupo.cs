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
    public class TraerConfiguracionesCupo : IConsultaEscalar<KendoGrid<ConfiguracionCupoDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerConfiguracionesCupo(KendoGridMvcRequest request)
        {
            this.request = request;            
        }

        private static KendoGrid<ConfiguracionCupoDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var hoy = DateTime.Today;
            var query =
                from cupo in contexto.Set<ConfiguracionCupo>() 
                where cupo.Fecha >= hoy
                select new ConfiguracionCupoDto()
                {
                    Id = cupo.Id,
                    LimiteCupo = cupo.LimiteCupo,
                    Fecha = cupo.Fecha,
                    Material = cupo.Material.Descripcion,
                    Centro = cupo.Centro.Descripcion,
                    CentroId = cupo.CentroId,
                    MaterialId = cupo.MaterialId,
                    BloquearCupera = cupo.CierreCupera ? "Si":"No"
                };
            
            return new KendoGrid<ConfiguracionCupoDto>(request, query);
        }

        public virtual KendoGrid<ConfiguracionCupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
