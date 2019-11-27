using Molinos.DataAgro.Entities.Dto;
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
    public class OperadorController : Controller
    {
        private IOperadorManager mobjOperadorManager;

        public OperadorController(IOperadorManager oOperadorManager)
        {
            mobjOperadorManager = oOperadorManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionOperador)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmOperadorModel
                {
                    Datos = mobjOperadorManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniOperadorModel();

            var result = mobjOperadorManager.TraerTodoOperador();

            if (result != null)
            {
                model.Datos = result.Operador;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult OperadorCombo(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmOperador
                {
                    Operador = mobjOperadorManager.TraerOperador(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmOperadorResult
                {
                    Operador = mobjOperadorManager.TraerOperador(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Operador oOperador)
        {
            var model = new AbmOperadorResult();

            var entityErrors = mobjOperadorManager.GrabarOperador(oOperador);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.Operador = new OperadorDto { Descripcion = oOperador.Descripcion, Id = oOperador.Id};
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjOperadorManager.EliminarOperador(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmOperadorResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}