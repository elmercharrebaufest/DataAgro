using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CondicionController : Controller
    {
        private ICondicionManager mobjCondicionManager;
        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public CondicionController(ICondicionManager oCondicionManager)
        {
            mobjCondicionManager = oCondicionManager;
        }

        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

        [Autorizacion(PermisosDataAgro.ConfiguracionCondicion)]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Buscar()
        {
            var model = new ResultIniCondicionModel();

            var result = mobjCondicionManager.TraerTodoCondicion();

            if (result != null)
            {
                model.Datos = result.Condicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Aplicar(AbmCondicionParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmCondicionCrearResult
                {
                    Condicion = mobjCondicionManager.TraerCondicion(oParam.CondicionId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Grabar(Condicion oCondicion)
        {
            var model = new AbmCondicionResult();

            var entityErrors = mobjCondicionManager.GrabarCondicion(oCondicion);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.Condicion = oCondicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmCondicionParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjCondicionManager.EliminarCondicion(oParam.CondicionId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            var model = new AbmCondicionResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


