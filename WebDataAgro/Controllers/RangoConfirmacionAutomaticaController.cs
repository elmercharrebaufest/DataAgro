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
    public class RangoConfirmacionAutomaticaController : Controller
    {
        private IRangoConfirmacionAutomaticaManager mobjRangoManager;
        private IComercialManager mobjComercialManager;

        public RangoConfirmacionAutomaticaController(IComercialManager oComercialManager, IRangoConfirmacionAutomaticaManager oRangoManager)
        {
            mobjRangoManager = oRangoManager;
            mobjComercialManager = oComercialManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionRangosConfirmacion)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmRangoConfirmacionAutomaticaModel
                {
                    Datos = mobjRangoManager.TraerDatosIniciales(),
                    RangoConfirmacion = new RangoConfirmacionAutomatica()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniRangoConfirmacionAutomaticaModel();

            var result = mobjRangoManager.TraerTodoRangoDisponible();

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

        public ActionResult RangoCombo(AbmRangoConfirmacionAutomaticaParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmRangoConfirmacionAutomatica
                {
                    Rango = mobjRangoManager.TraerRango(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmRangoConfirmacionAutomaticaParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmRangoConfirmacionAutomaticaResult
                {
                    Rango = mobjRangoManager.TraerRango(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(RangoConfirmacionAutomatica oRango)
        {
            var model = new AbmRangoConfirmacionAutomaticaResult();

            var entityErrors = mobjRangoManager.GrabarRangoConfirmacionAutomatica(oRango, GlobalVariables.ComercialId);

            if (!entityErrors.HayErrores)
            {
                model.Rango = new RangoConfirmacionAutomaticaDto { Id = oRango.Id, PrecioMinimo = oRango.PrecioMinimo, PrecioMaximo = oRango.PrecioMaximo, MaterialId = oRango.MaterialId, MonedaId = oRango.MonedaId, FechaDesde = oRango.FechaDesde};
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
                Data = mobjRangoManager.EliminarRangoConfirmacionAutomatica(oParam.Id),
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