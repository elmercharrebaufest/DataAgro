using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ConfirmaController : Controller
    {
        private readonly IConfirmaManager confirmaManager;
        private readonly IReportesManager reportesManager;
        private readonly string _logDir;

        public ConfirmaController(IConfirmaManager confirmaManager, IReportesManager reportesManager)
        {
            this.confirmaManager = confirmaManager;
            this.reportesManager = reportesManager;
            //_logDir = ConfigurationManager.AppSettings["PathConfirmas"].ToString();
        }
        
        [HttpGet]
        public ActionResult GenerarConfirma()
        {
            CargarSeleccionables();
            return View();
        }

        [HttpPost]
        public ActionResult GenerarConfirma(ConfirmaGeneradoDto confirma)
        {
            CargarSeleccionables();
            List<string> contratos = new List<string>();
            foreach (string itemContrato in confirma.ContratoSAP.TrimEnd(';').Split(';').ToList())
            {
                contratos.Add(itemContrato);
            }
            var confirmas = confirmaManager.GrabarConfirmas(confirma.ClaseNegocioId, GlobalVariables.ComercialId, contratos,confirma.IsWebService);
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

        public ActionResult DescargarConfirma(string codigoSAP)
        {   //confirma20211021_0002670442
            var nombreArchivo = "confirma"+DateTime.Now.Year.ToString()+DateTime.Now.Month.ToString()+DateTime.Now.Day.ToString()+"_000"+codigoSAP+".xml";
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
    }
}