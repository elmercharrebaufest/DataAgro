using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReportesController : Controller
    {
        private IHomeManager mobjHomeManager;
        private IReportesManager mobjReportesManager;
        
        
        public ReportesController(IReportesManager oReportesManager, IHomeManager oHomeManager)
        {
            mobjReportesManager = oReportesManager;            
            mobjHomeManager = oHomeManager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarIndicadores)]
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult TraerDatosCombo()
        {
            return new JsonResult()
            {
                Data = mobjReportesManager.TraerDatosIniciales(GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerDatosReporteComprasBarra(ParamReportes oParamReportes)
        {
            List<ResultComprasBarrasReportes> model = new List<ResultComprasBarrasReportes>();

            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            model = mobjReportesManager.TraerComprasBarra(oParamReportes,GlobalVariables.Equipo);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerDatosReporteObjetivoGauge(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            var model = mobjReportesManager.TraerObjetivosGauge(oParamReportes, GlobalVariables.Equipo);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerDatosReporteCompras(ParamReportes oParamReportes)
        {
            dynamic model = new List<ResulIndicadores>();

            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);


            if (oParamReportes.Indicadores == "compras")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    model = mobjReportesManager.TraerComprasMapa(oParamReportes,GlobalVariables.Equipo);
                }

                if (oParamReportes.Grafico == "torta")
                {
                    model = mobjReportesManager.TraerComprasTorta(oParamReportes, GlobalVariables.Equipo);
                }

            }

            if (oParamReportes.Indicadores == "capacidadproductiva")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    model = mobjReportesManager.TraerCapacidadProductivaMapa(oParamReportes, GlobalVariables.Equipo);
                }

                if (oParamReportes.Grafico == "barras")
                {
                    model = mobjReportesManager.TraerCapacidadProductivaBarra(oParamReportes, GlobalVariables.Equipo);
                }
            }

            if (oParamReportes.Indicadores == "capacidadacopio")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    model = mobjReportesManager.TraerCapacidadDeAcopioMapa(oParamReportes, GlobalVariables.Equipo);
                }

                if (oParamReportes.Grafico == "barras")
                {
                    model = mobjReportesManager.TraerCapacidadDeAcopioBarra(oParamReportes, GlobalVariables.Equipo);
                }
            }

            if (oParamReportes.Indicadores == "basededatos")
            {
                if (oParamReportes.Grafico == "torta")
                {

                }

            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerDatosGrillaBD(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            return new JsonResult()
            {
                Data = mobjReportesManager.TraerDatosGrillaBD(oParamReportes, GlobalVariables.Equipo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ExportarIndicadores(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            

            List<ResultIndicadoresReportesmini> ListaResult =  new List<ResultIndicadoresReportesmini>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos =  mobjReportesManager.TransformarFiltros(oParamReportes);
            if (oParamReportes.Indicadores == "compras")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                   ListaResult = mobjReportesManager.TraerComprasMapaExportacion(filtrosconvertidos, GlobalVariables.Equipo);
                }

                if (oParamReportes.Grafico == "torta")
                {
                    //ListaResult = await mobjReportesManager.TraerComprasTortaExportacion(filtrosconvertidos);
                }
            }

            if (oParamReportes.Indicadores == "capacidadproductiva")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    //ListaResult = await mobjReportesManager.TraerCapacidadProductivaMapaExportacion(filtrosconvertidos);
                }

                if (oParamReportes.Grafico == "barra")
                {
                   // ListaResult = await mobjReportesManager.TraerCapacidadProductivaBarraExportacion(filtrosconvertidos);
                }
            }

            if (oParamReportes.Indicadores == "capacidadacopio")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    //ListaResult = await mobjReportesManager.TraerCapacidadDeAcopioMapaExportacion(oParamReportes);
                }

                if (oParamReportes.Grafico == "barra")
                {
                    //ListaResult = await mobjReportesManager.TraerCapacidadDeAcopioBarraExportacion(oParamReportes);
                }
            }

            if (oParamReportes.Indicadores == "basededatos")
            {
            }

            var datos = new ResultComprasReportesExcel
            {
                Titulo = "Titulo de Prueba",
                Lista = ListaResult,

                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarComprasMapaExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        #region Exportacion
        public async Task<ActionResult> ExportarCompraMapaIndicadores(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            

            List<ResultIndicadoresReportesmini> ListaResult = new List<ResultIndicadoresReportesmini>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            ListaResult = mobjReportesManager.TraerComprasMapaExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultComprasReportesExcel
            {
                Titulo = "Compras",
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            var identif = await oLstIndicadores.GenerarComprasMapaExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarCompraTortaIndicadores(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            
            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            var ListaResult = mobjReportesManager.TraerComprasTortaExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultComprasTortaReportesExcel
            {
                Titulo = "Compras",
                Lista = ListaResult,
                oFiltros = oParamReportes
            };

            var identif = await oLstIndicadores.GenerarComprasMapaExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarComprasBarrasIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Compras";

            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            var ListaResult = mobjReportesManager.TraerComprasBarraExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultComprasBarrasReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,
                
                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarComprasBarraExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarBD(ParamReportes oParamReportes)
        {

            string Mititulo = "Base De Datos";

            
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            
            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            var ListaResult = mobjReportesManager.TraerDatosGrillaBD(oParamReportes, GlobalVariables.Equipo);

            var datos = new ResultDBExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult.valoresGrilla,
                oFiltros = filtrosconvertidos
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarObjetivosDBExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarObjetivosGaugeIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Objetivos";

            oParamReportes.ComercialActual =  mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            


            List<ResultObjetivoGaugeReportes> ListaResult = new List<ResultObjetivoGaugeReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = mobjReportesManager.TraerObjetivosGaugeExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultObjetivoGaugeReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarObjetivosGaugeExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarProduccionMapaIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Produccion";

            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            


            List<ResultProduccionMapaReportes> ListaResult = new List<ResultProduccionMapaReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = mobjReportesManager.TraerCapacidadProductivaMapaExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultProduccionMapaReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarProduccionMapaExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarProduccionBarraIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Produccion";

            
            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            


            List<ResultProduccionBarraReportes> ListaResult = new List<ResultProduccionBarraReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = mobjReportesManager.TraerCapacidadProductivaBarraExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultProduccionBarraReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarProduccionBarraExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarAcopioMapaIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Acopio";

            
            oParamReportes.ComercialActual =  mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            


            List<ResultAcopioMapaReportes> ListaResult = new List<ResultAcopioMapaReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = mobjReportesManager.TraerCapacidadDeAcopioMapaExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultAcopioMapaReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarAcopioMapaExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ExportarAcopioBarraIndicadores(ParamReportes oParamReportes)
        {

            string Mititulo = "Acopio";

            oParamReportes.ComercialActual = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            


            List<ResultAcopioBarraReportes> ListaResult = new List<ResultAcopioBarraReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = mobjReportesManager.TraerCapacidadDeAcopioBarraExportacion(filtrosconvertidos, GlobalVariables.Equipo);

            var datos = new ResultAcopioBarraReportesExcel
            {
                Titulo = Mititulo,
                Lista = ListaResult,


                oFiltros = oParamReportes
            };

            //var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstIndicadores.GenerarAcopioBarraExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        #endregion

    }
    
}


 