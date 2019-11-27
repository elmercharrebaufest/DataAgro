using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class RolController : Controller
    {
       private readonly IRolManager oRolManager; 
       public RolController(IRolManager oRolManager)
        {
            this.oRolManager = oRolManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionRolesPermisos)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }
        private void FillViewBag()
        {
            ViewBag.Permisos = Enum.GetValues(typeof(PermisosDataAgro)).Cast<PermisosDataAgro>()
                .Select(d => new SelectListItem { Text = d.DisplayEnum(), Value = ((int)d).ToString(CultureInfo.InvariantCulture) }).OrderBy(x=>x.Value).ToList();
        }
        public ActionResult Crear()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CrearModificarPost(RolDto dto)
        {
            
            var result = oRolManager.GuardarRol(dto);

            FillViewBag();
            return Json(result,JsonRequestBehavior.AllowGet);
        }
      

        public JsonResult ObtenerPermisos(int id)
        {
            var permisos = oRolManager.ObtenerPermisos(id);
            var jsonPermisos = new List<Object>();
            foreach (var permiso in permisos)
            {
                jsonPermisos.Add(new
                {
                    Id = permiso,
                    Descripcion = permiso.DisplayEnum()
                });
            }

            return Json(jsonPermisos, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BuscaDatosTabla()
        {
            var model = oRolManager.TraerRolesPermisos();

            return Json(model);
        }

        public ActionResult Modificar(int id)
        {
            var aModificar = oRolManager.TraerRol(id);        
            return Json(aModificar,  JsonRequestBehavior.AllowGet);
        }

        public ActionResult Eliminar (int id)
        {
            var result = oRolManager.EliminarRol(id);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}