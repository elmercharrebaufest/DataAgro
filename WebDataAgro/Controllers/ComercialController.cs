using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ComercialController : Controller
    {
        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ComercialController(IComercialManager oComercialManager)
        {
            mobjComercialManager = oComercialManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            } else if (!mobjComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            return View(ActionView);
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
            return new JsonResult()
            {
                Data = new AbmComercialCrearResult
                {
                    Comercial = mobjComercialManager.TraerComercial(oParam.ComercialId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Comercial oComercial)
        {
            var model = new AbmComercialResult();

            var entityErrors = mobjComercialManager.GrabarComercial(oComercial);

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


