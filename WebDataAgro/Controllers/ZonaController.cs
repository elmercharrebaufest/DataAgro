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
    public class ZonaController : Controller
    {
        private IZonaManager mobjZonaManager;
        private IComercialManager mobjComercialManager;

        public ZonaController(IZonaManager oZonaManager, IComercialManager oComercialManager)
        {
            mobjZonaManager = oZonaManager;
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
                Data = new DatosIniAbmZonaModel
                {
                    Datos = mobjZonaManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniZonaModel();

            var result = mobjZonaManager.TraerTodoZona();

            if (result != null)
            {
                model.Datos = result.Zona;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ZonaCombo(AbmZonaParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmZona
                {
                    Zona = mobjZonaManager.TraerZona(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmZonaParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmZonaResult
                {
                    Zona = mobjZonaManager.TraerZona(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Zona oZona)
        {
            var model = new AbmZonaResult();

            var entityErrors = mobjZonaManager.GrabarZona(oZona);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.Zona = new ZonaDto { CodigoSap= oZona.CodigoSap, Descripcion = oZona.Descripcion, Id = oZona.Id};
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmZonaParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjZonaManager.EliminarZona(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmZonaResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}