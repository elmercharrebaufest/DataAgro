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
    public class TraerSituacionCultivoParcial : IConsultaEscalar<KendoGrid<ResearchSituacionCultivoDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerSituacionCultivoParcial(KendoGridMvcRequest request)
        {
            this.request = request;
        }

        private static KendoGrid<ResearchSituacionCultivoDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                from situacion in contexto.Set<ResearchSituacionCultivo>()
                select new ResearchSituacionCultivoDto()
                {
                    Id = situacion.Id,
                    Comercial = situacion.Comercial.Apellido + " " + situacion.Comercial.Nombres,
                    FechaHora = situacion.FechaHora,
                    Localidad = situacion.Localidad.Nombre+" ("+situacion.Localidad.Provincia.Nombre+")",
                    Partido=situacion.Localidad.Partido.Descripcion,
                    LocalidadId = situacion.LocalidadId,
                    Material = situacion.Material.Descripcion,
                    MaterialId = situacion.MaterialId,
                    Observaciones = situacion.Observaciones,
                    Estadio = situacion.Estadio,
                    Situacion = situacion.Situacion
                };


            return new KendoGrid<ResearchSituacionCultivoDto>(request, queryContratos);
        }

        public virtual KendoGrid<ResearchSituacionCultivoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
