using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Helpers.Excel;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteCompraNetController : Controller
    {
        private readonly IReportesManager mobjReportesManager;
        private readonly ICentroManager centroManager;

        public ReporteCompraNetController(IReportesManager oReportesManager, ICentroManager oCentroManager)
        {
            mobjReportesManager = oReportesManager;
            centroManager = oCentroManager;
        }

        // GET: ReporteCompraNet
        [Autorizacion(PermisosDataAgro.VisualizarComprasDiarias)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartialReporteCompraNet(string fechaString, string fechaHastaString, string centroId = "0")
        {
            ViewBag.Fecha = fechaString;
            ViewBag.FechaHasta = fechaHastaString;
            ViewBag.CentroSeleccionado = centroId;
            DateTime fechaDesde = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var model = ObtenerDatosReporte(fechaDesde, fechaHasta, centroId ?? "0");
            return PartialView("_ReporteCompraNet", model);

        }
        public ExcelResult DetalleExcel(int mes, int anio, int materialId, string fechaString, string fechaHastaString, int? clasificacion, string centroId = "0")
        {
            DateTime fecha = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var detalle = mobjReportesManager.DetallePosicion(materialId, mes, anio, fecha, fechaHasta, (materialId != 2) ? null : clasificacion, int.Parse(centroId));
            return new ExcelResult(detalle.Headers, detalle.Data, detalle.Name, detalle.SheetName);
        }
        public ActionResult ReporteComprasDelDia(string fechaString, string fechaHastaString, string centroId = "0")
        {
            DateTime fechaDesde = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var model = ObtenerDatosReporte(fechaDesde, fechaHasta, centroId ?? "0");
            var posicion = mobjReportesManager.PosicionPorMaterial(fechaDesde, fechaHasta);
            return File(ExcelReporteCompleto.GenerarExcel(model, posicion, fechaDesde == fechaHasta), "application/vnd.ms-excel");
        }

        private ReporteCompraNetModel ObtenerDatosReporte(DateTime fechaDesde, DateTime fechaHasta, string centroId)
        {
            int idCentro = int.Parse(centroId);
            bool filtrarAcopio = idCentro == 0 || idCentro == 1;
            var agentes = filtrarAcopio ? mobjReportesManager.TraerAgenteDeCompra(fechaDesde, fechaHasta) : new List<AgenteCompraDto>();
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

            var objetivos = mobjReportesManager.TraerHedgeObjetivo(fechaDesde, fechaHasta);
            objetivos.PricingCumplido = objetivos.PricingCumplido;
            objetivos.PricingObjetivo = objetivos.PricingObjetivo;
            objetivos.RemitirCumplido = objetivos.RemitirCumplido;
            objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
            var result = new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(fechaDesde, fechaHasta, idCentro),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(fechaDesde, fechaHasta, idCentro),
                PricingCampania = mobjReportesManager.TraerPricingCampania(fechaDesde, fechaHasta, idCentro),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(fechaDesde, fechaHasta, idCentro),
                HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(fechaDesde, fechaHasta)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = mobjReportesManager.TraerTcPromedio(fechaDesde, fechaHasta),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
            };
            return result;
        }
        private List<HedgeMaterialModel> TransformarAModel(List<HedgeMaterialDto> hedgeMat)
        {
            var lista = new List<HedgeMaterialModel>()
            {
                new HedgeMaterialModel {MaterialId = 1, MaterialDescripcion ="Hedge Maíz",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad)},
                new HedgeMaterialModel {MaterialId = 3, MaterialDescripcion ="Hedge Soja",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad) }
            };
            return lista;
        }

        public JsonResult DetalleExcelModal(int? mes, int? anio, int materialId, string fechaString, string fechaHastaString, int? clasificacion,  string centroId = "0")
        {
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            DateTime fechaHasta;
            DateTime.TryParse(fechaHastaString, out fechaHasta);
            
            return Json(mobjReportesManager.DetallePosicionModal(materialId, mes, anio, fecha, fechaHasta, (materialId != 2) ? null : clasificacion,  int.Parse(centroId)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetalleIdsModal(string negocioids,string moneda)
        {
            var listnegocioids = negocioids.Split(',').Select(a => int.Parse(a)).ToList();
          
            var result = mobjReportesManager.DetallePosicionModalIds(listnegocioids, moneda);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ExcelResult ExcelAgente(string fechaString)
        {
            DateTime fecha;
            DateTime.TryParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
            var detalle = mobjReportesManager.DetalleAgente(fecha);
            return new ExcelResult(detalle.Headers, detalle.Data, detalle.Name, detalle.SheetName);
        }
        public JsonResult DetalleAgenteModal(string fechaString)
        {
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            return Json(mobjReportesManager.DetalleAgenteModal(fecha), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCentros()
        {
            return Json(JsonConvert.SerializeObject(new { data = centroManager.TraerTodoCentro().Centro }), JsonRequestBehavior.AllowGet);
        }
    }
}