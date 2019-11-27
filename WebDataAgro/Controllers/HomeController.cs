using Microsoft.Web.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report;
using System;
using System.IdentityModel.Services;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class HomeController : Controller
    {
        private readonly IHomeManager mobjHomeManager;
        private readonly IObjetivoManager objetivoManager;
        private readonly IComercialManager comercialManager;
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
            return View();
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

            var result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, GlobalVariables.Equipo);

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
                Data = mobjHomeManager.BusquedaHome(filtro, GlobalVariables.ComercialId, GlobalVariables.Equipo, GlobalVariables.CorredoresComercial),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerBusquedaContacto(oParamBusqueda filtro, int pagina)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;

            ResultIniContacto result;
            if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, GlobalVariables.Equipo);
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
        [Autorizacion(PermisosDataAgro.DescargaPdf)]
        public ActionResult ExportarContactosPDF(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;

            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarListado(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        [Autorizacion(PermisosDataAgro.DescargaExcel)]
        public ActionResult ExportarContactosExcel(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;
            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }
        [Autorizacion(PermisosDataAgro.DescargaExportAllComercial, PermisosDataAgro.DescargaExportAllVisualizador)]
        public ActionResult ExportarAll(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;
            filtro.Estado = null;
            var datos = mobjHomeManager.ExportarAll(filtro, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcelExportAll(datos);

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

        public ActionResult EliminarObjetivo(int id)
        {
            var resultado = objetivoManager.EliminarObjetivo(id);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarCookie()
        {
            return View();
        }

        [AjaxOnly]
        public void BorrarCookies()
        {
            if (FederatedAuthentication.SessionAuthenticationModule != null)
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
        }
    }

}