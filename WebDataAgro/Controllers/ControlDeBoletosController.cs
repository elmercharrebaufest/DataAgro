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
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ControlDeBoletosController : Controller
    {
        // TODO: Inyectar servicios reales
        // private readonly IBoletoService _boletoService;
        private readonly IControlDeBoletosEstadoManager _controlDeBoletosEstadoManager;
        private readonly IControlDeBoletosManager _controlDeBoletosManager;
        private readonly IContratoManager _contratoManager;
        // private readonly IComercialService _comercialService;

        private static readonly object _cacheLock = new object();
        private const string CTRL_BOLETOS_MATERIALES_CACHE_KEY = "CtrlBoletos_Materiales_Cache";
        private const string CTRL_BOLETOS_ESTADOS_CACHE_KEY = "CtrlBoletos_Estados_Cache";
        private const string CTRL_BOLETOS_BOLSAS_CACHE_KEY = "CtrlBoletos_Bolsas_Cache";
        private const string CTRL_BOLETOS_BOLSAS_SAP_CACHE_KEY = "CtrlBoletos_BolsasSAP_Cache";
        private const string CTRL_BOLETOS_COMERCIALES_CACHE_KEY = "CtrlBoletos_Comerciales_Cache";
        private const string CTRL_BOLETOS_PROVEEDORES_CACHE_KEY = "CtrlBoletos_Proveedores_Cache_{0}";
        private const string CTRL_BOLETOS_PROVINCIAS_CACHE_KEY = "CtrlBoletos_Provincias_Cache";
        private const string CTRL_BOLETOS_COSECHAS_CACHE_KEY = "CtrlBoletos_Cosechas_Cache";
        private const string CTRL_BOLETOS_CLASIFICACIONES_CACHE_KEY = "CtrlBoletos_Clasificaciones_Cache";
        private const string CTRL_BOLETOS_PROCEDENCIAS_CACHE_KEY = "CtrlBoletos_Procedencias_Cache_{0}";
        private const string CTRL_BOLETOS_TIPO_OBLEA_CACHE_KEY = "CtrlBoletos_TipoOblea_Cache";
        private const int CACHE_DURATION_MINUTES = 5;

        public ControlDeBoletosController(IControlDeBoletosEstadoManager controlDeBoletosEstadoManager, IControlDeBoletosManager controlDeBoletosManager, IContratoManager contratoManager)
        {
            this._controlDeBoletosEstadoManager = controlDeBoletosEstadoManager;
            this._controlDeBoletosManager = controlDeBoletosManager;
            this._contratoManager = contratoManager;
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
        public ActionResult SeguimientoBoletos()
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
        public ActionResult ModificacionMasivaBoletos()
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

        #region Pendiente de control
        [HttpPost]
        public JsonResult GetBoletos(ControlDeBoletoFiltroBusquedaDto filtrosBusqueda)
        {
            try
            {

                // Obtener todos los boletos con los filtros aplicados
                var todosBoletos = this._controlDeBoletosManager.GetControlBoletosPendientes(filtrosBusqueda);

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
                System.Diagnostics.Debug.WriteLine($"Error en GetBoletos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    Data = new List<object>(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.StackTrace
                });
            }
        }

        [HttpPost]
        public ActionResult ExportarBoletosExcel(ControlDeBoletoFiltroBusquedaDto filtrosBusqueda)
        {
            try
            {
                var todosBoletos = this._controlDeBoletosManager.GetControlBoletosPendientes(filtrosBusqueda);

                // Definir encabezados según la grilla principal
                string[] headers = new string[] {
                    "Tipo Boleto",
                    "Bolsa",
                    "Contrato SAP",
                    "Material",
                    "Estado Confirma",
                    "Estado",
                    "Fecha Creación",
                    "Proveedor",
                    "Comercial"
                };

                // Convertir datos
                var data = new List<string[]>();
                foreach (var b in todosBoletos)
                {
                    data.Add(new string[] {
                        b.TipoBoleto?.ToString() ?? "",
                        b.BolsaCompraNet?.ToString() ?? "",
                        b.ContratoSAP ?? "",
                        b.Material ?? "",
                        b.EstadoConfirma ?? "",
                        b.ControlDeBoletosEstado ?? "",
                        b.FechaCreacion.ToString("dd/MM/yyyy") ?? "",
                        b.Proveedor ?? "",
                        b.Comercial ?? ""
                    });
                }

                var fileName = $"Boletos_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var sheetName = "Boletos";
                return new WebDataAgro.Helpers.Excel.ExcelResult(headers, data, fileName, sheetName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public JsonResult GetTrackingBoleto(int controlDeBoletosId)
        {
            try
            {
                var tracking = _controlDeBoletosManager.ObtenerTrackingBoletos(controlDeBoletosId);

                var rows = tracking.SelectMany(t =>
                {
                    var acciones = string.IsNullOrEmpty(t.Acciones)
                        ? new List<Molinos.DataAgro.Entities.Dto.Acciones>()
                        : JsonConvert.DeserializeObject<List<Molinos.DataAgro.Entities.Dto.Acciones>>(t.Acciones)
                          ?? new List<Molinos.DataAgro.Entities.Dto.Acciones>();

                    return acciones
                        .Where(a => a.FechaHora != null)
                        .Select(a => new
                        {
                            a.FechaHora,
                            a.Accion,
                            a.Resultado,
                            a.Apellido,
                            a.Nombre,
                            a.TipoDocumento,
                            a.NroDocumento,
                            a.Cargo
                        });
                }).ToList();

                return Json(new { Data = rows, Total = rows.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Data = new List<object>(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.Message
                });
            }
        }
        #endregion

        #region Seguimiento de Boleto
        [HttpPost]
        public JsonResult GetReporteDeSeguimientoBoletos(ControlDeBoletoFiltroSeguimientoDto filtrosBusqueda)
        {
            try
            {

                // Obtener todos los boletos con los filtros aplicados
                var todosBoletos = this._controlDeBoletosManager.GetReporteDeSeguimientoBoletos(filtrosBusqueda);

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
                System.Diagnostics.Debug.WriteLine($"Error en GetBoletos: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    Data = new List<object>(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.Message
                });
            }
        }

        [HttpPost]
        public ActionResult ExportarReporteDeSeguimientoBoletosExcel(ControlDeBoletoFiltroSeguimientoDto filtrosBusqueda)
        {
            try
            {
                var todosBoletos = this._controlDeBoletosManager.GetReporteDeSeguimientoBoletos(filtrosBusqueda);

                // Definir encabezados
                string[] headers = new string[] {
                    "Tipo Boleto",
                    "Bolsa",
                    "Contrato SAP",
                    "Material",
                    "Proveedor",
                    "Fecha Certificación",
                    "Fecha Vencimiento Certificación",
                    "Fecha Envio Boleto",
                    "Fecha Recepción Boleto",
                    "Fecha Envío a Firma",
                    "Fecha Recibido de Firma",
                    "Fecha Envío a Bolsa",
                    "Fecha Vuelta de Bolsa",
                    "Fecha Envio a Afip",
                    "Fecha Vuelta a Afip"
                };

                // Convertir datos
                var data = new List<string[]>();
                foreach (var b in todosBoletos)
                {
                    data.Add(new string[] {
                        b.TipoBoleto?.ToString() ?? "",
                        b.BolsaCompraNet?.ToString() ?? "",
                        b.ContratoSAP ?? "",
                        b.Material ?? "",
                        b.Proveedor ?? "",
                        b.FechaCertificacion?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaVencimientoCertificacion?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaEnvio?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaRecepBoleto?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaEnviadoFirma?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaRecibFirma?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaEnvioBolsa?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaVueltaBolsa?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaEnvioAfip?.ToString("dd/MM/yyyy") ?? "",
                        b.FechaVueltaAfip?.ToString("dd/MM/yyyy") ?? ""
                    });
                }

                var fileName = $"ReporteSeguimiento_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                var sheetName = "ReporteSeguimiento";
                return new WebDataAgro.Helpers.Excel.ExcelResult(headers, data, fileName, sheetName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Datos de PreCertificacion
        [HttpGet]
        public JsonResult GetVerificarTipoBoletoyFechaRecepcion(int controlDeBoletosId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.VerificarTipoBoletoyFechaRecepcion(controlDeBoletosId);
                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Data = new object(),
                    Total = 0,
                    Errors = "Error al verificar tipo de boleto y fecha de recepción: " + ex.Message
                });
            }
        }
        [HttpGet]
        public JsonResult GetDatosPreCertificacion(int controlDeBoletosId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.ObtenerDatosPreCertificacion(controlDeBoletosId);
                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Data = new object(),
                    Total = 0,
                    Errors = "Error al Obtener datos de pre certificacion: " + ex.Message
                });
            }
        }
        [HttpGet]
        public JsonResult GetVerificarDuplicidadObleaCodigoArca(int controlDeBoletosId, string numeroOblea, string codigoArca)
        {
            try
            {
                var validacion = _controlDeBoletosManager.VerificarDuplicidadObleaCodigoArca(controlDeBoletosId, numeroOblea, codigoArca);
                return Json(validacion, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Data = new object(),
                    Total = 0,
                    Errors = "Error al verificar duplicidad de oblea y código Arca: " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion)
        {
            try
            {
                var resultado = _controlDeBoletosManager.RegistrarDatosPreCertificacion(controlDeBoletosPreCertificacion);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Modificacion los datos de precertificacion correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar los datos de precertificacion en el control de boletos: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EliminarDatosPreCertificacion(int controlDeBoletosId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.EliminarPreCertificacion(controlDeBoletosId);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Se eliminaron los datos de precertificacion correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar los datos de precertificacion en el control de boletos: " + ex.Message });
            }
        }
        #endregion

        #region Datos de Seguimiento
        [HttpGet]
        public JsonResult GetDatosDeSeguimiento(int controlDeBoletosId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.ObtenerDatosDeSeguimiento(controlDeBoletosId);
                return Json(resultado, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Data = new object(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.Message
                });
            }
        }
        [HttpPost]
        public JsonResult RegistrarDatosDeSeguimiento(ControlDeBoletosDatosSeguimientoDto seguimientoControlDeBoleto)
        {
            try
            {
                var resultado = _controlDeBoletosManager.RegistroDatosDeSeguimiento(seguimientoControlDeBoleto);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Modificacion los datos de seguimiento correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar los datos de seguimiento en el control de boletos: " + ex.Message });
            }
        }
        [HttpPost]
        public JsonResult EliminarDatosDeSeguimiento(int controlDeBoletosId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.EliminarDatosSeguimiento(controlDeBoletosId);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Se eliminaron los datos de seguimiento correctamente";
                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar los datos de seguimiento en el control de boletos: " + ex.Message });
            }
        }
        #endregion

        #region Modificacion de Contrato
        [HttpPost]
        public JsonResult ModificarContrato(ControlDeBoletosModificacionContratoDto controlDeBoletosModificacion)
        {
            try
            {
                controlDeBoletosModificacion.Usuario = PermisosHelper.ObtenerUsuario();
                var resultado = _controlDeBoletosManager.ModificacionContrato(controlDeBoletosModificacion);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Contrato modificado correctamente";

                return Json(new { success = success, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar el contrato: " + ex.Message });
            }
        }
        [HttpGet]
        public ActionResult ObtenerDatosDeContrato(int id)
        {
            try
            {
                var resultado = _controlDeBoletosManager.ObtenerDatosDeContrato(id);
                return Json(resultado, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al obtener los datos del contrato: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ObtenerDetalleContrato(int id)
        {
            if (id <= 0)
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;

            var filtro = new Kendo.DynamicLinq.Filter
            {
                Logic = "and",
                Filters = new List<Kendo.DynamicLinq.Filter>
                {
                    new Kendo.DynamicLinq.Filter
                    {
                        Field = "Id",
                        Operator = "eq",
                        Value = id
                    }
                }
            };

            var request = new DataSourceRequest();
            request.Filter = filtro;
            request.Sort = new List<Sort> { new Sort { Field = "Estado_Order", Dir = "asc" }, new Sort { Field = "Fecha_Order", Dir = "desc" } };

            var model = _contratoManager.TraerTodosContratos(request, PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial), equipo, GlobalVariables.CorredoresComercial);
            // Convertir la colección de resultados a List<object>
            List<object> dataList;
            if (model != null && model.Data != null)
            {
                dataList = model.Data.Cast<object>().ToList();
            }
            else
            {
                dataList = new List<object>();
            }

            var result = new
            {
                Data = dataList.FirstOrDefault(),
                Total = dataList.Count
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Metodos Get para cargar combos
        [HttpGet]
        public JsonResult GetMateriales()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_MATERIALES_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_MATERIALES_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var material = _controlDeBoletosManager.GetMaterial();
                            cachedData = material.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.MaterialId.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_MATERIALES_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
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
        public JsonResult GetEstadosControl()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_ESTADOS_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_ESTADOS_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var listaEstados = this._controlDeBoletosEstadoManager.ListarTodo();
                            cachedData = listaEstados.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.Id.ToString(),
                                Selected = false
                            }).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_ESTADOS_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
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
        public JsonResult GetBolsaCompraNet()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_BOLSAS_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_BOLSAS_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var listaEstados = this._controlDeBoletosManager.GetBolsaCompraNet();
                            cachedData = listaEstados.Select(x => new SelectListItem
                            {
                                Text = x.Descripcion,
                                Value = x.Id.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_BOLSAS_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
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
        public JsonResult GetBolsaCompraNetSAP()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_BOLSAS_SAP_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_BOLSAS_SAP_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var listaEstados = this._controlDeBoletosManager.GetBolsaCompraNet();
                            cachedData = listaEstados.Select(x => new SelectListItem
                            {
                                Text = x.Id.ToString(),
                                Value = x.CodigoSap,
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_BOLSAS_SAP_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBolsaCompraNetSAP: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetComerciales()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_COMERCIALES_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_COMERCIALES_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var comercial = _controlDeBoletosManager.GetComercial().OrderBy(x => x.Apellido);
                            cachedData = comercial.Select(x => new SelectListItem
                            {
                                Text = x.Apellido,
                                Value = x.ComercialId.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_COMERCIALES_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
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
                string cacheKey = string.Format(CTRL_BOLETOS_PROVEEDORES_CACHE_KEY, GlobalVariables.Equipo);
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
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(cacheKey, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
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

        [HttpGet]
        public JsonResult GetProvincias()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_PROVINCIAS_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_PROVINCIAS_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var provincias = _controlDeBoletosManager.GetProvincias();
                            cachedData = provincias.Select(provincia => new SelectListItem
                            {
                                Text = provincia.Nombre,
                                Value = provincia.ProvinciaId.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_PROVINCIAS_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetProvincias: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetCosechas()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_COSECHAS_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_COSECHAS_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var campanias = _controlDeBoletosManager.GetCosechas();
                            cachedData = campanias.Select(campania => new SelectListItem
                            {
                                Text = campania.Descripcion,
                                Value = campania.CampaniaId.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_COSECHAS_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetCosechas: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetClasificaciones()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_CLASIFICACIONES_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_CLASIFICACIONES_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var clasificaciones = _controlDeBoletosManager.GetClasificaciones();
                            cachedData = clasificaciones.Select(clasificacion => new SelectListItem
                            {
                                Text = clasificacion.Descripcion,
                                Value = clasificacion.Id.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_CLASIFICACIONES_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetClasificaciones: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetProcedencias(int provinciaId)
        {
            try
            {
                string cacheKey = string.Format(CTRL_BOLETOS_PROCEDENCIAS_CACHE_KEY, provinciaId);
                var cachedData = HttpContext.Cache[cacheKey] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[cacheKey] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var localidades = _controlDeBoletosManager.GetProcedencias(provinciaId);
                            cachedData = localidades.Select(localidad => new SelectListItem
                            {
                                Text = localidad.Nombre,
                                Value = localidad.LocalidadId.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(cacheKey, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetProcedencias: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetTipoOblea()
        {
            try
            {
                var cachedData = HttpContext.Cache[CTRL_BOLETOS_TIPO_OBLEA_CACHE_KEY] as List<SelectListItem>;
                if (cachedData == null)
                {
                    lock (_cacheLock)
                    {
                        cachedData = HttpContext.Cache[CTRL_BOLETOS_TIPO_OBLEA_CACHE_KEY] as List<SelectListItem>;
                        if (cachedData == null)
                        {
                            var tipoOblea = _controlDeBoletosManager.GetTipoOblea();
                            cachedData = tipoOblea.Select(tipo => new SelectListItem
                            {
                                Text = tipo.Descripcion,
                                Value = tipo.Id.ToString(),
                                Selected = false
                            }).OrderBy(x => x.Text).ToList();
                            HttpContext.Cache.Insert(CTRL_BOLETOS_TIPO_OBLEA_CACHE_KEY, cachedData, null, DateTime.Now.AddMinutes(CACHE_DURATION_MINUTES), System.Web.Caching.Cache.NoSlidingExpiration);
                        }
                    }
                }
                return Json(cachedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetTipoOblea: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetBoletoSap(int boletoCompraNetId)
        {
            try
            {
                var boletosSap = _controlDeBoletosManager.GetBoletoSap(boletoCompraNetId);
                return Json(boletosSap, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error GetBoletoSap: {ex.Message}");
                return Json(new List<SelectListItem>(), JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region Vistas Parciales
        [HttpGet]
        public PartialViewResult _ModificarDatosDelContrato(int id)
        {
            ViewBag.NegocioId = id;
            return PartialView("_ModificarDatosDelContrato");
        }
        [HttpGet]
        public PartialViewResult _TrackingControlDeBoletos(int id)
        {
            ViewBag.ControlDeBoletosId = id;
            return PartialView("_TrackingControlDeBoletos");
        }

        [HttpGet]
        public PartialViewResult _VisualizarContrato(int id)
        {
            ViewBag.NegocioId = id;
            return PartialView("_VisualizarContrato");
        }
        [HttpGet]
        public PartialViewResult _DatosCertificacion(int id)
        {
            ViewBag.ControlDeBoletosId = id;
            return PartialView("_DatosCertificacion");
        }
        [HttpGet]
        public PartialViewResult _SeguimientoControlBoleto(int id)
        {
            ViewBag.ControlDeBoletosId = id;
            return PartialView("_SeguimientoControlBoleto");
        }
        [HttpGet]
        public ActionResult _GestionControlBoleto(int? controlDeBoletosId, int? seguimientoBoletoId, int? preCertificacionId, int? negocioId)
        {
            try
            {
                ViewBag.ControlDeBoletosId = controlDeBoletosId ?? 0;
                ViewBag.SeguimientoBoletoId = seguimientoBoletoId ?? 0;
                ViewBag.PreCertificacionId = preCertificacionId ?? 0;
                ViewBag.NegocioId = negocioId ?? 0;
                return PartialView("_GestionControlBoleto");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la página: " + ex.Message;
                return View("Error");
            }
        }
        #endregion

        #region Modificacion masiva de boletos
        [HttpPost]
        public JsonResult GetBoletosParaModificar(ControlDeBoletosParaModificarFiltroDto filtrosBusqueda)
        {
            try
            {
                var boletos = this._controlDeBoletosManager.GetBoletosParaModificar(filtrosBusqueda);

                var result = new
                {
                    Data = boletos,
                    Total = boletos.Count
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GetBoletosParaModificar: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    Data = new List<object>(),
                    Total = 0,
                    Errors = "Error al cargar datos: " + ex.Message
                });
            }
        }

        [HttpPost]
        public JsonResult GuardarBoletosParaModificarFechas(List<ControlDeBoletosParaModificarDto> boletos)
        {
            try
            {
                var resultado = this._controlDeBoletosManager.GuardarBoletosParaModificarFechas(boletos);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Se guardaron los cambios correctamente";
                return Json(new
                {
                    success = success,
                    message = mensaje
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GuardarBoletosParaModificarFechas: {ex.Message}");
                return Json(new
                {
                    success = false,
                    errors = new[] { new { Message = "Error al guardar fechas: " + ex.Message } }
                });
            }
        }
        #endregion

        #region Eliminacion de control de boletos
        [HttpPost]
        public JsonResult EliminarControlDeBoletos(EliminarControlDeBoletoDto eliminarControlDeBoleto)
        {
            try
            {
                var resultado = this._controlDeBoletosManager.EliminarControlDeBoletos(eliminarControlDeBoleto);
                bool success = !resultado.HayError;
                string mensaje = resultado.HayError ? resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next) : "Eliminación realizada correctamente";
                return Json(new
                {
                    success = success,
                    message = mensaje
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en EliminarControlDeBoletos: {ex.Message}");
                return Json(new
                {
                    success = false,
                    errors = new[] { new { Message = "Error al eliminar control de boletos: " + ex.Message } }
                });
            }
        }
        #endregion

        #region Descarga Documento Confirma
        [HttpGet]
        public ActionResult GetDocumentoConfirmaPDF(int controlDeBoletoId)
        {
            var documentoConfirma = _controlDeBoletosManager.ObtenerDocumentoConfirma(controlDeBoletoId);

            if (documentoConfirma == null)
            {
                return HttpNotFound();
            }

            if (documentoConfirma.PdfBinario == null || documentoConfirma.PdfBinario.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NoContent, "No se encontró el PDF.");
            }

            return File(
                documentoConfirma.PdfBinario,
                "application/pdf",
                $"Documento_Confirma_{controlDeBoletoId}.pdf");
        }
        #endregion

    }

    #region ViewModels y DTOs
    public class ModificarControlBoletoViewModel
    {
        public int Id { get; set; }
        public string ImProvincia { get; set; }
        public string ImClasificacion { get; set; }
        public string ImCosecha { get; set; }
        public string ImProcedencia { get; set; }
        public DateTime? ImFecha { get; set; }
        public TimeSpan? ImHora { get; set; }
    }

    public class FiltrosBoletoModel
    {
        public string ContratoSAPDesde { get; set; }
        public string ContratoSAPHasta { get; set; }
        public int? MaterialId { get; set; }
        public int? EstadoControlId { get; set; }
        public bool EsConfirma { get; set; }
        public DateTime? FechaCargaDesde { get; set; }
        public DateTime? FechaCargaHasta { get; set; }
        public int? Proveedor { get; set; }
        public int? BolsaId { get; set; }
        public int? ComercialId { get; set; }
    }

    #endregion
}
