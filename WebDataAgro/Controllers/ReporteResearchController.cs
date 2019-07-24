using KendoGridBinder;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ReporteResearchController : Controller
    {
        private readonly IResearchManager oResearchManager;
        private readonly IMaterialManager oMaterialManager;
        public ReporteResearchController(IResearchManager oResearchManager, IMaterialManager oMaterialManager)
        {
            this.oResearchManager = oResearchManager;
            this.oMaterialManager = oMaterialManager;
        }
        // GET: ResearchAvanceSiembra
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


