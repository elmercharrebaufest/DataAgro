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
    public class CentroController : Controller
    {
        private ICentroManager mobjCentroManager;
        private IComercialManager mobjComercialManager;

        public CentroController(ICentroManager oCentroManager, IComercialManager oComercialManager)
        {
            mobjCentroManager = oCentroManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
            else if (!mobjComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }
            return View(ActionView);
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmCentroModel
                {
                    Datos = mobjCentroManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniCentroModel();

            var result = mobjCentroManager.TraerTodoCentro();

            if (result != null)
            {
                model.Datos = result.Centro;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CentroCombo(AbmCentroParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmCentro
                {
                    Centro = mobjCentroManager.TraerCentro(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmCentroParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmCentroResult
                {
                    Centro = mobjCentroManager.TraerCentro(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Centro oCentro)
        {
            var model = new AbmCentroResult();

            var entityErrors = mobjCentroManager.GrabarCentro(oCentro);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.Centro = new CentroDto { CodigoSap= oCentro.CodigoSap, Descripcion = oCentro.Descripcion, Id = oCentro.Id, Acopio = oCentro.Acopio};
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmCentroParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjCentroManager.EliminarCentro(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmCentroResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}