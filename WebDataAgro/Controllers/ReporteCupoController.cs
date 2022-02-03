using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;
using Filter = Kendo.DynamicLinq.Filter;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteCupoController : Controller
    {
        private readonly ICupoManager cupoManager;
        private readonly IMaterialManager mobjMaterialManager;
        private readonly IZonaCupoManager zonaCupoManager;
        private readonly ICentroManager mobjCentroManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly IHttpContextManager httpContextManager;
        private readonly IProveedorManager proveedormanager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ReporteCupoController(
            ICupoManager cupoManager,
            IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, ICentroManager mobjCentroManager,
            IComercialManager mobjComercialManager, IHttpContextManager httpContextManager, IProveedorManager proveedormanager)
        {
            this.cupoManager = cupoManager;
            this.mobjMaterialManager = materialManager;
            this.zonaCupoManager = zonaCupoManager;
            this.mobjCentroManager = mobjCentroManager;
            this.mobjComercialManager = mobjComercialManager;
            this.httpContextManager = httpContextManager;
            this.proveedormanager = proveedormanager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteCupo)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }


        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort> {
                    new Sort {Field= "FechaIngreso", Dir="desc" },
                    new Sort {Field="Material", Dir="desc" } };
            }

            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosCupos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            return Json(model);
        }
        public JsonResult BuscarProveedor(string text)
        {
            var proveedores = proveedormanager.DevolverProveedoresCorredores(text, null, false);
            return Json(proveedores.Select(x => new { ProveedorId = x.Id, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
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


            var listaZonas = zonaCupoManager.TraerTodoZonaCupo().ZonaCupo.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Zona = listaZonas;

            var estado = cupoManager.TraerTodoLosEstados();
            var estadoListItems = estado.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Estado = estadoListItems;

            var centros = mobjCentroManager.TraerTodoCentro().Centro;
            var centroListItems = centros.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;


            var comercial = mobjComercialManager.TraerTodoComercial();
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
        static readonly object _lockAnulacionMasiva = new object();

        public async Task<string> AnulacionMasivaAsync(List<int> equipo, DataSourceResult cupos, string path)
        {
          
            var comercialId = GlobalVariables.IdActiveDirectory;
            await Task.Run(() =>
            {
                //Thread.Sleep(5000);
                cupoManager.AnulacionMasiva(equipo, comercialId, cupos, path);

            });
            return "";
        }

        public ActionResult AnulacionMasiva(Filter filter)
        {
            var request = new DataSourceRequest { Filter = filter };
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosCupos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            var path = httpContextManager.ObtenerPathLogoMail();
            if (model.Total == 0)
            {
                return Json("Ningún cupo para anular");
            }
            var a = AnulacionMasivaAsync(equipo, model, path);           
            return Json("Estamos procesando tu solicitud, en breve te enviaremos un mail.");
        }
    }
}
