using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Business;
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
using Molinos.DataAgro.Mapping.Context;
using Mastersoft.Framework.Interfaces;
using System.Web.Helpers;
using KendoGridBinder.ModelBinder.Mvc;

namespace WebDataAgro.Controllers {
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

        private string idActiveDirectory;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CompraNetController(IMSContextProvider oMSContextProvider, IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager, IProveedorManager oProveedorManager, IMaterialManager oMaterialManager, IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager) {


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

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index() {

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);

            ViewBag.perfil = "";

            if (mobcomercialmanager.EsPerfilMesa(idActiveDirectory))
            {
                ViewBag.perfil = "Mesa";
            }

            if (mobcomercialmanager.EsPerfilComercial(idActiveDirectory))
            {
                ViewBag.perfil = "Comercial";
            }

            if (mobcomercialmanager.EsPerfilJefe(idActiveDirectory)) {
                ViewBag.perfil = "Jefe";
            }


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

            var ActiveDirectory = Util.GetIdActiveDirectory();
            model.Datos = await mobjCompraNetManager.TraerDatosInicialesAsync(ActiveDirectory);
            
            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> InicializarContrato() {
            var model = new ContratoModel_prueba();

            var ActiveDirectory = Util.GetIdActiveDirectory();

            model.Datos = await mobjContratoManager.TraerDatosCombo();

            
            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> InicializarFijacion() {
            var model = new FijacionDePrecioContratoModel();

            var ActiveDirectory = Util.GetIdActiveDirectory();

            model.Datos = await mobjFijacionDePrecioContratoManager.TraerDatosInicialesAsync();

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarContrato(Contrato oParam) {

            GrabarContratoResult model = new GrabarContratoResult();
            
            if (oParam.Base == null) oParam.Base = false;
            if (oParam.NoInformaSio == null) oParam.NoInformaSio = false;
            if (oParam.TrigoEspecial == null) oParam.TrigoEspecial = false;

            Comercial comercial = await mobjComercialManager.TraerComercialAsync(Convert.ToInt32(oParam.ComercialId));

            oParam.GrupoCompra = Convert.ToInt32(comercial.GrupoDeCompras);

            oParam.UsuarioId = idActiveDirectory;

            model = await mobjContratoManager.GrabarContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> FinalizarContrato(Contrato oParam)
        {

            GrabarContratoResult model = new GrabarContratoResult();
            
            model = await mobjContratoManager.FinalizarContrato(oParam, idActiveDirectory);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ConfirmarContrato(Contrato oParam) {
            GrabarContratoResult model = new GrabarContratoResult();
            
            model = await mobjContratoManager.ConfirmarContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> BorrarContrato(Contrato oParam)
        {

            GrabarContratoResult model = new GrabarContratoResult();
            model = await mobjContratoManager.BorrarContrato(oParam);

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

        public async Task<ActionResult> ReenviarMails(Contrato oParam)
        {
            GrabarContratoResult model = new GrabarContratoResult();
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarAmpliacionContrato(Contrato oParam)
        {
            GrabarContratoResult model = new GrabarContratoResult();

            model = await mobjContratoManager.GrabarAmpliacionContrato(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarAmpliacionFijacion(FijacionDePrecioContrato oParam) {
            GrabarContratoResult model = new GrabarContratoResult();

            model = await mobjFijacionDePrecioContratoManager.GrabarAmpliacionFijacion(oParam);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GrabarFijacion(FijacionDePrecioContrato oParam)
        {
            GrabarFijacionResult model = new GrabarFijacionResult();

            //var entityErrors = await mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecioAsync(oParam, idActiveDirectory);

            model = await mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecioAsync(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> TraerContrato(string contratoId) {

            var model = new Contrato();

            model = await mobjContratoManager.TraerContratoAsync(Convert.ToInt32(contratoId));

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        [HttpPost]
        public async Task<ActionResult> BuscaDatosTabla(KendoGridMvcRequest request)
        {
            List<ComercialQry> listComercial = await RecuperaEquipo(idActiveDirectory);
           
            var model = mobjContratoManager.TraerTodosContratos(request, listComercial);

            return Json(model);
        }

        private async Task<List<ComercialQry>> RecuperaEquipo(string idActiveDirectory) {
            
            int ComercialId = await mobjContratoManager.ObtenerComercialId(idActiveDirectory);

            IComercialManager mObjcomercialmanager = new ComercialManager();

            mObjcomercialmanager.Inicializar(mobjMSContext);

            List<ComercialQry> listComercial = new List<ComercialQry>();

            ComercialQry comercial = new ComercialQry();

            List<ComercialQry> listComercialAux = await mobjContratoManager.TraerComerciales();

            if (mObjcomercialmanager.EsPerfilComercial(idActiveDirectory))
            {
                comercial.ComercialId = ComercialId;

                listComercial.Add(comercial);
            }
            else if (mObjcomercialmanager.EsPerfilJefe(idActiveDirectory))
            {
                listComercialAux.RemoveAll(x => ComercialId != x.EmpleadorACargo);
                foreach (ComercialQry comerciallista in listComercialAux)
                {
                    listComercial.Add(comerciallista);
                }
                comercial.ComercialId = ComercialId;

                listComercial.Add(comercial);
            }
            else if(mObjcomercialmanager.EsPerfilMesa(idActiveDirectory))
            {
                listComercial = listComercialAux;
            }

            return listComercial;
        }

        public async Task<ActionResult> TraerCampanaPorMaterial(int? MaterialId) {

            if (MaterialId == null) MaterialId = 0;

            var model = await mobjCampañaManager.TraerCampañaPorMaterial(Convert.ToInt32(MaterialId));

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public async Task<ActionResult> TraerLocalidadPorProvincia(int? ProvinciaId) {

            if (ProvinciaId == null) ProvinciaId = 0;

            var model = await mobjLocalidadManager.TraerLocalidadPorProvincia(ProvinciaId.Value);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<int?> TraerCampanaActualMaterial(int MaterialId) {

            Material material = await mobjMaterialManager.TraerMaterialAsync(MaterialId);
            
            return material.CampañaId;

        }

        public async Task<ActionResult> ObtenerComercialId() {

            var ComercialId = mobjContratoManager.ObtenerComercialId(idActiveDirectory);

            return new JsonResult() {
                Data = ComercialId,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<int> ObtenerProveedorId (string Cuit) {

            int proveedorId = 0;

            ProveedorQry oProveedor = await mobjProveedorManager.TraerProveedorPorCuit(Cuit);

            proveedorId = oProveedor.ProveedorId;

            return proveedorId;
        }

        public async Task<ActionResult> ObtenerProvinciaLocalidad (string Cuit) {

            var model = new DatosLocalidadProvincia();

            model = await mobjProveedorManager.TraerLocalidadProveedorPorCuitAsync(Cuit);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            }; 
        }

        public async Task<ActionResult> ObtenerProvinciaLocalidadProv(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {

            var model = new DatosLocalidadProvincia();

            model = await mobjProveedorManager.TraerLocalidadProveedorPorCuitAsync(oDatosLocalidadProvinciaFiltro);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> BuscarCuitPorId(int? ProveedorId) {

            var model = new Proveedor();

            model = await mobjProveedorManager.TraerProveedor(ProveedorId);

            return new JsonResult() {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}