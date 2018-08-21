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
        
        private IComercialManager comercialManager;
        private readonly IReportesManager reportesManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public HomeController(IComercialManager comercialManager, IReportesManager reportesManager, IHomeManager homeManager)
        {
            
            this.comercialManager = comercialManager;
            this.reportesManager = reportesManager;
            this.mobjHomeManager = homeManager;
        }

        
        public ActionResult Index()
        {
            string ActionView = "";
            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
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
            var filtro = new oParamBusqueda {
                ComercialId = GlobalVariables.ComercialId,
                Equipo = GlobalVariables.EquipoReal
            };
            var result = mobjHomeManager.TraerBusquedaContacto(filtro, 1);

            model.Campaña = mobjHomeManager.TraerInfoCampaña(GlobalVariables.ComercialId, GlobalVariables.EquipoReal);

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
                Data = mobjHomeManager.BusquedaHome(filtro, GlobalVariables.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerBusquedaContacto(oParamBusqueda filtro, int pagina)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = GlobalVariables.EquipoReal;

            var result = mobjHomeManager.TraerBusquedaContacto(filtro, pagina);

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
            var datos = mobjHomeManager.ExportarAll(filtro, GlobalVariables.IdActiveDirectory);

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

    } 
 
}