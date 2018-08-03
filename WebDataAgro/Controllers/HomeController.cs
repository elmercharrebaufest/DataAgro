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

            int idComercial = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var result = mobjHomeManager.TraerTodoContacto(idComercial);

            model.Campaña = mobjHomeManager.TraerInfoCampaña(idComercial, GlobalVariables.Equipo);

            model.Datos = mobjHomeManager.TraerInfoIniciales(GlobalVariables.Equipo);

            if (result != null)
            {
                model.Contactos = result.Contactos;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult BusquedaHome(string filtro)
        {
            int idComercial = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            
            return new JsonResult()
            {
                Data = mobjHomeManager.BusquedaHome(filtro, idComercial),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerBusquedaContacto(oParamBusqueda filtro)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var result = mobjHomeManager.TraerBusquedaContacto(filtro);

            if (result != null)
            {
                model.Contactos = result.Contactos;
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

            var comercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var result = mobjHomeManager.TraerActividadesPorComercialId(comercialId);

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

        public ActionResult ExportarContactosPDF(string Ids)
        {

            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add((Convert.ToInt32(x))));

            var model = new ReportesModel();

            var datos = mobjHomeManager.ExportarContactos(ides, GlobalVariables.IdActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarListado(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }


        public ActionResult ExportarContactosExcel(string Ids)
        {

            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add((Convert.ToInt32(x))));

            var model = new ReportesModel();

            var datos = mobjHomeManager.ExportarContactos(ides, GlobalVariables.IdActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        public ActionResult ExportarAll(string Ids)
        {

            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add((Convert.ToInt32(x))));

            var model = new ReportesModel();

            var datos = mobjHomeManager.ExportarAll(ides, GlobalVariables.IdActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcelExportAll(datos, (int)GlobalVariables.Perfil);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }


        public ActionResult ValidarComercial()
        {
            int puedeVer = 1;

            return new JsonResult()
            {
                Data = puedeVer,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult TraerPostIt()
        {
            var model = new ResultIniPostItModel();

            int idComercial = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var result = mobjHomeManager.TraerTexto(idComercial);

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

            int idComercial = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            post.ComercialId = idComercial;

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