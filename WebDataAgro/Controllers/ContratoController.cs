using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ContratoController : Controller
    {
        private IContratoManager mobjContratoManager;
        private IProveedorManager mobjProveedorManager;
        private IComercialManager mobjComercialManager;
        private IProvinciaManager mobjProvinciaManager;
        private ILocalidadManager mobjLocalidadManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ContratoController(  IProveedorManager oProveedorManager, IContratoManager ocontratoManager, IComercialManager oComercialManager, IProvinciaManager oProvinciaManager, ILocalidadManager oLocalidadManager)
        {
            mobjProveedorManager = oProveedorManager;
            mobjComercialManager = oComercialManager;
            mobjContratoManager = ocontratoManager;
            mobjProvinciaManager = oProvinciaManager;
            mobjLocalidadManager = oLocalidadManager;
        }

        public ActionResult Index()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            if (request.SortObjects != null)
            {
                request.SortObjects = request.SortObjects.Concat(new[] { new SortObject("Estado_Order", "asc") });
            }
            else
            {
                request.SortObjects = new List<SortObject> { new SortObject("Estado_Order", "asc") };
            }
            
            var model = mobjContratoManager.TraerTodosContratos(request, GlobalVariables.Equipo);

            return Json(model);
        }

        public ActionResult ListarComercial(string text = "")
        {
            var comerciales = mobjComercialManager.ListarComercial(text, GlobalVariables.Equipo);
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
    }
}
