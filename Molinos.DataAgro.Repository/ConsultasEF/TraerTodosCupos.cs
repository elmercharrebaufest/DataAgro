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
    public class TraerTodosCupos : IConsultaEscalar<KendoGrid<CupoDto>>
    {
        private readonly KendoGridMvcRequest request;
        private readonly List<int> equipo;

        public TraerTodosCupos(KendoGridMvcRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static KendoGrid<CupoDto> Query(DbContext contexto, KendoGridMvcRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryContratos =
                from cupo in contexto.Set<Cupo>()
                where equipo.Contains(cupo.ComercialId != null ? cupo.ComercialId.Value : 0)
                
                select new CupoDto()
                {
                    Id=cupo.Id,
                    FechaIngreso = cupo.FechaIngreso,
                    Comercial=cupo.Comercial.Nombres+" "+cupo.Comercial.Apellido,
                    CupoSap = cupo.CupoSap,
                    Material = cupo.Material.Descripcion,
                    Proveedor=cupo.Proveedor.RazonSocial,
                    Destinatario = cupo.Destinatario,
                    Centro = cupo.Centro.Descripcion,
                    CentroId = cupo.CentroId,
                    Calidad = cupo.Calidad,
                    ZonaCupo = cupo.ZonaCupo.Descripcion,
                    FleteProcedencia = cupo.FleteProcedencia,
                    Observaciones = cupo.Observaciones,
                    EstadoCupoId = cupo.EstadoCupoId,
                    EstadoCupo = cupo.EstadoCupo.Descripcion,
                    MensajeError = cupo.ErrorStop
                };

            return new KendoGrid<CupoDto>(request, queryContratos);
        }

        public virtual KendoGrid<CupoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
