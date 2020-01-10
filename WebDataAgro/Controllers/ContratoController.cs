using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ContratoController : Controller
    {
        private readonly IContratoManager mobjContratoManager;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly IProvinciaManager mobjProvinciaManager;
        private readonly ILocalidadManager mobjLocalidadManager;
        private readonly IMaterialManager mobjMaterialManager;
        private readonly ITipoNegocioManager mobjTipoNegocioManager;
        private readonly ICampañaManager mobjCampaniaManager;
        private readonly ICentroManager mobjCentroManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ContratoController(IProveedorManager oProveedorManager, IContratoManager ocontratoManager, IComercialManager oComercialManager, IProvinciaManager oProvinciaManager, ILocalidadManager oLocalidadManager,
            IMaterialManager oMaterialManager, ITipoNegocioManager oTipoNegocioManager,  ICampañaManager oCampaniaManager,  ICentroManager oCentroManager)
        {
            mobjProveedorManager = oProveedorManager;
            mobjComercialManager = oComercialManager;
            mobjContratoManager = ocontratoManager;
            mobjProvinciaManager = oProvinciaManager;
            mobjLocalidadManager = oLocalidadManager;
            mobjMaterialManager = oMaterialManager;
            mobjTipoNegocioManager = oTipoNegocioManager;
            mobjCampaniaManager = oCampaniaManager;
            mobjCentroManager = oCentroManager;            
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteCompraNet)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(FiltroReporteNegocioDto filtro)
        {
            var filtrarReporte = new FiltroReporteNegocioDto();
            if (!filtrarReporte.Equals(filtro))
            {
                filtro.FechaCarga = DateTime.Now.Date.ToString("dd-MM-yyyy");
            }
            var equipo = GlobalVariables.EquipoReal;

            var model = mobjContratoManager.TraerContratosFiltrados(filtro, PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial), equipo, GlobalVariables.CorredoresComercial);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        
        public ActionResult ListarComercial(string text = "")
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var comerciales = mobjComercialManager.ListarComercial(text, equipo);
            //tiene que coincidir ComercialId y Comercial con los campos configurados en el js linea 291
            return Json(comerciales.Select(x => new { x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProvincia(string text = "")
        {
            var provincias = mobjProvinciaManager.ListarProvincia(text);
            return Json(provincias.Select(x => new { x.ProvinciaId, Provincia = x.Nombre }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLocalidad(string text = "")
        {
            var localidades = mobjLocalidadManager.ListarLocalidad(text);
            return Json(localidades.Select(x => new { x.LocalidadId, Localidad = x.Nombre }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = mobjProveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarCorredor(string text = "")
        {
            var corredores = mobjProveedorManager.ListarCorredor(text);
            return Json(corredores.Select(x => new { CorredorId = x.ProveedorId, Corredor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        private void FillViewBag()
        {

            var material = mobjMaterialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var tipoNegocio = mobjTipoNegocioManager.TraerTodoTipoNegocio();
            var tipoNegocioListItems = tipoNegocio.Select(

                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.TipoNegocioId.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = tipoNegocioListItems;

            var campania = mobjCampaniaManager.TraerTodoCampania().Where(x => x.CampañaId >= 6).ToList();
            var campaniaListItems = campania.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.CampañaId.ToString(),
                Selected = false
            }).OrderBy(x => x.Value);
            ViewBag.Campania = campaniaListItems;

            var zona = mobjContratoManager.TraerTodoGrupoDeCompras();
            var zonaListItems = zona.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.Id.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value);
            ViewBag.Zona = zonaListItems;

            var estado = mobjContratoManager.TraerTodoLosEstados().Where(x => x.EstadoContratoId != 3);
            var estadoListItems = estado.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.EstadoContratoId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Estado = estadoListItems;

            var centro = mobjCentroManager.TraerTodoCentro();
            var centroListItems = centro.Centro.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;

            var boleto = mobjContratoManager.TraerTodosLosBoletos();
            var boletoListItems = boleto.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Boleto = boletoListItems;

            var comercial = mobjComercialManager.TraerTodoComercial();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Nombres+" "+ x.Apellido,
                   Value = x.ComercialId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Comercial = comercialListItems;
        }
    
    }
}
