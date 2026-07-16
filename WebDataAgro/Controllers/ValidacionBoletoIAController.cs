using Kendo.DynamicLinq;
using Molinos.DataAgro.Agent.ScatoRepositorio;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ValidacionBoletoIAController : Controller
    {
        private readonly IControlDeBoletosValidacionIAManagerIA controlDeBoletosValidacionIAManagerIA;
        public ValidacionBoletoIAController(IControlDeBoletosValidacionIAManagerIA controlDeBoletosValidacionIAManagerIA)
        {
            this.controlDeBoletosValidacionIAManagerIA = controlDeBoletosValidacionIAManagerIA;
        }

        #region Vistas Principales
        [Autorizacion(PermisosDataAgro.Control_de_Boletos)]
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                // Log del error
                TempData["Error"] = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }
        #endregion

        [HttpPost]
        public JsonResult GetBoletos(ControlDeBoletoFiltroBusquedaDto filtrosBusqueda)
        {
            var result = new
            {
                Data = 1,
                Total = 0
            };

            return Json(result);
        }
        [HttpPost]
        public JsonResult ExportarBoletosValidadosExcel(ControlDeBoletoFiltroBusquedaDto filtrosBusqueda)
        {
            var result = new
            {
                Data = 1,
                Total = 0
            };

            return Json(result);
        }
        [HttpPost]
        public JsonResult CargarBoletosValidacion(ControlDeBoletoFiltroBusquedaDto filtrosBusqueda)
        {
            var result = new
            {
                Data = 1,
                Total = 0
            };

            return Json(result);
        }
    }
}
