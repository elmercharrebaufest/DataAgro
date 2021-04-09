using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IReportesManager
    {

        Resultado GrabarReporte(Reportes oReporte);

        ReportesDto ObtenerReporte(string identificador);

        DatosInicialesReportes TraerDatosIniciales(string idActiveDirectory);

        BaseDeDatosReturn TraerDatosGrillaBD(ParamReportes oParamReportes, List<int> equipo);

        List<ResulIndicadores> TraerComprasMapa(ParamReportes oParamReportes, List<int> equipo);

        List<ResulIndicadores> TraerComprasTorta(ParamReportes oParamReportes, List<int> equipo);

        List<ResultComprasBarrasReportes> TraerComprasBarra(ParamReportes oParamReportes, List<int> equipo);

        List<ResulIndicadores> TraerCapacidadProductivaMapa(ParamReportes oParamReportes, List<int> equipo);

        List<ResultComprasBarrasReportes> TraerCapacidadProductivaBarra(ParamReportes oParamReportes, List<int> equipo);

        List<ResulIndicadores> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes,List<int> equipo);

        List<ResultComprasBarrasReportes> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes, List<int> equipo);


        #region Objetivos
        List<ResultObjetivoGaugeReportes> TraerObjetivosGauge(ParamReportes oParamReportes, List<int> equipo);
        List<ResultObjetivoGaugeReportes> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes, List<int> equipo);
        #endregion


        List<ResulIndicadores> TraerBasedeDatos(ParamReportes oParamReportes);

        ParamReportes TransformarFiltros(ParamReportes oParamReportes);

        List<ResultIndicadoresReportesmini> TraerComprasMapaExportacion(ParamReportes oParamReportes, List<int> equipo);

        List<ResultIndicadoresReportesTorta> TraerComprasTortaExportacion(ParamReportes filtrosconvertidos, List<int> equipo);
        ReporteCompraNetModel ObtenerDatosReporteCompraNet(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId);
        void GrabarDatosReporteCompraNet(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId);
        List<ResultComprasBarrasReportesmini> TraerComprasBarraExportacion(ParamReportes filtrosconvertidos, List<int> equipo);

        List<ResultProduccionMapaReportes> TraerCapacidadProductivaMapaExportacion(ParamReportes filtrosconvertidos, List<int> equipo);
        List<ResultProduccionBarraReportes> TraerCapacidadProductivaBarraExportacion(ParamReportes filtrosconvertidos, List<int> equipo);

        List<ResultAcopioBarraReportes> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes, List<int> equipo);
        List<ResultAcopioMapaReportes> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes, List<int> equipo);
        List<ToneladasGranoTipoDto> TraerToneladasGranoTipo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0);
        ReporteSojaSustDto TraerToneladasSojaSust(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0);
        List<PosicionComprasDto> TraerPosicionCompras(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0,bool TraerPosicionMaterialCampaña = false);
        List<PricingCampaniaDto> TraerPricingCampania(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0);
        List<PrecioCantidadDto> TraerMonedaCantidad(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0);
        List<HedgeMaterialDto> TraerTodosHedgeMaterial(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId);
        HedgeCargaObjetivoDto TraerHedgeObjetivo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId);
        HedgeCargaObjetivoDto TraerUltimoHedgeObjetivo();
        HedgeTCPromedioDto TraerTcPromedio(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId);
        List<AgenteCompraDto> TraerAgenteDeCompra(DateTime fechaDesde, DateTime fechaHasta,List<int> materialId);
        ExcelDetallePosicionDto DetallePosicion(int materialId, int mes, int anio, DateTime fechaDesde, DateTime fechaHasta, int? calidad, int centroId = 0);
        ExcelDetallePosicionDto DetalleAgente(DateTime fecha, List<int> materialId);
        List<ExcelPosicionMaterialDto> PosicionPorMaterial(DateTime fechaDesde, DateTime fechaHasta);
        string DetallePosicionModal(int materialId, int? mes, int? anio, DateTime fechaDesde, DateTime fechaHasta, int? calidad,  int centroId = 0);
        string DetalleAgenteModal(DateTime fecha,List<int> materialId);
        string DetallePosicionModalIds(List<int> negocios,string moneda);
        void TraerPosicionNegocios(List<BasicoContrato> datos);
        ReporteEvolucionFijacionModel ObtenerDatosReporteEvolucionFijacion(DateTime desde, DateTime hasta, int? ProveedorId, int? ComercialId, int? CampanaId, int? MaterialId,
            int? GrupoCompraId, int? ClasificacionId, int? DestinoId);
        List<ClasificacionCompraNet> TraerTodoClasificacionCompraNet();
        List<PesificarAgentDto> GrabarTodoDatoPesificar();
        //List<ReportePesificadoDto> TraerTodoDatoPesificado(string contrato, DateTime? fechaHastaDolarizado, decimal? kilosPesificable, int? ComercialId,
        //  int? precio, decimal? noPesificables, decimal? total, string vendedor, string corredor, bool? dolarizado, bool? dolarizadoExpress, bool? dolarizadoNoP);
        DataSourceResult TraerTodoDatoPesificado(DataSourceRequest filtro, List<int> equipo);
        DataSourceResult TraerTodoPrecioMoaPizarra(DataSourceRequest request);
    }
}
