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
    public class TraerTodoPesificadoSinFiltro /*: IConsultaEscalar<DataSourceResult>*/
    {
        //private readonly DataSourceRequest request;
        //private readonly List<int> equipo;

        //public TraerTodoPesificado(DataSourceRequest request, List<int> equipo)
        //{
        //    this.request = request;
        //    this.equipo = equipo;
        //}

        public static IQueryable<ReportePesificadoDto> Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {

            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryRango =
                from item in contexto.Set<ReportePesificado>()
                join np in contexto.Set<NegocioPesificacion>() on item.NegocioId equals np.Negocio.Id into nps
                from np in nps.DefaultIfEmpty()
                where (equipo.Contains(item.ComercialId.Value) || item.ComercialId == null) && item.KgTotales > 0
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
                    KgTotales = item.KgTotales,
                    Unidad = item.Unidad,
                    Precio = item.Precio,
                    NombreCorredor = item.NombreCorredor,
                    NombreVendedor = item.NombreVendedor,
                    NingunDolarizado = item.Dolarizado == false && item.DolarizadoExpress == false && item.DolarizadoNoProductor == false,

                    USDPesificable = (item.KgVencimientoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) > 0 ?
                    ((item.KgVencimientoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0,

                    USDNoPesificable = (item.KgNoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) > 0 ?
                    ((item.KgNoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0,

                    USDTotal = (item.KgTotales * (item.Precio != null && item.Precio != 0 ? item.Precio : 1) > 0 ?
                    ((item.KgTotales * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0),

                    USDTotalizador = ((item.KgVencimientoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) > 0 ?
                    ((item.KgVencimientoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0) +
                    ((item.KgNoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) > 0 ?
                    ((item.KgNoPesificable * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0) +
                    (item.KgTotales * (item.Precio != null && item.Precio != 0 ? item.Precio : 1) > 0 ?
                    ((item.KgTotales * (item.Precio != null && item.Precio != 0 ? item.Precio : 1)) / 1000) : 0),
                    Pase = item.Pase,
                    Plus = item.Plus,
                    Posicion = item.Posicion,
                    KgTotalesPase = item.KgTotalesPase,
                    Cantidad = item.Cantidad,
                    CantidadRecibida = item.CantidadRecibida,
                    Excepcion = np.Excepcion,
                    FechaInstruccion = np.FechaInstruccion != null ? DbFunctions.TruncateTime(np.FechaInstruccion) : (DateTime?)null,
                    EsCorredor = string.IsNullOrEmpty(item.CuitCorredor) ? false : true,
                    EsOperacionDirecta = string.IsNullOrEmpty(item.CuitVendedor) ? false : true,
                    Cesion = item.Cesion,
                    CesionDescripcion = item.Cesion == true ? "SI" : "NO",
                    Status = item.Status,
                    StatusDescripcion = item.Status == "" ? "Slip" : item.Status == "A" ? "Con Anulación Automática" : 
                    item.Status == "X" ? "Confirmado" : item.Status == "F" ? "Liquidación Finalizada" :
                    item.Status == "C" ? "Cumplido" : item.Status == "M" ? "Con Anulación parcial" :
                    item.Status == "B" ? "Contrato Anulado Totalmente" : item.Status == "K" ? "Cumplido en Camiones" :
                    item.Status == "T" ? "Contrato de Canje Cerrado" : item.Status == "J" ? "Prefijación Cerrada" : "",
                    NegocioId = np.NegocioId,
                    NegocioPesificacionId = np.Id,
                };
            queryRango = queryRango.GroupBy(x =>new { x.NegocioId, x.Contrato, x.Fijacion }).Select(x => x.OrderByDescending(y=> y.NegocioPesificacionId).FirstOrDefault());
            GridHelper.TruncateTime(request.Filter, ref queryRango);
            return queryRango;
        }

        //public virtual DataSourceResult Ejecutar(DbContext contexto)
        //{
        //    using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
        //    {
        //        return Query(contexto, request, equipo);
        //    }
        //}
    }
}
