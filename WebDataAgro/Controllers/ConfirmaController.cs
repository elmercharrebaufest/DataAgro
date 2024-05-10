using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
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
            return View();
        }

        public ActionResult ValidarNegocios(List<string> listaCodigosSAP, int tipoNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ValidarNegocios(listaCodigosSAP, tipoNegocio)
            };
        }

        public ActionResult ListarNegocios(int desdeSAP, int hastaSAP, int tipoNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ListarNegociosPorRangoCodigoSAP(desdeSAP, hastaSAP, tipoNegocio)
            };
        }

        public ActionResult ValidarNegocio(string codigoSAP, int tipoNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.ValidarNegocio(codigoSAP, tipoNegocio)
            };
        }

        //FiltrarNegociosPorFecha
        public ActionResult FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio)
        {
            return new JsonResult()
            {
                Data = confirmaManager.FiltrarNegociosPorFecha(desde, hasta, tipoNegocio)
            };
        }

        private void CargarSeleccionables()
        {
            var datosCombos = confirmaManager.TraerDatosCombos();
            var tiposListItems = datosCombos.tiponegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = tiposListItems;
        }
    }
}