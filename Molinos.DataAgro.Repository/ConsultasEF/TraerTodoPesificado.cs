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
    public class TraerTodoPesificado : IConsultaEscalar<DataSourceResult>
    {       
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodoPesificado(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryRango =
                from item in contexto.Set<ReportePesificado>()
                where equipo.Contains(item.ComercialId)
                select new ReportePesificadoDto
                {
                    Id = item.Id,
                    MaterialDesc = item.Material.Descripcion,
                    MaterialId = item.MaterialId,
                    MonedaId = item.Moneda.Descripcion,
                    CantidadPendiente = item.CantidadPendiente,
                    Clasificacion = item.Clasificacion,
                    ComercialDesc = item.Comercial.Nombres + " " + item.Comercial.Apellido,
                    ComercialId = item.ComercialId,
                    Contrato = item.Contrato,
                    CuitCorredor = item.CuitCorredor,
                    CuitVendedor = item.CuitVendedor,
                    Dolarizado = item.Dolarizado,
                    DolarizadoExpress = item.DolarizadoExpress,
                    FechaFijacion = item.FechaFijacion,
                    DolarizadoNoProductor = item.DolarizadoNoProductor,
                    FechaHastaDolarizado = item.FechaHastaDolarizado != null ? DbFunctions.TruncateTime(item.FechaHastaDolarizado) : (DateTime?)null,
                    FechaUltimaAplicacion = item.FechaUltimaAplicacion != null ? DbFunctions.TruncateTime(item.FechaUltimaAplicacion) : (DateTime?)null,
                    Fijacion = item.Fijacion,
                    KgNoPesificable = item.KgNoPesificable,
                    KgVencimientoPesificable = item.KgVencimientoPesificable,
                    KgTotales = item.KgNoPesificable + item.KgVencimientoPesificable,
                    Unidad = item.Unidad,
                    Precio = item.Precio,
                    NombreCorredor = item.NombreCorredor,
                    NombreVendedor = item.NombreVendedor 
                };

            GridHelper.TruncateTime(request.Filter, ref queryRango);
            return queryRango.ToDataSourceResult<ReportePesificadoDto>(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
