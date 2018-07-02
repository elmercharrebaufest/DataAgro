using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
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
        private string idActiveDirectory;
        private IComercialManager comercialManager;
        private readonly IReportesManager reportesManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public HomeController(IMSContextProvider oMSContextProvider, IComercialManager comercialManager, IReportesManager reportesManager, IHomeManager homeManager)
        {
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
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
            
            if (!comercialManager.ComercialExiste(idActiveDirectory))
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







        public async Task<ActionResult> Inicializar()
        {
            var model = new ResultIniContactoModel();

            int idComercial = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var result = await mobjHomeManager.TraerTodoContactoAsync(idComercial);

            model.Campaña = await mobjHomeManager.TraerInfoCampañaAsync(idComercial);

            model.Datos = await mobjHomeManager.TraerInfoInicialesAsync(idComercial);
            
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

        public async Task<ActionResult> BusquedaHome(string filtro)
        {
            var model = new List<BusquedaHome>();

           int idComercial = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            model = await mobjHomeManager.BusquedaHome(filtro, idComercial);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> TraerBusquedaContacto(oParamBusqueda filtro)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var result = await mobjHomeManager.TraerBusquedaContactoAsync(filtro);

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

        public async Task<ActionResult> TraerActividadesPorComercialId()
        {
            var model = new ResultActividadesModel();

            var ComercialId = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var result = await mobjHomeManager.TraerActividadesPorComercialId(ComercialId);

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

        public async Task<ActionResult> ExportarContactosPDF(string Ids)
        {
            
            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add( (Convert.ToInt32(x))));

            var model = new ReportesModel();
            
            var datos = await mobjHomeManager.ExportarContactos(ides,idActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = await oLstContacto.GenerarListadoAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);
            
            return Json(model);
        }


        public async Task<ActionResult> ExportarContactosExcel(string Ids)
        {

            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add((Convert.ToInt32(x))));

            var model = new ReportesModel();

            var datos = await mobjHomeManager.ExportarContactos(ides,idActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = await oLstContacto.GenerarExcelAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        public async Task<ActionResult> ExportarAll(string Ids)
        {

            List<int> ides = new List<int>();

            Ids.Split(',').ToList().ForEach(x => ides.Add((Convert.ToInt32(x))));

            var model = new ReportesModel();

            var datos = await mobjHomeManager.ExportarAll(ides, idActiveDirectory);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = await oLstContacto.GenerarExcelExportAllAsync(datos,Util.ObtenerPerfilDeUsuario(idActiveDirectory));

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

        public async Task<ActionResult> TraerPostIt()
        {
            var model = new ResultIniPostItModel();

            int idComercial = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var result = await mobjHomeManager.TraerTextoAsync(idComercial);

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

        public async Task<ActionResult> GuardarPostItAsync(PostIt post)
        {
            var model = new GrabarPostItResult();

            int idComercial = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            post.ComercialId = idComercial;

            if (post.ComercialId  != 0)
            {
                model = await mobjHomeManager.GuardarPostItAsync(post);
            }
            

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

    } 
 
}