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
    public class RangoConfirmacionAutomaticaController : Controller
    {
        private IRangoConfirmacionAutomaticaManager mobjRangoManager;
        private IComercialManager mobjComercialManager;

        public RangoConfirmacionAutomaticaController(IComercialManager oComercialManager, IRangoConfirmacionAutomaticaManager oRangoManager)
        {
            mobjRangoManager = oRangoManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            string ActionView = "";

            if (GlobalVariables.Perfil != EnumPerfil.Mesa || !mobjComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            return View(ActionView);
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

            var entityErrors = mobjRangoManager.GrabarRangoConfirmacionAutomatica(oRango);

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