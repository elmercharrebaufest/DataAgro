using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class ProveedorController : Controller
    {


        private MSContext mobjMSContext;

        private IProveedorManager mobjProveedorManager;

        private IHomeManager mobjHomeManager;

        private ICampañaManager mobjCampañaManager;

        private string idActiveDirectory;
        
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ProveedorController(IMSContextProvider oMSContextProvider, IProveedorManager oProveedorManager, IHomeManager oHomeManager, ICampañaManager oCampañaManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjProveedorManager = oProveedorManager;
            mobjProveedorManager.Inicializar(mobjMSContext);
            mobjHomeManager = oHomeManager;
            mobjHomeManager.Inicializar(mobjMSContext);
            mobjCampañaManager = oCampañaManager;
            mobjCampañaManager.Inicializar(mobjMSContext);
        }


        // GET: Contactos
        public ActionResult Index()
        {            
            return View();
        }

        public ActionResult ReporteProveedor()
        {
            IComercialManager mobComercialManager = new Molinos.DataAgro.Business.ComercialManager();
            mobComercialManager.Inicializar(mobjMSContext);

            if (mobComercialManager.EsPerfilAdministrativo(idActiveDirectory) || mobComercialManager.EsPerfilVisualizador(idActiveDirectory))
            {
                ViewBag.edita = false;
            }

            return View();
        }
        

        public ActionResult Agregar(int? ProveedorId)
        {

            IComercialManager mobComercialManager = new Molinos.DataAgro.Business.ComercialManager();
            mobComercialManager.Inicializar(mobjMSContext);
            if (mobComercialManager.EsPerfilAdministrativo(idActiveDirectory) || mobComercialManager.EsPerfilVisualizador(idActiveDirectory))
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

            IComercialManager mobComercialManager = new Molinos.DataAgro.Business.ComercialManager();
            mobComercialManager.Inicializar(mobjMSContext);
            string ActionView  = "";
            bool mostrarEditar = true;
            

            if (!mobComercialManager.ComercialExiste(idActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            
            if(mobComercialManager.EsPerfilAdministrativo(idActiveDirectory) || mobComercialManager.EsPerfilVisualizador(idActiveDirectory))
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
            var model = new StoredPorProveedorResult();

            model = await mobjProveedorManager.TraerProveedor(ProveedorId, idActiveDirectory);            
            //  model = await mobjProveedorManager.TraerDatosCombo();           

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> Iniciliazar(int ProveedorId)
        {
            var model = new DatosIniProveedor();

            model = await mobjProveedorManager.TraerDatosCombo(ProveedorId);
            



            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> TraerLocalidad(int Id)
        {
            var model = new List<Localidad>();

            model = await mobjProveedorManager.TraerLocalidad(Id);

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
            var model = new ProveedorNuevo();
            
            /*   
            var a = new Compras.Z_MPRFC_DATOS_COMPRAS();

            var ParametroDeLaCompra = new Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASRequest();
            var DatosDeLaCompra= new Compras.ZMPES5130();


            var w = new Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient();

            w.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS(a);

    */

            model = await mobjProveedorManager.TraerRazonSocial(cuit);

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

            //oParam.ActiveDirectoryId = Util.GetIdActiveDirectory();

            var datos = await mobjProveedorManager.TraerProveedor(ProveedorId, idActiveDirectory);

            var oLstProveedor = new LstProveedor(mobjMSContext);

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
