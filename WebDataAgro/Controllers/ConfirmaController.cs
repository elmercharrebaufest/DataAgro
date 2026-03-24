using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
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
        private readonly IMaterialManager _materialManager;
        private readonly IContratoManager _contratoManager;
        private readonly IControlDeBoletosManager _controlDeBoletosManager;

        // Cache configuration
        private static readonly object _cacheLock = new object();
        private const string CONFIRMA_MATERIALES_CACHE_KEY = "Confirma_Materiales_Cache";
        private const string CONFIRMA_BOLSAS_CACHE_KEY = "Confirma_Bolsas_Cache";
        private const string CONFIRMA_COMERCIALES_CACHE_KEY = "Confirma_Comerciales_Cache";
        private const string CONFIRMA_PROVEEDORES_CACHE_KEY = "Confirma_Proveedores_Cache_{0}"; // {0} = Equipo
        private const int CACHE_DURATION_MINUTES = 5;

        public ConfirmaController(IConfirmaManager confirmaManager, IReportesManager reportesManager, IMaterialManager materialManager, IContratoManager contratoManager, IControlDeBoletosManager controlDeBoletosManager)
        {
            this.confirmaManager = confirmaManager;
            this.reportesManager = reportesManager;
            _materialManager = materialManager;
            _contratoManager = contratoManager;
            this._controlDeBoletosManager = controlDeBoletosManager;
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

            var result = confirmaManager.GrabarConfirmas(GlobalVariables.ComercialId, contratos, confirma.IsWebService, clausulas, GlobalVariables.EquipoReal);
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

        public ActionResult ListarConfirmas(string filtroArchivo = "")//Para pantalla descargar
        {
            List<ConfirmaArchivoDto> confirmas = confirmaManager.ListarConfirmas(filtroArchivo);

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
            var material = _materialManager.TraerTodoMaterial();

            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var boleto = _contratoManager.TraerTodosLosBoletos();
            var boletoListItems = boleto.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Boleto = boletoListItems;
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

        [HttpPost]
        public JsonResult BuscaDatosTabla(ConfirmaFiltroBusquedaDto filtrosBusqueda)
        {
            try
            {
                // Obtener todos las confirmas con los filtros aplicados
                var todasConfirmas = confirmaManager.TraerNegociosFiltrados(filtrosBusqueda, GlobalVariables.EquipoReal);

                // Aplicar paginación
                var confirmasQuery = todasConfirmas.AsQueryable();
                var totalRegistros = confirmasQuery.Count();

                // Aplicar ordenamiento si existe
                if (filtrosBusqueda.Sort != null && filtrosBusqueda.Sort.Any())
                {
                    var sortDescriptor = filtrosBusqueda.Sort.First();
                    var orderBy = sortDescriptor.Field + (sortDescriptor.Dir == "desc" ? " descending" : " ascending");
                    confirmasQuery = confirmasQuery.OrderBy(orderBy);
                }

                // Aplicar skip y take para paginación
                var confirmas = confirmasQuery
                    .Skip(filtrosBusqueda.Skip)
                    .Take(filtrosBusqueda.Take)
                    .ToList();

                var result = new
                {
                    Data = confirmas,
                    Total = totalRegistros
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                // Log del error para debugging
                System.Diagnostics.Debug.WriteLine($"Error en BuscaDatosTabla: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    Data = new List<object>(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.Message
                });
            }
        }

        [HttpGet]
        public ActionResult GestionarClausulas(string numeroSap)
        {
            ViewBag.ValidacionContrato = confirmaManager.ValidarContratoConfirma(numeroSap, GlobalVariables.EquipoReal);
            string mensajeValidacion = (string)ViewBag.ValidacionContrato;
            if (mensajeValidacion.Length == 0)
            {
                ViewBag.Clausulas = confirmaManager.ObtenerClausulasPorNegocio(numeroSap, GlobalVariables.EquipoReal);
                ViewBag.NegocioSAP = numeroSap;
            }
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

        public ActionResult DescargarZipConfirmas(List<string> nombresArchivos)
        {
            try
            {
                if (nombresArchivos == null || !nombresArchivos.Any())
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No se han proporcionado nombres de archivo.");
                }

                byte[] fileBytes = confirmaManager.DescargarZipConfirmas(nombresArchivos);

                if (fileBytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    return File(fileBytes, "application/zip", "confirmas.zip");
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        #region Cargar Combos
        [HttpGet]
        public JsonResult GetMateriales()
        {
            try
            {
                var cachedData = HttpContext.Cache[CONFIRMA_MATERIALES_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CONFIRMA_MATERIALES_CACHE_KEY] as List<SelectListItem>;

                        if (cachedData == null)
                        {
                            var material = _controlDeBoletosManager.GetMaterial();
                            cachedData = material.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.MaterialId.ToString(),
                                Selected = false
                            })
                            .OrderBy(x => x.Text)
                            .ToList();

                            HttpContext.Cache.Insert(
                                CONFIRMA_MATERIALES_CACHE_KEY,
                                cachedData,
                                null,
                                DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES),
                                System.Web.Caching.Cache.NoSlidingExpiration
                            );
                        }
                    }
                }

                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetMateriales: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBolsaCompraNet()
        {
            try
            {
                var cachedData = HttpContext.Cache[CONFIRMA_BOLSAS_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CONFIRMA_BOLSAS_CACHE_KEY] as List<SelectListItem>;

                        if (cachedData == null)
                        {
                            var listaEstados = this._controlDeBoletosManager.GetBolsaCompraNet();
                            cachedData = listaEstados.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.Id.ToString(),
                                Selected = false
                            })
                            .OrderBy(x => x.Text)
                            .ToList();

                            HttpContext.Cache.Insert(
                                CONFIRMA_BOLSAS_CACHE_KEY,
                                cachedData,
                                null,
                                DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES),
                                System.Web.Caching.Cache.NoSlidingExpiration
                            );
                        }
                    }
                }

                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBolsaCompraNet: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetComerciales()
        {
            try
            {
                var cachedData = HttpContext.Cache[CONFIRMA_COMERCIALES_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CONFIRMA_COMERCIALES_CACHE_KEY] as List<SelectListItem>;

                        if (cachedData == null)
                        {
                            var comercial = _controlDeBoletosManager.GetComercial().OrderBy(x => x.Apellido);
                            cachedData = comercial.Select(x => new SelectListItem
                            {
                                Text = x.Apellido,
                                Value = x.ComercialId.ToString(),
                                Selected = false
                            })
                            .OrderBy(x => x.Text)
                            .ToList();

                            HttpContext.Cache.Insert(
                                CONFIRMA_COMERCIALES_CACHE_KEY,
                                cachedData,
                                null,
                                DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES),
                                System.Web.Caching.Cache.NoSlidingExpiration
                            );
                        }
                    }
                }

                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetComerciales: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProveedores()
        {
            try
            {
                // Cache key varies by Equipo (user-specific)
                string cacheKey = string.Format(CONFIRMA_PROVEEDORES_CACHE_KEY, GlobalVariables.Equipo);
                var cachedData = HttpContext.Cache[cacheKey] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[cacheKey] as List<SelectListItem>;

                        if (cachedData == null)
                        {
                            var proveedores = _controlDeBoletosManager.GetProveedorPorComercial(GlobalVariables.Equipo).OrderBy(x => x.RazonSocial);
                            cachedData = proveedores.Select(comercial => new SelectListItem
                            {
                                Text = comercial.RazonSocial,
                                Value = comercial.ProveedorId.ToString(),
                                Selected = false
                            })
                            .OrderBy(x => x.Text)
                            .ToList();

                            HttpContext.Cache.Insert(
                                cacheKey,
                                cachedData,
                                null,
                                DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES),
                                System.Web.Caching.Cache.NoSlidingExpiration
                            );
                        }
                    }
                }

                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetProveedores: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}
