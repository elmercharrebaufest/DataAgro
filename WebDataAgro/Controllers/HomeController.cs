using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;

using WebDataAgro.Core;
using WebDataAgro.Models;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using System.Security.Principal;
using Molinos.DataAgro.Report;
using System.Configuration;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Business;
using static WebDataAgro.MvcApplication;
using Molinos.DataAgro.Entities.Common.Enums;

namespace WebDataAgro.Controllers
{
    public class HomeController : Controller
    {

        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private IHomeManager mobjHomeManager;

        private ICampañaManager mobCampañaManager;

        private IEstadoProveedorManager mobEstadoProveedorManager;

        private ICubProveedoresManager mobCuboProveedorManager;

        



        private string idActiveDirectory;
        private object mobcomercialmanager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public HomeController(IMSContextProvider oMSContextProvider, IHomeManager oHomeManager, ICampañaManager mobCampañaManager, IEstadoProveedorManager mobEstadoProveedorManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjHomeManager = oHomeManager;

            mobCampañaManager.Inicializar(mobjMSContext);

            mobEstadoProveedorManager.Inicializar(mobjMSContext);

            mobjHomeManager.Inicializar(mobjMSContext, mobCampañaManager, mobEstadoProveedorManager);            
        }

        
        public ActionResult Index()
        {
            string ActionView = "";
            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);

            if (!mobcomercialmanager.ComercialExiste(idActiveDirectory))
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

            var oLstContacto = new LstContacto(mobjMSContext);

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

            var oLstContacto = new LstContacto(mobjMSContext);

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

            var oLstContacto = new LstContacto(mobjMSContext);

            var identif = await oLstContacto.GenerarExcelExportAllAsync(datos,Util.ObtenerPerfilDeUsuario(idActiveDirectory));

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }


        public async Task<ActionResult> ValidarComercial()
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