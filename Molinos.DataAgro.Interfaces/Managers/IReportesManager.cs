using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IReportesManager
    {

        Resultado GrabarReporte(Reportes oReporte);

        ReportesDto ObtenerReporte(string identificador);

        DatosInicialesReportes TraerDatosIniciales(string idActiveDirectory);

        BaseDeDatosReturn TraerDatosGrillaBD(ParamReportes oParamReportes);

        List<ResulIndicadores> TraerComprasMapa(ParamReportes oParamReportes);

        List<ResulIndicadores> TraerComprasTorta(ParamReportes oParamReportes);

        List<ResultComprasBarrasReportes> TraerComprasBarra(ParamReportes oParamReportes);
        
        List<ResulIndicadores> TraerCapacidadProductivaMapa(ParamReportes oParamReportes);

        List<ResultComprasBarrasReportes> TraerCapacidadProductivaBarra(ParamReportes oParamReportes);
        
        List<ResulIndicadores> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes);
         
        List<ResultComprasBarrasReportes> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes);


        #region Objetivos
        List<ResultObjetivoGaugeReportes> TraerObjetivosGauge(ParamReportes oParamReportes);
        List<ResultObjetivoGaugeReportes> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes); 
        #endregion

        
        List<ResulIndicadores> TraerBasedeDatos(ParamReportes oParamReportes);

        ParamReportes TransformarFiltros(ParamReportes oParamReportes);
        
        List<ResultIndicadoresReportesmini>TraerComprasMapaExportacion(ParamReportes oParamReportes);
           
        List<ResultIndicadoresReportesTorta> TraerComprasTortaExportacion(ParamReportes filtrosconvertidos);

        List<ResultComprasBarrasReportesmini>TraerComprasBarraExportacion(ParamReportes filtrosconvertidos);
         
        List<ResultProduccionMapaReportes> TraerCapacidadProductivaMapaExportacion(ParamReportes filtrosconvertidos);
        List<ResultProduccionBarraReportes> TraerCapacidadProductivaBarraExportacion(ParamReportes filtrosconvertidos);
        
        List<ResultAcopioBarraReportes> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes);
        List<ResultAcopioMapaReportes> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes);
        List<ToneladasGranoTipoDto> TraerToneladasGranoTipo();
        ReporteSojaSustDto TraerToneladasSojaSust();
        List<PosicionComprasDto> TraerPosicionCompras();
        List<PrecioCantidadDto> TraerMonedaCantidad();
        ExcelDetallePosicionDto DetallePosicion(int materialId, int mes, bool? calidad);
    }
}
