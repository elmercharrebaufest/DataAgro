using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Core;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
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
            List<string> contratos = confirma.ContratoSAP.TrimEnd(';').Split(';').ToList();
            
            var confirmas = confirmaManager.GrabarConfirmas(confirma.ClaseNegocioId, GlobalVariables.ComercialId, contratos,confirma.IsWebService, GlobalVariables.EquipoReal);
            return new JsonResult()
            {
                Data = confirmas.confirmasGenerados,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ValidarNegocios(List<string> listaCodigosSAP, int claseNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ValidarNegocios(listaCodigosSAP, claseNegocio)
            };
        }

        public ActionResult ListarNegocios(int desdeSAP, int hastaSAP, int claseNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ListarNegociosPorRangoCodigoSAP(desdeSAP, hastaSAP, claseNegocio)
            };
        }

        public ActionResult ValidarNegocio(string codigoSAP, int claseNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ValidarNegocio(codigoSAP, claseNegocio)
            };
        }

        public ActionResult FiltrarNegociosPorFecha(string desde, string hasta, int claseNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.FiltrarNegociosPorFecha(desde, hasta, claseNegocio)
            };
        }

        public ActionResult DescargarArchivoConfirma(string codigoSAP)
        {

            var nombreArchivo = confirmaManager.GenerarNombreArchivoConfirma(codigoSAP);
            try
            {
                Byte[] fileBytes = confirmaManager.ConfirmaEnByte(codigoSAP);

                if (fileBytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //return Json(fileBytes); 
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Text.Xml,nombreArchivo);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult ListarConfirmas()
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
    }
}