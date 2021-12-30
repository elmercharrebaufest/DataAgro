using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class DiferencialController : Controller
    {
        private readonly IDiferencialManager mobjDiferencialManager;
        private ITipoNegocioManager tipoNegocioManager;

        public DiferencialController(IDiferencialManager oDiferencialManager, ITipoNegocioManager tipoNegocioManager)
        {
            mobjDiferencialManager = oDiferencialManager;
            this.tipoNegocioManager = tipoNegocioManager;
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
                ComercialId = GlobalVariables.ComercialId,
                TipoNegocioId = diferencialDto.TipoNegocioId
            };
            var res = mobjDiferencialManager.GrabarDiferencial(diferencial);
            return AbmDiferencialPartial(res);

        }

        public ActionResult AbmDiferencialPartial(Resultado res)
        {
               var model = mobjDiferencialManager.TraerDiferencial();
                model = model ?? new DiferencialDto { };
                model.Resultado = res;
            var tipos = tipoNegocioManager.TraerTodoTipoNegocio().Where(x => x.TipoNegocioId == 1 || x.TipoNegocioId == 2).ToList();
            ViewBag.TipoNegocioList = tipos.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString(),
                        Selected = false
                    });
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