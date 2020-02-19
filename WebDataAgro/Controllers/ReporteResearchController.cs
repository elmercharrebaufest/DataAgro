using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteResearchController : Controller
    {
        private readonly IResearchManager oResearchManager;
        public ReporteResearchController(IResearchManager oResearchManager)
        {
            this.oResearchManager = oResearchManager;
        }
        // GET: ResearchAvanceSiembra
        [Autorizacion(PermisosDataAgro.ReporteResearch)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteAvanceCosechaPartial()
        {
            return PartialView("_ReporteAvanceCosecha");
        }

        [HttpPost]
        public ActionResult BuscarDatosAvanceCosecha(KendoGridMvcRequest request)
        {

            var model = oResearchManager.TraerAvanceCosecha(request);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult AvanceSiembraPartial()
        {
            return PartialView("_ReporteAvanceSiembra");
        }
        [HttpPost]
        public ActionResult BuscaDatosAvanceSiembra(KendoGridMvcRequest request)
        {
            var model = oResearchManager.TraerAvanceSiembra(request);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult ReporteSituacionCultivoPartial()
        {
            return PartialView("_ReporteSituacionCultivo");
        }
        [HttpPost]
        public ActionResult BuscarDatosSituacionCultivoParcial(KendoGridMvcRequest request)
        {
            var model = oResearchManager.TraerSituacionCultivoParcial(request);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult ReporteVentaStockPartial()
        {
            return PartialView("_ReporteVentaStock");
        }
        [HttpPost]
        public ActionResult BuscarDatosVentaStock(KendoGridMvcRequest request)
        {
            var model = oResearchManager.TraerVentaStock(request);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

    }
}


