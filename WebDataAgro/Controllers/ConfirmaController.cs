using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using static WebDataAgro.MvcApplication;
using Resultado = Molinos.DataAgro.Entities.Dto.Resultado;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ConfirmaController : Controller
    {
        private readonly IConfirmaManager confirmaManager;
        private readonly IReportesManager reportesManager;

        public ConfirmaController(IConfirmaManager confirmaManager, IReportesManager reportesManager)
        {
            this.confirmaManager = confirmaManager;
            this.reportesManager = reportesManager;
        }

        public ActionResult DescargarConfirma()
        {
            return View();
        }

        public ActionResult GenerarConfirma()
        {
            CargarSeleccionables();
            return View();
        }

        [HttpPost]
        public ActionResult GenerarConfirma(ConfirmaGeneradoDto confirma)
        {
            CargarSeleccionables();
            if (string.IsNullOrEmpty(confirma.ContratoSAP)) return new JsonResult() { MaxJsonLength = Int32.MaxValue, Data = new Resultado { Errores = new List<ErrorMessage> { new ErrorMessage { Message = "No se ha ingresado ningun valor", ErrorCode = 04 } } } };
            List<string> contratos = confirma.ContratoSAP.TrimEnd(';').Split(';').ToList();
            var clausulas = confirma.Clausulas;

            var result = confirmaManager.GrabarConfirmas(confirma.ClaseNegocioId, GlobalVariables.ComercialId, contratos, confirma.IsWebService, clausulas, GlobalVariables.EquipoReal);
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult DescargarArchivoConfirma(string nombreArchivo)
        {
            try
            {
                Byte[] fileBytes = confirmaManager.ObtenerArchivoXML(nombreArchivo);

                if (fileBytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //return Json(fileBytes);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Text.Xml, nombreArchivo);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult ListarConfirmas()//Para pantalla descargar
        {
            List<ConfirmaArchivoDto> confirmas = confirmaManager.ListarConfirmas();

            var json = new JsonResult()
            {
                Data = confirmas,
                MaxJsonLength = Int32.MaxValue,
            };
            json.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return json;
        }

        private void CargarSeleccionables()
        {
            var datosCombos = confirmaManager.TraerDatosCombos();
            var claseListItems = datosCombos.clasenegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.ClaseNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.ClaseNegocio = claseListItems;
        }

        private string GetFriendlyFileSize(long lengthInBytes)
        {
            var kb = Math.Round(lengthInBytes / 1024d);
            var groupSeparator = NumberFormatInfo.CurrentInfo.NumberGroupSeparator;
            var friendly = kb.ToString("N0").Replace(groupSeparator, " ") + " KB";
            return friendly;
        }

        public ActionResult ObtenerDownloadKey(oParamBusqueda filtro)
        {
            DateTime oNow = DateTime.Now;
            string strFechaHora = oNow.ToString("yyyyMMddHHmmss");
            string strTicks = oNow.Ticks.ToString();
            string identif = strFechaHora + strTicks;

            return Json(Util.GetDownloadKey(identif));
        }

        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            var model = confirmaManager.TraerNegociosFiltrados(filtro, GlobalVariables.EquipoReal);

            return new JsonResult()
            {
                Data = model,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpGet]
        public ActionResult GestionarClausulas(string numeroSap, int tipoNegocio)
        {
            ViewBag.Clausulas = confirmaManager.ObtenerClausulasPorNegocio(numeroSap, GlobalVariables.EquipoReal);
            ViewBag.NegocioSAP = numeroSap;
            ViewBag.TipoNegocio = tipoNegocio;
            return View();
        }

        public ActionResult ValidarNegocio(string NegocioSAP)
        {
            var mensaje = confirmaManager.ValidarNegocio(NegocioSAP, GlobalVariables.EquipoReal);
            return new JsonResult() { Data = new { Mensaje = mensaje } };
        }

        public ActionResult ListarComerciales()
        {
            var comerciales = confirmaManager.ListarComerciales();
            comerciales.Sort();
            var result = new JsonResult() { Data = comerciales.Select(x => new { Comercial = x }) };
            return result;
        }

        public ActionResult ListarCorredores()
        {
            var corredores = confirmaManager.ListarCorredores();
            corredores.Sort();
            var result = new JsonResult() { Data = corredores.Select(x => new { Corredor = x }) };
            return result;
        }

        public ActionResult ListarVendedores()
        {
            var vendedores = confirmaManager.ListarVendedores();
            vendedores.Sort();
            var result = new JsonResult() { Data = vendedores.Select(x => new { Vendedor = x }) };
            return result;
        }
    }
}