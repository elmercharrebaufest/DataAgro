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
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro, PermisosDataAgro.ConfiguracionUsuarios)]
    public class ComercialController : Controller
    {
        private IComercialManager mobjComercialManager;
        private IRolManager mobjRolManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ComercialController(IComercialManager oComercialManager, IRolManager mobjRolManager)
        {
            mobjComercialManager = oComercialManager;
            this.mobjRolManager = mobjRolManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        [Autorizacion(PermisosDataAgro.ConfiguracionUsuarios)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmComercialModel
                {
                    Datos = mobjComercialManager.TraerDatosIniciales(),
                    Comercial = new Comercial()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniComercialModel();

            var result = mobjComercialManager.TraerTodoComercial();

            if (result != null)
            {
                model.Datos = result.Comercial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ComercialCombo(AbmComercialParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmComercial
                {
                    Comercial = mobjComercialManager.ObtenerComerciales(GlobalVariables.Equipo, oParam.ComercialId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Aplicar(AbmComercialParam oParam)
        {
            var comercial = mobjComercialManager.TraerComercial(oParam.ComercialId);
            return new JsonResult()
            {
                Data = new AbmComercialCrearResult
                {
                    Comercial = comercial
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Comercial oComercial, List<Rol> roles)
        {
            var model = new AbmComercialResult();
            
            var entityErrors = mobjComercialManager.GrabarComercial(oComercial, roles);

            model.Errores = entityErrors.Errores;
            if (model.HayError)
            {
                model.Comercial = oComercial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmComercialParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjComercialManager.EliminarComercial(oParam.ComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        private void FillViewBag()
        {

            var rol = mobjRolManager.TraerTodoRoles();
            var rolesListItems = rol.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Rol = rolesListItems;
        }
        public ActionResult Cancelar()
        {
            var model = new AbmComercialResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }       
    }
}

