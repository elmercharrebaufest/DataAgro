using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ZonaController : Controller
    {
        private IZonaManager mobjZonaManager;

        public ZonaController(IZonaManager oZonaManager)
        {
            mobjZonaManager = oZonaManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionZonaGirasol)]
        public ActionResult Index()
        {
            return View();
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