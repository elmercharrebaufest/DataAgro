using Kendo.DynamicLinq;
using Molinos.DataAgro.Agent.ScatoRepositorio;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
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
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ValidacionBoletoIAController : Controller
    {
        private readonly IControlDeBoletosValidacionIAManagerIA controlDeBoletosValidacionIAManagerIA;
        private static readonly object _cacheLock = new object();
        private const int CACHE_DURATION_MINUTES = 5;
        private const string VALID_BOLETOS_ESTADOS_CACHE_KEY = "CtrlValidacion_Estados_Cache";

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

        #region Vistas Parciales
        [HttpGet]
        public PartialViewResult _CargarBoletosValidacion()
        {
            return PartialView("_CargarBoletosValidacion");
        }
        [HttpGet]
        public PartialViewResult _ResultadoBoletosValidacion(int id)
        {
            ViewBag.NegocioId = id;
            return PartialView("_ResultadoBoletosValidacion");
        }
        #endregion

        [HttpGet]
        public JsonResult GetEstadosValidacion()
        {
            try
            {
                var cachedData = HttpContext.Cache[VALID_BOLETOS_ESTADOS_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[VALID_BOLETOS_ESTADOS_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var listaEstados = this.controlDeBoletosValidacionIAManagerIA.ListarEstados();
                            cachedData = listaEstados.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.Id.ToString(),
                                Selected = false
                            }).ToList();
                            HttpContext.Cache.Insert(VALID_BOLETOS_ESTADOS_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetEstadosControl: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetValidacionBoletosPendientes(ValidacionBoletoFiltroBusquedaDto filtros)
        {
            var boletos = this.controlDeBoletosValidacionIAManagerIA.GetValidacionBoletosPendientes(filtros);
            var result = new
            {
                Data = boletos,
                Total = boletos.Count
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
