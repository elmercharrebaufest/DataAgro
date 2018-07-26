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
    public class ProveedorController : Controller
    {
        private IProveedorManager mobjProveedorManager;
        private IHomeManager mobjHomeManager;
        private ICampañaManager mobjCampañaManager;
        private string idActiveDirectory;
        private IComercialManager mobComercialManager;
        private IReportesManager mobjreportesManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ProveedorController(IMSContextProvider oMSContextProvider, IProveedorManager oProveedorManager, IHomeManager oHomeManager, ICampañaManager oCampañaManager, IComercialManager oComercialManager, IReportesManager reportesManager)
        {
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjProveedorManager = oProveedorManager;
            mobjHomeManager = oHomeManager;
            mobjCampañaManager = oCampañaManager;
            mobComercialManager = oComercialManager;
            mobjreportesManager = reportesManager;
        }


        // GET: Contactos
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult ReporteProveedor()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            return View();
        }
        

        public ActionResult Agregar(int? ProveedorId)
        {
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.ProveedorId = ProveedorId;
                return View();
            }            

        }

        // GET: Contactos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contactos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contactos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Contactos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contactos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Contactos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Contactos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Detalle(int ProveedorId, bool? Agenda)
        {
            string ActionView  = "";
            bool mostrarEditar = true;
            

            if (!mobComercialManager.ComercialExiste(idActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                mostrarEditar = false;
                ViewBag.edita = false;
            }
            else if (!mobComercialManager.ComercialPerteneceProveedor(idActiveDirectory, ProveedorId))
            {
                ActionView = "ErrorUsuarioSinDerechos";
            }


            ViewBag.MostrarEditar = mostrarEditar;
            ViewBag.MostrarAgenda = Agenda;
            ViewBag.ProveedorId = ProveedorId;
            return View(ActionView);
        }

        public async Task<ActionResult> TraerProveedor(int ProveedorId)
        {
            var model = await mobjProveedorManager.TraerProveedor(ProveedorId, idActiveDirectory, GlobalVariables.Equipo);      

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> Iniciliazar(int ProveedorId)
        {
            var model =  await mobjProveedorManager.TraerDatosCombo(ProveedorId);
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> TraerLocalidad(int Id)
        {
            var model = await mobjProveedorManager.TraerLocalidad(Id);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> CrearActividad(ActividadInsetarIni oParam)
        {
            var model = new Actividad();
            
            oParam.ComercialId = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            oParam.UserName = Core.Util.GetNameUser();

            await mobjProveedorManager.GrabarRecordatorioAsync(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> EliminarRecordatorio(int id)
        {
            var model = new Actividad();

            await mobjProveedorManager.EliminarRecordatorio(id);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerContacto(int ProveedorId)
        {
            var model = new List<ContactoComercial>();

            model = await mobjProveedorManager.TraerContacto(ProveedorId);
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerRazonSocial(string cuit)
        {
            var model = await mobjProveedorManager.TraerRazonSocial(cuit);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> GrabarProveedor(NuevoProveedor oParam)
        {

             GrabarProveedorResult model = new GrabarProveedorResult();

            //EntityErrors model = null;

            if (oParam.ProveedorId != null && oParam.ProveedorId != 0)
            {
                 model = await mobjProveedorManager.UpdateProveedor(oParam, idActiveDirectory);
            }
            else { 
                 model = await mobjProveedorManager.GrabarNuevoProveedor(oParam, idActiveDirectory);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerCampañasActivas()
        {
            var model = await mobjCampañaManager.TraerCampañasActivas();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerMaterialPorCampaña(int CampañaId)
        {
            var model = await mobjCampañaManager.TraerMaterialPorCampaña(CampañaId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerCampañaPorMaterial(int MaterialId)
        {
            var model = await mobjCampañaManager.TraerCampañaPorMaterial(MaterialId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerFiltros(string TipoActividadId, int ProveedorId, HistorialActiviad oParam )
        {
            var model = await mobjProveedorManager.TraerHistorialActividad(oParam, ProveedorId,  TipoActividadId);
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> ImprimirReporteProveedor(int ProveedorId)
        {
            var model = new ReportesModel();

            var datos = await mobjProveedorManager.TraerProveedor(ProveedorId, idActiveDirectory, GlobalVariables.Equipo);

            var oLstProveedor = new LstProveedor(mobjreportesManager);

            var identif = await oLstProveedor.GenerarListadoAsync(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerCampañasPorGrano(int MaterialId)
        {
            var model = await mobjCampañaManager.TraerCampañasPorGrano(MaterialId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ObtenerReporteProveedor(string Valor)
        {

            var model = await mobjProveedorManager.ObtenerReporteProveedor(Valor, idActiveDirectory);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> BuscarProveedores(string filtro)
        {
            var model = await mobjProveedorManager.DevolverProveedores(filtro);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }       

    }
}
