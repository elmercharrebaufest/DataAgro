using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Helpers.Excel;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class ReporteCompraNetController : Controller
    {
        private readonly IReportesManager mobjReportesManager;
        public ReporteCompraNetController(IReportesManager oReportesManager)
        {
            mobjReportesManager = oReportesManager;
        }

        // GET: ReporteCompraNet
        public ActionResult Index()
        {
            var model = new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad()
            };

            return View(model);
        }
        public ActionResult _ReporteCompraNet()
        {
            var model = new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad()
            };
            return PartialView("_ReporteCompraNet", model);

        }
        public ExcelResult DetalleExcel(int mes, int materialId, bool? clasificacion)
        {
            var detalle = mobjReportesManager.DetallePosicion(materialId, mes, clasificacion);
            return new ExcelResult(detalle.Headers,detalle.Data,detalle.Name,detalle.SheetName);
        }
    }
}