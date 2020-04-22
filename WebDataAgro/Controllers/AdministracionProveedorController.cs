using Molinos.DataAgro.Entities.Dto;
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
        private readonly IComercialManager comercialManager;

        public AdministracionProveedorController(IRolManager rolManager,IProveedorManager proveedorManager, IComercialManager comercialManager)
        {
            this.rolManager = rolManager;
            this.proveedorManager = proveedorManager;
            this.comercialManager = comercialManager;
        }

        [Autorizacion(PermisosDataAgro.AdministracionProveedores)]
        public ActionResult Index()
        {
            return View( );
        }
        public ActionResult Inicializar()
        {
            var roles = rolManager.TraerTodoRoles();
            var comerciales = comercialManager.TraerTodoComercial().Comercial;
            return new JsonResult()
            {
                Data = new {roles= roles, comerciales= comerciales },
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult GrabarProveedor(int id, List<Rol> roles, List<Comercial> comerciales)
        {
            var resultado = proveedorManager.GrabarRol(id, roles, comerciales);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult TraerDatosProveedor(int id)
        {
            var roles = proveedorManager.TraerRolesProveedor(id);
            List<ComercialDto> comerciales = comercialManager.TraerComercialesProveedor(id);
            return new JsonResult()
            {
                Data = new {roles=roles,comerciales=comerciales },
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}