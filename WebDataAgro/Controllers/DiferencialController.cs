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
    public class DiferencialController : Controller
    {
        private IDiferencialManager mobjDiferencialManager;
        private IComercialManager mobjComercialManager;

        public DiferencialController(IComercialManager oComercialManager, IDiferencialManager oDiferencialManager)
        {
            mobjDiferencialManager = oDiferencialManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Mesa)
            {
                var model = mobjDiferencialManager.TraerDiferencial();
                model = model ?? new DiferencialDto { };
                return View(model);
            }
            else
            {
                return View("ErrorDePermisos");
            }
        }

        [HttpPost]
        public ActionResult GrabarDiferencial(DiferencialDto diferencialDto)
        {
            Diferencial diferencial = new Diferencial
            {
                DiferencialDefault = diferencialDto.DiferencialDefault,
                Fecha = DateTime.Now,
                ComercialId = GlobalVariables.ComercialId
            };
            var res = mobjDiferencialManager.GrabarDiferencial(diferencial);
            return AbmDiferencialPartial(res);

        }

        public ActionResult AbmDiferencialPartial(Resultado res)
        {
            if (GlobalVariables.Perfil == EnumPerfil.Mesa)
            {
                var model = mobjDiferencialManager.TraerDiferencial();
                model = model ?? new DiferencialDto { };
                model.Resultado = res;
                return PartialView("_AbmDiferencial", model);
            }
            else
            {
                return View("ErrorDePermisos");
            }

        }

        public ActionResult Eliminar(int id)
        {
            return new JsonResult()
            {
                Data = mobjDiferencialManager.EliminarDiferencial(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}