using Molinos.DataAgro.Interfaces;
using System;
using System.Globalization;
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
           return View();
        }
        public ActionResult PartialReporteCompraNet(string fechaString)
        {
            ViewBag.Fecha = fechaString;
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            var model = ObtenerDatosReporte(fecha);
            return PartialView("_ReporteCompraNet", model);

        }
        public ExcelResult DetalleExcel(int mes, int materialId,string fechaString, bool? clasificacion)
        {
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            var detalle = mobjReportesManager.DetallePosicion(materialId, mes, fecha, clasificacion);
            return new ExcelResult(detalle.Headers,detalle.Data,detalle.Name,detalle.SheetName);
        }

        public ActionResult ReporteComprasDelDia(string fechaString)
        {
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            var model = ObtenerDatosReporte(fecha);
            var posicion = mobjReportesManager.PosicionPorMaterial(fecha);
            return File(ExcelReporteCompleto.GenerarExcel(model, posicion), "application/vnd.ms-excel");

        }

        private ReporteCompraNetModel ObtenerDatosReporte(DateTime fecha)
        {
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(fecha),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(fecha),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(fecha),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(fecha)
            };
        }
    }
}