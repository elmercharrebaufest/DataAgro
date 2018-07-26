using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ReportesController : Controller
    {
        private IHomeManager mobjHomeManager;
        private IReportesManager mobjReportesManager;
        private IComercialManager mobcomercialmanager;
        private string idActiveDirectory;
        
        public ReportesController(IMSContextProvider oMSContextProvider, IReportesManager oReportesManager, IHomeManager oHomeManager, IComercialManager oMScomercialmanager)
        {
            mobjReportesManager = oReportesManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjHomeManager = oHomeManager;
            mobcomercialmanager = oMScomercialmanager;
        }

        public ActionResult Index()
        {
            string ActionView = "";
            ViewBag.esadmin = false;

            if (!mobcomercialmanager.ComercialExiste(idActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            else if (GlobalVariables.EsAdministrador)
            {

                ViewBag.esadmin = true;
            }

            return View(ActionView);
        }

        public async Task<ActionResult> TraerDatosCombo()
        {
            var model =  await mobjReportesManager.TraerDatosIniciales(idActiveDirectory);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> TraerDatosReporteComprasBarra(ParamReportes oParamReportes)
        {
            List<ResultComprasBarrasReportes> model = new List<ResultComprasBarrasReportes>();

            
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            

            model = await mobjReportesManager.TraerComprasBarra(oParamReportes);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerDatosReporteObjetivoGauge(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            var model = await mobjReportesManager.TraerObjetivosGauge(oParamReportes);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerDatosReporteCompras(ParamReportes oParamReportes)
        {
           dynamic model = new List<ResulIndicadores>();

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            

            if (oParamReportes.Indicadores== "compras")
            {
                if (oParamReportes.Grafico =="mapa")
                {
                  model = await mobjReportesManager.TraerComprasMapa(oParamReportes);
                }

                if (oParamReportes.Grafico == "torta")
                {
                    model = await mobjReportesManager.TraerComprasTorta(oParamReportes);
                }
 
            }

            if (oParamReportes.Indicadores == "capacidadproductiva")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    model = await mobjReportesManager.TraerCapacidadProductivaMapa(oParamReportes);
                }

                if (oParamReportes.Grafico == "barras")
                {
                    model = await mobjReportesManager.TraerCapacidadProductivaBarra(oParamReportes);
                }

              
            }

            if (oParamReportes.Indicadores == "capacidadacopio")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                    model = await mobjReportesManager.TraerCapacidadDeAcopioMapa(oParamReportes);
                }

                if (oParamReportes.Grafico == "barras")
                {
                    model = await mobjReportesManager.TraerCapacidadDeAcopioBarra(oParamReportes);
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

        

        public async Task<ActionResult> TraerDatosGrillaBD(ParamReportes oParamReportes)
        {

            var ComercialId =  await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            




            var model = await mobjReportesManager.TraerDatosGrillaBD(oParamReportes);


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> ExportarIndicadores(ParamReportes oParamReportes)
        {
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            

            List<ResultIndicadoresReportesmini> ListaResult =  new List<ResultIndicadoresReportesmini>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos =  mobjReportesManager.TransformarFiltros(oParamReportes);
            if (oParamReportes.Indicadores == "compras")
            {
                if (oParamReportes.Grafico == "mapa")
                {
                   ListaResult = await mobjReportesManager.TraerComprasMapaExportacion(filtrosconvertidos);
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
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            

            List<ResultIndicadoresReportesmini> ListaResult = new List<ResultIndicadoresReportesmini>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            ListaResult = await mobjReportesManager.TraerComprasMapaExportacion(filtrosconvertidos);

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
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            

            List<ResultIndicadoresReportesTorta> ListaResult = new List<ResultIndicadoresReportesTorta>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            ListaResult = await mobjReportesManager.TraerComprasTortaExportacion(filtrosconvertidos);

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

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultComprasBarrasReportesmini> ListaResult = new List<ResultComprasBarrasReportesmini>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerComprasBarraExportacion(filtrosconvertidos);

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

            
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            BaseDeDatosReturn ListaResult = new BaseDeDatosReturn();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();

            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);

            ListaResult = await mobjReportesManager.TraerDatosGrillaBD(oParamReportes);

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

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultObjetivoGaugeReportes> ListaResult = new List<ResultObjetivoGaugeReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerObjetivosGaugeExportacion(filtrosconvertidos);

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

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultProduccionMapaReportes> ListaResult = new List<ResultProduccionMapaReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerCapacidadProductivaMapaExportacion(filtrosconvertidos);

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

            
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultProduccionBarraReportes> ListaResult = new List<ResultProduccionBarraReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerCapacidadProductivaBarraExportacion(filtrosconvertidos);

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

            
            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultAcopioMapaReportes> ListaResult = new List<ResultAcopioMapaReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerCapacidadDeAcopioMapaExportacion(filtrosconvertidos);

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

            oParamReportes.ComercialActual = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            


            List<ResultAcopioBarraReportes> ListaResult = new List<ResultAcopioBarraReportes>();

            var oLstIndicadores = new LstIndicadores(mobjReportesManager);

            var model = new ReportesModel();


            var filtrosconvertidos = mobjReportesManager.TransformarFiltros(oParamReportes);


            ListaResult = await mobjReportesManager.TraerCapacidadDeAcopioBarraExportacion(filtrosconvertidos);

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


 