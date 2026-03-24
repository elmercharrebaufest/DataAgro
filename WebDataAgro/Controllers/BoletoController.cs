using Kendo.DynamicLinq;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class BoletoController : Controller
    {
        private readonly IBoletoManager boletoManager;
        private readonly IReportesManager reportesManager;
        private readonly IMaterialManager materialManager;
        private readonly IControlDeBoletosManager _controlDeBoletosManager;

        private readonly string _logDir;

        // Cache configuration
        private static readonly object _cacheLock = new object();
        private const string BOLETO_MATERIALES_CACHE_KEY = "Boleto_Materiales_Cache";
        private const string BOLETO_BOLSAS_CACHE_KEY = "Boleto_Bolsas_Cache";
        private const string BOLETO_COMERCIALES_CACHE_KEY = "Boleto_Comerciales_Cache";
        private const string BOLETO_PROVEEDORES_CACHE_KEY = "Boleto_Proveedores_Cache_{0}"; // {0} = Equipo
        private const int CACHE_DURATION_MINUTES = 5;

        public BoletoController(IBoletoManager boletoManager, IReportesManager reportesManager, IMaterialManager materialManager, IControlDeBoletosManager controlDeBoletosManager)
        {
            this.boletoManager = boletoManager;
            this.reportesManager = reportesManager;
            this.materialManager = materialManager;
            this._controlDeBoletosManager = controlDeBoletosManager;
            _logDir = ConfigurationManager.AppSettings["PathBoletos"].ToString();
        }

        public ActionResult Index()
        {
            CompletarVista();
            return View();
        }

        [HttpGet]
        public ActionResult GenerarBoletos()
        {
            CompletarVista();
            return View();
        }

        [HttpPost]
        public ActionResult GenerarBoletos(BoletoDto boleto)
        {
            CompletarVista();
            List<string> contratos = new List<string>();
            var tipoNegocios = new List<int>();
            boleto.ComercialId = GlobalVariables.ComercialId;
            if (boleto.TipoNegocioId == 1) // Contrato
            {
                tipoNegocios.Add((int)EnumTipoNegocio.A_FIJAR);
                tipoNegocios.Add((int)EnumTipoNegocio.A_PRECIO);
            }
            else // Fijación
            {
                tipoNegocios.Add((int)EnumTipoNegocio.FIJACION);
            }

            boleto.ContratoSAP = Regex.Replace(boleto.ContratoSAP, @"\s+", ";");

            foreach (string itemContrato in boleto.ContratoSAP.TrimEnd(';').Split(';').ToList())
            {
                contratos.Add(itemContrato.PadLeft(10, '0'));
            }
            var boletos = boletoManager.GrabarBoleto(contratos, tipoNegocios, boleto, GlobalVariables.EquipoReal);

            return new JsonResult()
            {
                Data = boletos.BoletosDto,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void CompletarVista()
        {
            var tipoNegocio = boletoManager.TraerDatosCombo(null);
            var tiposListItems = tipoNegocio.tiponegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = tiposListItems;

            var material = materialManager.TraerTodoMaterial();

            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

        }

        [HttpGet]
        public ActionResult DescargarBoletos()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ListarBoletos()//Para Pantalla Descargar Boleto
        {
            List<BoletoArchivoDto> boletos = new List<BoletoArchivoDto>();
            if (Directory.Exists(_logDir))
            {
                boletos = Directory.GetFiles(_logDir)
                .Where(path => path.EndsWith(".pdf"))
                .Select(path => new FileInfo(path))
                .Select(file => new BoletoArchivoDto
                {
                    Nombre = file.Name,
                    FechaUltimaEscritura = file.LastWriteTime.ToString("yyyy/MM/dd HH:mm"),
                    Url = _logDir + file.Name,
                    Tamano = GetFriendlyFileSize(file.Length)
                })
                .OrderByDescending(x => x.Nombre)
                .ToList();
            }

            return Json(boletos);
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
            var identif = boletoManager.ObtenerIdentDescarga();

            return Json(Util.GetDownloadKey(identif));
        }

        //[HttpPost]
        public ActionResult DescargarArchivoBoleto(string nombre)
        {
            try
            {
                Byte[] fileBytes = boletoManager.BoletoEnByte(_logDir + "\\" + nombre);

                if (fileBytes == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                else
                {
                    //return Json(fileBytes);
                    return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, nombre);
                }
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        public ActionResult ReenviarEmailBoletos(List<string> contratosBoletos, List<string> nombresArchivos)
        {
            try
            {
                boletoManager.ReenviarBoletos(contratosBoletos, nombresArchivos, _logDir);
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [HttpGet]
        public ActionResult GestionarClausulas(string numeroSap)
        {

            ViewBag.ValidacionContrato = boletoManager.ValidarContratoTipoBoleto(numeroSap, GlobalVariables.EquipoReal);
            string mensajeValidacion = (string)ViewBag.ValidacionContrato;
            if (mensajeValidacion.Length == 0)
            {
                ViewBag.Clausulas = boletoManager.ObtenerClausulasPorNegocio(numeroSap, GlobalVariables.EquipoReal);
                ViewBag.NegocioSAP = numeroSap;
            }
            return View();
        }

        [HttpPost]
        public JsonResult BuscaDatosTabla(BoletoFiltroBusquedaDto filtrosBusqueda)
        {
            try
            {
                // Obtener todos los boletos con los filtros aplicados
                var todosBoletos = boletoManager.TraerContratosFiltrados(filtrosBusqueda, GlobalVariables.EquipoReal);

                // Aplicar paginación
                var boletosQuery = todosBoletos.AsQueryable();
                var totalRegistros = boletosQuery.Count();

                // Aplicar ordenamiento si existe
                if (filtrosBusqueda.Sort != null && filtrosBusqueda.Sort.Any())
                {
                    var sortDescriptor = filtrosBusqueda.Sort.First();
                    var orderBy = sortDescriptor.Field + (sortDescriptor.Dir == "desc" ? " descending" : " ascending");
                    boletosQuery = boletosQuery.OrderBy(orderBy);
                }

                // Aplicar skip y take para paginación
                var boletos = boletosQuery
                    .Skip(filtrosBusqueda.Skip)
                    .Take(filtrosBusqueda.Take)
                    .ToList();

                var result = new
                {
                    Data = boletos,
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

        public ActionResult ValidarNegocio(string NegocioSAP)
        {
            var mensaje = boletoManager.ValidarNegocio(NegocioSAP, GlobalVariables.EquipoReal);
            return new JsonResult() { Data = new { Mensaje = mensaje } };
        }

        #region Cargar Combos
        [HttpGet]
        public JsonResult GetMateriales()
        {
            try
            {
                var cachedData = HttpContext.Cache[BOLETO_MATERIALES_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[BOLETO_MATERIALES_CACHE_KEY] as List<SelectListItem>;

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
                                BOLETO_MATERIALES_CACHE_KEY,
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
                var cachedData = HttpContext.Cache[BOLETO_BOLSAS_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[BOLETO_BOLSAS_CACHE_KEY] as List<SelectListItem>;

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
                                BOLETO_BOLSAS_CACHE_KEY,
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
                var cachedData = HttpContext.Cache[BOLETO_COMERCIALES_CACHE_KEY] as List<SelectListItem>;

                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[BOLETO_COMERCIALES_CACHE_KEY] as List<SelectListItem>;

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
                                BOLETO_COMERCIALES_CACHE_KEY,
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
                string cacheKey = string.Format(BOLETO_PROVEEDORES_CACHE_KEY, GlobalVariables.Equipo);
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
