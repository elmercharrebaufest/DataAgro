using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class HomeController : Controller
    {
        private IHomeManager mobjHomeManager;
        private readonly IObjetivoManager objetivoManager;
        private IComercialManager comercialManager;
        private readonly IReportesManager reportesManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public HomeController(IComercialManager comercialManager, IReportesManager reportesManager, IHomeManager homeManager, IObjetivoManager objetivoManager)
        {

            this.comercialManager = comercialManager;
            this.reportesManager = reportesManager;
            this.mobjHomeManager = homeManager;
            this.objetivoManager = objetivoManager;
        }


        public ActionResult Index()
        {
            string ActionView = "";

            if (GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            if (!comercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }
            else if (GlobalVariables.EsAdministrador)
            {

                ViewBag.esadmin = true;
            }

            return View(ActionView);
        }


        public ActionResult ErrorDePermisos()
        {

            return View("ErrorDePermisos");
        }

        public ActionResult ErrorUsuarioSinDerechos()
        {

            return View("ErrorUsuarioSinDerechos");
        }

        public ActionResult Inicializar()
        {
            var model = new ResultIniContactoModel();
            var filtro = new oParamBusqueda
            {
                ComercialId = GlobalVariables.ComercialId,
                Equipo = GlobalVariables.EquipoReal
            };
            ResultIniContacto result;
            if (GlobalVariables.Perfil != EnumPerfil.CorredoresComercial)
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, new List<int>());
            }
            else
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, GlobalVariables.Equipo);
            }

            model.Campaña = mobjHomeManager.TraerInfoCampaña(GlobalVariables.ComercialId, GlobalVariables.EquipoReal);
            model.Objetivo = mobjHomeManager.TraerInfoObjetivo(GlobalVariables.ComercialId, GlobalVariables.EquipoReal);
            model.Datos = mobjHomeManager.TraerInfoIniciales(GlobalVariables.EquipoReal);

            if (result != null)
            {
                model.Contactos = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BusquedaHome(string filtro)
        {
            return new JsonResult()
            {
                Data = mobjHomeManager.BusquedaHome(filtro, GlobalVariables.ComercialId, GlobalVariables.Equipo, GlobalVariables.CorredoresComercial, (int)GlobalVariables.Perfil),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerBusquedaContacto(oParamBusqueda filtro, int pagina)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;

            ResultIniContacto result;
            if (GlobalVariables.Perfil != EnumPerfil.CorredoresComercial)
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, new List<int>());
            }
            else
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, GlobalVariables.CorredoresComercial);
            }

            if (result != null)
            {
                model.Contactos = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerActividadesPorComercialId()
        {
            var model = new ResultActividadesModel();

            var result = mobjHomeManager.TraerActividadesPorComercialId(GlobalVariables.ComercialId);

            if (result != null)
            {
                model.Actividades = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult ExportarContactosPDF(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;

            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarListado(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }


        public ActionResult ExportarContactosExcel(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;
            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        public ActionResult ExportarAll(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;
            filtro.Estado = null;
            var datos = mobjHomeManager.ExportarAll(filtro, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcelExportAll(datos, (int)GlobalVariables.Perfil);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult TraerPostIt()
        {
            var model = new ResultIniPostItModel();

            var result = mobjHomeManager.TraerTexto(GlobalVariables.ComercialId);

            if (result != null)
            {
                model.Texto = result.Texto;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult GuardarPostIt(PostIt post)
        {
            var model = new GrabarPostItResult();

            post.ComercialId = GlobalVariables.ComercialId;

            if (post.ComercialId != 0)
            {
                model = mobjHomeManager.GuardarPostIt(post);
            }


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult GuardarObjetivoComercial(ObjetivoComercial objetivo)
        {
            var model = new Resultado();

            objetivo.ComercialId = GlobalVariables.ComercialId;

            if (objetivo.ComercialId != 0)
            {
                model = objetivoManager.GuardarObjetivo(objetivo);
            }


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult TraerObjetivos()
        {
            var model = new ResultIniContactoModel();
            model.Objetivo = mobjHomeManager.TraerInfoObjetivo(GlobalVariables.ComercialId, GlobalVariables.EquipoReal);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
    }

}