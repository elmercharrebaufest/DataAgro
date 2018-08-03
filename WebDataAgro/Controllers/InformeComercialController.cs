using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class InformeComercialController : Controller
    {
        private ICondicionManager mobjCondicionManager;
        private IInformeComercialManager mobjInformeComercialManager;
        
        private IComercialManager mobjComercialManager;
        private IHomeManager mobjHomeManager;
        private IReportesManager reportesManager;

        public InformeComercialController(ICondicionManager oCondicionManager, IInformeComercialManager oInformeComercialManager, IComercialManager oComercialManager, IHomeManager oHomeManager, IReportesManager reportesManager)
        {
            mobjCondicionManager = oCondicionManager;
            mobjInformeComercialManager = oInformeComercialManager;
            

            mobjComercialManager = oComercialManager;
            mobjHomeManager = oHomeManager;
            this.reportesManager = reportesManager;

            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
        }

        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InformeAdministrativo()
        {
            return View();
        }

        public ActionResult Reporte()
        {
            return View();
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniCondicionModel();

            var result = mobjCondicionManager.TraerTodoCondicion();

            if (result != null)
            {
                model.Datos = result.Condicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarInformeComercial(Condicion oCondicion)
        {
            var model = new AbmCondicionResult();

            var entityErrors = mobjCondicionManager.GrabarCondicion(oCondicion);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.Condicion = oCondicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> Listar(ParamInformeComercial oParam)
        {
            var model = new ReportesModel();

            var ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var entityError = mobjInformeComercialManager.GrabarInformeComercial(oParam, ComercialId);

            if (!entityError.HayErrores)
            {
                var oLstInformeComercial = new LstInformeComercial(reportesManager);

                var datos = mobjInformeComercialManager.GenerarInformeComercial(oParam, (int)entityError.InformeId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);
            }
            else
            {
                model.Errores = entityError.Errores;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarMateriales(oParamInforme oParam)
        {
            var model = new InformesModel
            {
                materiales = mobjInformeComercialManager.TraerInformeComercial(oParam.filtro),
                InformeGenerado = mobjInformeComercialManager.TraerInformeComercialGenerado(oParam.filtro)
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarInformes()
        {
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.TraerInformesGenerados(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GenerarExcel(oParamExcel oParamReportes)
        {
            var model = new ReportesModel();

            var oLstIndicadores = new LstInformeComercial(reportesManager);

            var odatos = mobjInformeComercialManager.TraerCapacidadProductiva(oParamReportes.Informes);

            var identif = oLstIndicadores.GenerarInformesExcel(odatos);

            var d = mobjInformeComercialManager.GrabarCapacidadProductiva(oParamReportes.Informes);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> GenerarExcelIA(ParamReportesIC oParamReportes)
        {
            var model = new ReportesModel();

            oParamReportes.ComercialIDGenerador = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var oLstIndicadores = new LstInformeComercial(reportesManager);

            var odatos = mobjInformeComercialManager.ListarReportes(oParamReportes);

            var identif = await oLstIndicadores.GenerarInformesExcelICAsync(odatos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ReImprimirPDF(int InformeComercialId)
        {
            var model = new ReportesModel();

            var informe = mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId);

            if (informe != null)
            {
                var oLstInformeComercial = new LstInformeComercial(reportesManager);

                var datos = mobjInformeComercialManager.GenerarInformeComercial(informe, InformeComercialId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult EliminarInformeComercial(int informeComercialId)
        {
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.EliminarInformes(informeComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ModificarInformeComercial(int InformeComercialId)
        {
            return new JsonResult()
            {
                Data = new ReportesModificacionModel
                {
                    parametros = mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId),
                    materiales = mobjInformeComercialManager.TraerInformeMateriales(InformeComercialId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarReportes(ParamReportesIC oParam)
        {
            var model = new List<ReportesList>();

            if (oParam.ComercialIDGenerador == null)
            {
                oParam.ComercialIDGenerador = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            }
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.ListarReportes(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


