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
    public class RangoPrecioController : Controller
    {
        private IRangoManager mobjRangoManager;
        private IComercialManager mobjComercialManager;

        public RangoPrecioController(IComercialManager oComercialManager, IRangoManager oRangoManager)
        {
            mobjRangoManager = oRangoManager;
            mobjComercialManager = oComercialManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionRangosPrecios)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmRangoModel
                {
                    Datos = mobjRangoManager.TraerDatosIniciales(),
                    RangoPrecio = new RangoPrecio()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniRangoModel();

            var result = mobjRangoManager.TraerTodoRango();

            if (result != null)
            {
                model.Datos = result.Rango;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult RangoCombo(AbmRangoParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmRango
                {
                    Rango = mobjRangoManager.TraerRango(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmRangoParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmRangoResult
                {
                    Rango = mobjRangoManager.TraerRango(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(RangoPrecio oRango)
        {
            var model = new AbmRangoResult();

            var entityErrors = mobjRangoManager.GrabarRango(oRango);

            if (!entityErrors.HayErrores)
            {
                model.Rango = new RangoPrecioDto { Id = oRango.Id, PrecioMinimo = oRango.PrecioMinimo, PrecioMaximo = oRango.PrecioMaximo, MaterialId = oRango.MaterialId, MonedaId = oRango.MonedaId};
            }
            else
            {
                model.Errores = entityErrors.Errores;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmRangoParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjRangoManager.EliminarRango(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmRangoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}