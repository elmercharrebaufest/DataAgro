using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Helpers.Excel;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteEvolucionFijacionController : Controller
    {
        private readonly IProveedorManager proveedorManager;
        private readonly IReportesManager reportesManager;
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IComercialManager comercialManager;
        private readonly IContratoManager contratoManager;
        private readonly ICampañaManager campañaManager;


        public ReporteEvolucionFijacionController(IProveedorManager proveedorManager, IReportesManager reportesManager, ICentroManager centroManager, IMaterialManager materialManager, IComercialManager comercialManager, IContratoManager contratoManager, ICampañaManager campañaManager)
        {
            this.proveedorManager = proveedorManager;
            this.reportesManager = reportesManager;
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.comercialManager = comercialManager;
            this.contratoManager = contratoManager;
            this.campañaManager = campañaManager;
        }
        // GET: ResearchAvanceSiembra
        [Autorizacion(PermisosDataAgro.VisualizarReporteEvolucionFijacion)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }
        private void FillViewBag()
        {

            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var campania = campañaManager.TraerTodoCampania().Where(x => x.CampañaId >= 6).ToList();
            var campaniaListItems = campania.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.CampañaId.ToString(),
                Selected = false
            }).OrderBy(x => x.Value);
            ViewBag.Campania = campaniaListItems;

            var zona = contratoManager.TraerTodoGrupoDeCompras();
            var zonaListItems = zona.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.Id.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value);
            ViewBag.Zona = zonaListItems;



            var centro = centroManager.TraerTodoCentro();
            var centroListItems = centro.Centro.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;



            var comercial = comercialManager.TraerTodoComercial();
            comercial.Comercial = comercial.Comercial.Where(a => (a.Rol.ToUpper().Contains("Comercial".ToUpper()) || a.Rol.ToUpper().Contains("Comercial corredor".ToUpper()) || a.Rol.ToUpper().Contains("Mesa".ToUpper())) && a.Deshabilitado != true).ToList();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Nombres + " " + x.Apellido,
                   Value = x.ComercialId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Comercial = comercialListItems;

            var clasificaciones = reportesManager.TraerTodoClasificacionCompraNet();
            var clasificacionListItems = clasificaciones.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.Id.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value);
            ViewBag.Clasificacion = clasificacionListItems;

        }
        public ActionResult PartialReporte(string fecha, string fechaHasta, int? ProveedorId, int? ComercialId, int? CampanaId, int? MaterialId,
            int? GrupoCompraId, int? ClasificacionId, int? DestinoId)
        {
            ViewBag.fecha = fecha;
            ViewBag.fechaHasta = fechaHasta;
            ViewBag.DestinoId = DestinoId;
            ViewBag.MaterialId = MaterialId;
            ViewBag.ProveedorId = ProveedorId;
            ViewBag.ComercialId = ComercialId;
            ViewBag.CampanaId = CampanaId;
            ViewBag.GrupoCompraId = GrupoCompraId;
            ViewBag.ClasificacionId = ClasificacionId;

            if (fecha == null && fechaHasta == null)
            {
                fecha = DateTime.Now.AddYears(-1).ToString("dd-MM-yyyy");
                fechaHasta = DateTime.Now.AddYears(1).ToString("dd-MM-yyyy");
            }
            DateTime desde = DateTime.ParseExact(fecha ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime hasta = DateTime.ParseExact(fechaHasta ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var model = reportesManager.ObtenerDatosReporteEvolucionFijacion(desde, hasta, ProveedorId, ComercialId, CampanaId, MaterialId, GrupoCompraId, ClasificacionId, DestinoId);
            return PartialView("_Reporte", model.tablero);

        }


        public ActionResult DescargarReporte(string fecha, string fechaHasta, int? ProveedorId, int? ComercialId, int? CampanaId, int? MaterialId,
            int? GrupoCompraId, int? ClasificacionId, int? DestinoId)
        {
            if (fecha == null && fechaHasta == null)
            {
                fecha = DateTime.Now.AddYears(-1).ToString("dd-MM-yyyy");
                fechaHasta = DateTime.Now.AddYears(1).ToString("dd-MM-yyyy");
            }
            DateTime desde = DateTime.ParseExact(fecha ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime hasta = DateTime.ParseExact(fechaHasta ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var model = reportesManager.ObtenerDatosReporteEvolucionFijacion(desde, hasta, ProveedorId, ComercialId, CampanaId, MaterialId, GrupoCompraId, ClasificacionId, DestinoId);

            return File(ExcelReporteCompleto.GenerarExcelReporteEvolucionFijacion(model), "application/vnd.ms-excel");            
        }

        //public JsonResult ObtenerCentros()
        //{
        //    return Json(JsonConvert.SerializeObject(new { data = centroManager.TraerTodoCentro().Centro }), JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ObtenerMateriales()
        //{
        //    List<MaterialCombo> materiales = materialManager.TraerDatosIniciales().Material;
        //    return Json(JsonConvert.SerializeObject(new { data = materiales }), JsonRequestBehavior.AllowGet);

        //}
    }
}


