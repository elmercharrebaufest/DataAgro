using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IReportesManager
    {

        Task<EntityErrors> GrabarReporteAsync(Reportes oReporte);

        Task<Reportes> ObtenerReporteAsync(string identificador);

        Task<DatosInicialesReportes> TraerDatosIniciales(string idActiveDirectory);

        Task<BaseDeDatosReturn> TraerDatosGrillaBD(ParamReportes oParamReportes);

        Task<List<ResulIndicadores>> TraerComprasMapa(ParamReportes oParamReportes);

        Task<List<ResulIndicadores>> TraerComprasTorta(ParamReportes oParamReportes);

        Task<List<ResultComprasBarrasReportes>> TraerComprasBarra(ParamReportes oParamReportes);

  


       Task<List<ResulIndicadores>> TraerCapacidadProductivaMapa(ParamReportes oParamReportes);

        Task<List<ResultComprasBarrasReportes>> TraerCapacidadProductivaBarra(ParamReportes oParamReportes);

      



        Task<List<ResulIndicadores>> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes);
         
        Task<List<ResultComprasBarrasReportes>> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes);


        #region Objetivos
        Task<List<ResultObjetivoGaugeReportes>> TraerObjetivosGauge(ParamReportes oParamReportes);
        Task<List<ResultObjetivoGaugeReportes>> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes); 
        #endregion



        Task<List<ResulIndicadores>> TraerBasedeDatos(ParamReportes oParamReportes);

        ParamReportes TransformarFiltros(ParamReportes oParamReportes);


        Task<List<ResultIndicadoresReportesmini>>TraerComprasMapaExportacion(ParamReportes oParamReportes);
           
        Task<List<ResultIndicadoresReportesTorta>> TraerComprasTortaExportacion(ParamReportes filtrosconvertidos);

        Task<List<ResultComprasBarrasReportesmini>>TraerComprasBarraExportacion(ParamReportes filtrosconvertidos);

 
        Task<List<ResultProduccionMapaReportes>> TraerCapacidadProductivaMapaExportacion(ParamReportes filtrosconvertidos);
        Task<List<ResultProduccionBarraReportes>> TraerCapacidadProductivaBarraExportacion(ParamReportes filtrosconvertidos);
        

        Task<List<ResultAcopioBarraReportes>> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes);
        Task<List<ResultAcopioMapaReportes>> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes);


        
    }




   


}
