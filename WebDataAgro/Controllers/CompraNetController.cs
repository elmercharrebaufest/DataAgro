using Mastersoft.Framework.DataRepository;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Common.Enums;
using Autofac.Extras.NLog;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class CompraNetController : Controller {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------
         
        private MSContext mobjMSContext;

        private IHomeManager mobjHomeManager;

        private ICompraNetManager mobjCompraNetManager;

        private IContratoManager mobjContratoManager;

        private IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;

        private IComercialManager mobjComercialManager;

        private ICampañaManager mobjCampañaManager;

        private ILocalidadManager mobjLocalidadManager;

        private IMaterialManager mobjMaterialManager;

        private IProveedorManager mobjProveedorManager;

        private ILogger mobjLogger;

        private string idActiveDirectory;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CompraNetController(IMSContextProvider oMSContextProvider, IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager, IProveedorManager oProveedorManager, IMaterialManager oMaterialManager, IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager, ILogger oLogger) {


            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjHomeManager = oHomeManager;

            mobjHomeManager.Inicializar(mobjMSContext);

            mobjComercialManager = oComercialManager;

            mobjComercialManager.Inicializar(mobjMSContext);

            mobjCompraNetManager = oCompraNetManager;

            mobjCompraNetManager.Inicializar(mobjMSContext);

            mobjContratoManager = oContratoManager;

            mobjContratoManager.Inicializar(mobjMSContext);

            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;

            mobjFijacionDePrecioContratoManager.Inicializar(mobjMSContext);

            mobjCampañaManager = oCampañaManager;

            mobjCampañaManager.Inicializar(mobjMSContext);

            mobjMaterialManager = oMaterialManager;

            mobjMaterialManager.Inicializar(mobjMSContext);

            mobjProveedorManager = oProveedorManager;

            mobjProveedorManager.Inicializar(mobjMSContext);

            mobjLocalidadManager = ojLocalidadManager;

            mobjLocalidadManager.Inicializar(mobjMSContext);

            mobjLogger = oLogger;
            
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index() {
            ViewBag.perfil = GlobalVariables.Perfil.DisplayEnum();
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            mobjLogger.Debug("Hola");
            return View();

        }

        public ActionResult CrearContrato() {

            return View();
        }

        public ActionResult CrearFijacion() {

            return View();
        }

        public async Task<ActionResult> Inicializar() {
            var model = new DatosIniCompraNetModel();

            var activeDirectory = Util.GetIdActiveDirectory();
            model.Datos = await mobjCompraNetManager.TraerDatosInicialesAsync(activeDirectory);
            
            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> InicializarContrato() {
            var model = new ContratoModel_prueba();

            model.Datos = await mobjContratoManager.TraerDatosCombo();
            
            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> InicializarFijacion() {
            var model = new FijacionDePrecioContratoModel();
            
            model.Datos = await mobjFijacionDePrecioContratoManager.TraerDatosInicialesAsync();

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarContrato(Contrato oParam)
        {
            if (oParam.Base == null) oParam.Base = false;
            if (oParam.NoInformaSio == null) oParam.NoInformaSio = false;
            if (oParam.TrigoEspecial == null) oParam.TrigoEspecial = false;

            var comercial = await mobjComercialManager.TraerComercialAsync(oParam.ComercialId.Value);
            oParam.GrupoCompra = comercial.GrupoDeCompras.HasValue ? comercial.GrupoDeCompras.Value : 0;
            oParam.UsuarioId = idActiveDirectory;

            var model = await mobjContratoManager.GrabarContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> FinalizarContrato(Contrato oParam)
        {
            var model = await mobjContratoManager.FinalizarContrato(oParam, idActiveDirectory);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ConfirmarContrato(Contrato oParam)
        {         
            var model = await mobjContratoManager.ConfirmarContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> BorrarContrato(Contrato oParam)
        {
            var model = await mobjContratoManager.BorrarContrato(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> ConfirmarFijacion(FijacionDePrecioContrato oParam) {
            GrabarFijacionResult model = new GrabarFijacionResult();

            model = await mobjFijacionDePrecioContratoManager.ConfirmarFijacion(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> FinalizarFijacion(FijacionDePrecioContrato oParam) {

            GrabarFijacionResult model = new GrabarFijacionResult();

            model = await mobjFijacionDePrecioContratoManager.FinalizarFijacion(oParam, idActiveDirectory);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ReenviarMails(Contrato oParam)
        {
            var model = new GrabarContratoResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarAmpliacionContrato(Contrato oParam)
        {
            var model = await mobjContratoManager.GrabarAmpliacionContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oParam)
        {
            var model = await mobjFijacionDePrecioContratoManager.GrabarAmpliacionFijacion(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarFijacion(FijacionDePrecioContrato oParam)
        {
            var model = await mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecioAsync(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerContrato(string contratoId)
        {
            var model = await mobjContratoManager.TraerContratoAsync(Convert.ToInt32(contratoId));

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            var model = mobjContratoManager.TraerTodosContratos(request, GlobalVariables.Equipo);

            return Json(model);
        }

        public async Task<ActionResult> TraerCampanaPorMaterial(int? MaterialId)
        {
            if (MaterialId == null) MaterialId = 0;

            var model = await mobjCampañaManager.TraerCampañaPorMaterial(Convert.ToInt32(MaterialId));

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public async Task<ActionResult> TraerLocalidadPorProvincia(int? ProvinciaId)
        {
            if (ProvinciaId == null) ProvinciaId = 0;

            var model = await mobjLocalidadManager.TraerLocalidadPorProvincia(ProvinciaId.Value);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<int?> TraerCampanaActualMaterial(int MaterialId)
        {
            var material = await mobjMaterialManager.TraerMaterialAsync(MaterialId);
            
            return material.CampañaId;
        }

        public ActionResult ObtenerComercialId()
        {
            var ComercialId = mobjContratoManager.ObtenerComercialId(idActiveDirectory);

            return new JsonResult()
            {
                Data = ComercialId,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<int> ObtenerProveedorId (string Cuit)
        {          
            return (await mobjProveedorManager.TraerProveedorPorCuit(Cuit)).ProveedorId;
        }

        public async Task<ActionResult> ObtenerProvinciaLocalidad (string Cuit)
        {
            var model = await mobjProveedorManager.TraerLocalidadProveedorPorCuitAsync(Cuit);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            }; 
        }

        public async Task<ActionResult> ObtenerProvinciaLocalidadProv(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {
            var model = await mobjProveedorManager.TraerLocalidadProveedorPorCuitAsync(oDatosLocalidadProvinciaFiltro);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> BuscarCuitPorId(int? ProveedorId)
        {
            var model = await mobjProveedorManager.TraerProveedor(ProveedorId);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}