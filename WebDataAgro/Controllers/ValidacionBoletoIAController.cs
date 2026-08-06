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
        private readonly IControlDeBoletosValidacionIAManager controlDeBoletosValidacionIAManager;
        private static readonly object _cacheLock = new object();
        private const int CACHE_DURATION_MINUTES = 5;
        private const string VALID_BOLETOS_ESTADOS_CACHE_KEY = "CtrlValidacion_Estados_Cache";

        public ValidacionBoletoIAController(IControlDeBoletosValidacionIAManager controlDeBoletosValidacionIAManagerIA)
        {
            this.controlDeBoletosValidacionIAManager = controlDeBoletosValidacionIAManagerIA;
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
        public PartialViewResult _ResultadoBoletosValidacion()
        {
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
                            var listaEstados = this.controlDeBoletosValidacionIAManager.ListarEstados();
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
        [HttpGet]
        public JsonResult GetValidacionBoletoSap(string contratoSAP)
        {
            try
            {
                var boletos = this.controlDeBoletosValidacionIAManager.GetValidacionBoletoSap(contratoSAP);
                var result = new
                {
                    Data = boletos
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetValidacionBoletoSap: {ex.Message}");
                return Json(new { Data = new List<ValidacionDeBoletosIAConsultaDto>() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetValidacionBoletosResultados(int validacionBoletosId)
        {
            try
            {
                var resultados = this.controlDeBoletosValidacionIAManager.GetValidacionResultadosPorContrato(validacionBoletosId);
                var result = new
                {
                    Data = resultados,
                    Total = resultados.Count()
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetValidacionBoletosResultados : {ex.Message}");
                return Json(new { Data = new List<ValidacionBoletosResultadoDto>() }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult GetValidacionBoletosPendientes(ValidacionBoletoFiltroBusquedaDto filtros)
        {
            var boletos = this.controlDeBoletosValidacionIAManager.GetValidacionBoletosPendientes(filtros);
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
        [HttpPost]
        public JsonResult RechazarValidacionResultado(AccionesValidacionBoletosDto accionesValidacionBoletos)
        {
            try
            {
                var resultado = controlDeBoletosValidacionIAManager.RechazarValidacionResultado(accionesValidacionBoletos);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Rechazo de resultados correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al rechazar la validación de resultados: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult AprobarValidacionResultado(AccionesValidacionBoletosDto accionesValidacionBoletos)
        {
            try
            {
                var resultado = controlDeBoletosValidacionIAManager.AprobarValidacionResultado(accionesValidacionBoletos);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Aprobacion de resultados correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al aprobar la validación de resultados: " + ex.Message });
            }
        }


    }
}
