using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ReportePesificadosController : Controller
    {
        private readonly IProveedorManager proveedorManager;
        private readonly IReportesManager reportesManager;
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IComercialManager comercialManager;
        private readonly IContratoManager contratoManager;
        private readonly ICampañaManager campañaManager;
        private readonly ILogger logger;

        public ReportePesificadosController(IProveedorManager proveedorManager, IReportesManager reportesManager, ICentroManager centroManager,
            IMaterialManager materialManager, IComercialManager comercialManager, IContratoManager contratoManager, 
            ICampañaManager campañaManager, ILogger logger)
        {
            this.proveedorManager = proveedorManager;
            this.reportesManager = reportesManager;
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.comercialManager = comercialManager;
            this.contratoManager = contratoManager;
            this.campañaManager = campañaManager;
            this.logger = logger;
        }

        // GET: ReportePesificados
        public ActionResult Index()
        {
            CargarView();
            return View();
        }

        private void CargarView()
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

        }
        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = proveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { x.CUIT, Proveedor = !string.IsNullOrEmpty(x.Alias) ? (x.Alias + " - " + x.RazonSocial) : x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarCorredor(string text = "")
        {
            var corredores = proveedorManager.ListarCorredor(text);
            return Json(corredores.Select(x => new { x.CUIT, Proveedor = !string.IsNullOrEmpty(x.Alias) ? (x.Alias + " - " + x.RazonSocial) : x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field= "MaterialDesc", Dir="desc" }
                };
            }

            var equipo = GlobalVariables.EquipoReal;
            var model =  reportesManager.TraerTodoDatoPesificado(filtro, equipo);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}