using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteProveedoresController : Controller
    {
        private readonly IProveedorManager proveedorManager;

        public ReporteProveedoresController(IProveedorManager proveedorManager)
        {
            this.proveedorManager = proveedorManager;
        }
        // GET: ResearchAvanceSiembra
        [Autorizacion(PermisosDataAgro.ReporteResearch)]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BuscarDatosProveedor(DataSourceRequest request)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = proveedorManager.BuscarDatosProveedor(request,equipo);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        [HttpPost]
        public ActionResult BuscarDatosContacto(DataSourceRequest request)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = proveedorManager.BuscarDatosContacto(request, equipo);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        [HttpPost]
        public ActionResult BuscarDatosProduccion(DataSourceRequest request)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = proveedorManager.BuscarDatosProduccion(request, equipo);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        [HttpPost]
        public ActionResult BuscarDatosAlmacenamiento(DataSourceRequest request)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = proveedorManager.BuscarDatosAlmacenamiento(request, equipo);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}


