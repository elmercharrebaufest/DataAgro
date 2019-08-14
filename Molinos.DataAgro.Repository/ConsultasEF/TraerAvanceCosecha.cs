using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerAvanceCosecha : IConsultaEscalar<KendoGrid<ResearchAvanceCosechaDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerAvanceCosecha(KendoGridMvcRequest request)
        {
            this.request = request;
        }

        private static KendoGrid<ResearchAvanceCosechaDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                from avance in contexto.Set<ResearchAvanceCosecha>()
                select new ResearchAvanceCosechaDto()
                {
                    Id = avance.Id,
                    Avance = avance.Avance,
                    Comercial = avance.Comercial.Apellido + " " + avance.Comercial.Nombres,
                    FechaHora = avance.FechaHora,
                    Localidad = avance.Localidad.Nombre,
                    Provincia = avance.Localidad.Provincia.Nombre,
                    LocalidadId = avance.LocalidadId,
                    Partido = avance.Localidad.Partido.Descripcion,
                    Material = avance.Material.Descripcion,
                    MaterialId = avance.MaterialId,
                    Observaciones = avance.Observaciones,
                    RangoDesde = avance.RangoDesde,
                    RangoHasta = avance.RangoHasta,
                    Rendimiento = avance.Rendimiento,
                    Campania = avance.Campania.Descripcion,
                    CampaniaId = avance.CampaniaId
                };


            return new KendoGrid<ResearchAvanceCosechaDto>(request, queryContratos);
        }

        public virtual KendoGrid<ResearchAvanceCosechaDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
