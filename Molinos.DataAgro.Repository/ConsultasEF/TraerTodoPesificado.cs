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
                    Pase = false,
                    KgTotalesPase = null,
                    Plus = null,
                    Posicion = "",
                };
            var queryRango2 =
                from contrato in contexto.Set<Contrato>()
                where contrato.TipoNegocioId == 1 && contrato.TipoPosicionCBOTId == 3 && contrato.EstadoId == 5
                select new ReportePesificadoDto
                {
                    Id = contrato.Id,
                    MaterialDesc = contrato.Material.Descripcion,
                    MaterialId = contrato.MaterialId,
                    MonedaId = "USD",
                    CantidadPendiente = 0,
                    Clasificacion = contrato.Clasificacion.Descripcion,
                    ComercialDesc = contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                    ComercialId = contrato.ComercialId,
                    Contrato = contrato.ContratoSAP,
                    CuitCorredor = contrato.Corredor == null ? "" : contrato.Corredor.CUIT,
                    CuitVendedor = contrato.Proveedor == null ? "" : contrato.Proveedor.CUIT,
                    Dolarizado = null,
                    DolarizadoExpress = null,
                    FechaFijacion = null,
                    DolarizadoNoProductor = null,
                    FechaHastaDolarizado = (DateTime?)null,
                    FechaUltimaAplicacion = (DateTime?)null,
                    Fijacion = "",
                    KgNoPesificable = null,
                    KgVencimientoPesificable = null,
                    KgTotales = null,
                    Unidad = "KG",
                    Precio = contrato.PrecioPonderado,
                    NombreCorredor = contrato.Corredor == null ? "" : !string.IsNullOrEmpty(contrato.Corredor.Alias) ? contrato.Corredor.Alias + " - " + contrato.Corredor.RazonSocial : contrato.Corredor.RazonSocial,
                    NombreVendedor = contrato.Proveedor == null ? "" : !string.IsNullOrEmpty(contrato.Proveedor.Alias) ? contrato.Proveedor.Alias + " - " + contrato.Proveedor.RazonSocial : contrato.Proveedor.RazonSocial,
                    NingunDolarizado = false,
                    USDPesificable = null,
                    USDNoPesificable = null,
                    USDTotal = null,
                    USDTotalizador = null,
                    Pase = true,
                    KgTotalesPase = contrato.Cantidad,
                    Plus = contrato.Descuentos.FirstOrDefault(t => t.TipoPeriodoDBId == 1 && t.TipoDBId == 1).Importe,
                    Posicion = contrato.PosicionCBOT,
                };
            var list = queryRango.Concat(queryRango2);
            GridHelper.TruncateTime(request.Filter, ref list);
            return list.ToDataSourceResult<ReportePesificadoDto>(request);
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
