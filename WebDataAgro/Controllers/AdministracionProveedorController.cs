using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro, PermisosDataAgro.AdministracionProveedores)]
    public class AdministracionProveedorController : Controller
    {
        private readonly IRolManager rolManager;
        private readonly IProveedorManager proveedorManager;

        public AdministracionProveedorController(IRolManager rolManager,IProveedorManager proveedorManager)
        {
            this.rolManager = rolManager;
            this.proveedorManager = proveedorManager;
        }
        [Autorizacion(PermisosDataAgro.AdministracionProveedores)]
        public ActionResult Index()
        {
            return View( );
        }
        public ActionResult Inicializar()
        {
            var roles = rolManager.TraerTodoRoles();
            return new JsonResult()
            {
                Data = roles,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult GrabarRolProveedor(int id, List<Rol> roles)
        {
            var resultado = proveedorManager.GrabarRol(id, roles);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult TraerRolesProveedor(int id)
        {
            var resultado = proveedorManager.TraerRolesProveedor(id);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}