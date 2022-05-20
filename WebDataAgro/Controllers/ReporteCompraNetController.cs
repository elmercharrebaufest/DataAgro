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
        private readonly IMaterialManager materialManager;

        public ReporteCompraNetController(IReportesManager oReportesManager, ICentroManager oCentroManager, IMaterialManager oMaterialManager)
        {
            mobjReportesManager = oReportesManager;
            centroManager = oCentroManager;
            materialManager = oMaterialManager;
        }

        // GET: ReporteCompraNet
        [Autorizacion(PermisosDataAgro.VisualizarComprasDiarias)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PartialReporteCompraNet(string fechaString, string fechaHastaString, List<int> materialId, string centroId = "0", string verFijaciones = "")
        {
            ViewBag.Fecha = fechaString;
            ViewBag.FechaHasta = fechaHastaString;
            ViewBag.CentroSeleccionado = centroId;
            ViewBag.Materiales = materialId == null ? "" : string.Join(",", materialId);
            ViewBag.verFijaciones = verFijaciones == "on";
            DateTime fechaDesde = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            if (materialId == null || materialId.Count() == 0) materialId = materialManager.TraerDatosIniciales().Material.Select(x => x.MaterialId).ToList();
            ReporteCompraNetModel model = mobjReportesManager.ObtenerDatosReporteCompraNet(fechaDesde, fechaHasta, centroId ?? "0", materialId, verFijaciones == "on");
            return PartialView("_ReporteCompraNet", model);

        }
        public ExcelResult DetalleExcel(int mes, int anio, int materialId, string fechaString, string fechaHastaString, int? clasificacion, string centroId = "0", bool verFijaciones = true)
        {
            DateTime fecha = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var detalle = mobjReportesManager.DetallePosicion(materialId, mes, anio, fecha, fechaHasta, (materialId != 2) ? null : clasificacion, int.Parse(centroId),verFijaciones);
            return new ExcelResult(detalle.Headers, detalle.Data, detalle.Name, detalle.SheetName);
        }
        public ActionResult ReporteComprasDelDia(string fechaString, string fechaHastaString, string materialId, string centroId = "0", bool verFijaciones = true)
        {
            var listmaterialId = String.IsNullOrEmpty(materialId) ? new List<int>() : materialId.Split(',').Select(a => int.Parse(a)).ToList();

            DateTime fechaDesde = DateTime.ParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            var model = mobjReportesManager.ObtenerDatosReporteCompraNet(fechaDesde, fechaHasta, centroId ?? "0", listmaterialId, verFijaciones);
            var posicion = mobjReportesManager.PosicionPorMaterial(fechaDesde, fechaHasta, verFijaciones);
            return File(ExcelReporteCompleto.GenerarExcel(model, posicion, fechaDesde == fechaHasta), "application/vnd.ms-excel");
        }

        //private ReporteCompraNetModel ObtenerDatosReporte(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId)
        //{
        //    int idCentro = int.Parse(centroId);
        //    bool filtrarAcopio = idCentro == 0 || idCentro == 1;
        //    var agentes = filtrarAcopio ? mobjReportesManager.TraerAgenteDeCompra(fechaDesde, fechaHasta, materialId) : new List<AgenteCompraDto>();
        //    var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
        //    agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

        //    var objetivos = mobjReportesManager.TraerHedgeObjetivo(fechaDesde, fechaHasta,materialId);
        //    objetivos.PricingCumplido = objetivos.PricingCumplido;
        //    objetivos.PricingObjetivo = objetivos.PricingObjetivo;
        //    objetivos.RemitirCumplido = objetivos.RemitirCumplido;
        //    objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
        //    var result = new ReporteCompraNetModel
        //    {
        //        ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(fechaDesde, fechaHasta,materialId, idCentro),
        //        SojaSustentable = (materialId==null || materialId.Contains(3))? mobjReportesManager.TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro): new ReporteSojaSustDto(),
        //        PosicionCompras = mobjReportesManager.TraerPosicionCompras(fechaDesde, fechaHasta,materialId, idCentro),
        //        PricingCampania = mobjReportesManager.TraerPricingCampania(fechaDesde, fechaHasta, materialId, idCentro),
        //        PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(fechaDesde, fechaHasta, materialId, idCentro),
        //        HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(fechaDesde, fechaHasta, materialId)),
        //        HedgeObjetivo = objetivos,
        //        TCPromedioDto = mobjReportesManager.TraerTcPromedio(fechaDesde, fechaHasta,materialId),
        //        AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
        //    };
        //    return result;
        //}


        public JsonResult DetalleExcelModal(int? mes, int? anio, int materialId, string fechaString, string fechaHastaString, int? clasificacion, string centroId = "0", bool verFijaciones = true)
        {
            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            DateTime fechaHasta;
            DateTime.TryParse(fechaHastaString, out fechaHasta);

            return Json(mobjReportesManager.DetallePosicionModal(materialId, mes, anio, fecha, fechaHasta, (materialId != 2) ? null : clasificacion, int.Parse(centroId),verFijaciones), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetalleIdsModal(List<int> negocioids, string moneda, int? verDepositoTipoNegocio)
        {
            var result = mobjReportesManager.DetallePosicionModalIds(negocioids, moneda, verDepositoTipoNegocio);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetalleIdsSojaSustentableModal(List<int> negocioids, string moneda)
        {
            var result = mobjReportesManager.SustentablePosicionModalIds(negocioids, moneda);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ExcelResult ExcelAgente(string fechaString, string materialId)
        {
            var listmaterialId = String.IsNullOrEmpty(materialId) ? new List<int>() : materialId.Split(',').Select(a => int.Parse(a)).ToList();

            DateTime fecha;
            DateTime.TryParseExact(fechaString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha);
            var detalle = mobjReportesManager.DetalleAgente(fecha, listmaterialId);
            return new ExcelResult(detalle.Headers, detalle.Data, detalle.Name, detalle.SheetName);
        }
        public JsonResult DetalleAgenteModal(string fechaString, string materialId)
        {
            var listmaterialId = String.IsNullOrEmpty(materialId) ? new List<int>() : materialId.Split(',').Select(a => int.Parse(a)).ToList();

            DateTime fecha;
            DateTime.TryParse(fechaString, out fecha);
            return Json(mobjReportesManager.DetalleAgenteModal(fecha, listmaterialId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerCentros()
        {
            return Json(JsonConvert.SerializeObject(new { data = centroManager.TraerTodoCentro().Centro }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ObtenerMateriales()
        {
            List<MaterialCombo> materiales = materialManager.TraerDatosIniciales().Material;
            return Json(JsonConvert.SerializeObject(new { data = materiales }), JsonRequestBehavior.AllowGet);

        }

        public ActionResult ExcelModeloAltaMasiva()
        {
            var materiales = materialManager.TraerTodoMaterial().Material;
            var centros = centroManager.TraerTodoCentro().Centro;
            return File(ExcelReporteCompleto.ExcelModeloAltaMasiva(materiales, centros), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}