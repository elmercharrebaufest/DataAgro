using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using Molinos.DataAgro.Entities.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;
using Molinos.DataAgro.Entities.Seguridad;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodoRago : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodoRago(DataSourceRequest request)
        {
            this.request = request;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request)
        {
            
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryRango =
                from rango in contexto.Set<RangoConfirmacionAutomatica>()
                select new RangoConfirmacionAutomaticaDto
                {
                    Id = rango.Id,
                    PrecioMinimo = rango.PrecioMinimo,
                    PrecioMaximo = rango.PrecioMaximo,
                    Material = rango.Material.Descripcion,
                    MaterialId = rango.MaterialId,
                    Moneda = rango.Moneda.Descripcion,
                    MonedaId = rango.MonedaId,
                    FechaDesde = rango.FechaDesde,
                    HastaAnio = rango.HastaAnio,
                    ZonaId = rango.ZonaId ?? 0,
                    Zona = rango.Zona.Descripcion,
                    Cantidad = rango.Cantidad,
                    DesdeAnio = rango.DesdeAnio,
                    DesdeMes = rango.DesdeMes,
                    HastaMes = rango.HastaMes,
                    FechaHasta = rango.FechaHasta,
                    TipoNegocio = rango.TipoNegocio.Descripcion
                };

            GridHelper.TruncateTime(request.Filter, ref queryRango);
            return queryRango.ToDataSourceResult<RangoConfirmacionAutomaticaDto>(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
