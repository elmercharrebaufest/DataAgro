using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class DiferencialController : Controller
    {
        private readonly IDiferencialManager mobjDiferencialManager;

        public DiferencialController(IDiferencialManager oDiferencialManager)
        {
            mobjDiferencialManager = oDiferencialManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionDiferencial)]
        public ActionResult Index()
        {
            var model = mobjDiferencialManager.TraerDiferencial();
            model = model ?? new DiferencialDto { };
            return View(model);
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
               var model = mobjDiferencialManager.TraerDiferencial();
                model = model ?? new DiferencialDto { };
                model.Resultado = res;
                return PartialView("_AbmDiferencial", model);
           

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