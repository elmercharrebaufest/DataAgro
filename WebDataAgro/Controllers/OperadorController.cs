using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class OperadorController : Controller
    {
        private IOperadorManager mobjOperadorManager;
        private IComercialManager mobjComercialManager;

        public OperadorController(IOperadorManager oOperadorManager, IComercialManager oComercialManager)
        {
            mobjOperadorManager = oOperadorManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
            else if (!mobjComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory) || GlobalVariables.Perfil != EnumPerfil.Mesa)
            {
                ActionView = "ErrorDePermisos";
            }
            return View(ActionView);
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