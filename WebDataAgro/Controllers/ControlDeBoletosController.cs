using Kendo.DynamicLinq;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities.Common.Enums;
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
using System.Web.Mvc;
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

        public ControlDeBoletosController(IControlDeBoletosEstadoManager controlDeBoletosEstadoManager, IControlDeBoletosManager controlDeBoletosManager, IContratoManager contratoManager)
        {
            this._controlDeBoletosEstadoManager = controlDeBoletosEstadoManager;
            this._controlDeBoletosManager = controlDeBoletosManager;
            this._contratoManager = contratoManager;
        }

        #region Vistas Principales
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
                    Errors = "Error al cargar datos: " + ex.Message
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

        [HttpGet]
        public JsonResult GetContadores()
        {
            try
            {
                // TODO: Implementar lógica real de contadores
                var contadores = new
                {
                    Pendientes = 25,
                    EnProceso = 12,
                    Completados = 68,
                    Certificados = 34
                };

                return Json(contadores, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Pendientes = 0,
                    EnProceso = 0,
                    Completados = 0,
                    Certificados = 0
                }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Datos de PreCertificacion
        [HttpGet]
        public JsonResult GetDatosPreCertificacion(int datosPreCertificacionId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.ObtenerDatosPreCertificacion(datosPreCertificacionId);
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
        public JsonResult RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion)
        {
            try
            {
                var resultado = _controlDeBoletosManager.RegistrarDatosPreCertificacion(controlDeBoletosPreCertificacion);
                string mensaje = "Modificacion los datos de pre certificacion correctamente";
                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar los datos de pre certificacion en el control de boletos: " + ex.Message });
            }
        }
        #endregion

        #region Datos de Seguimiento
        [HttpGet]
        public JsonResult GetDatosDeSeguimiento(int datosSeguimientoId)
        {
            try
            {
                var resultado = _controlDeBoletosManager.ObtenerDatosDeSeguimiento(datosSeguimientoId);
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
                string mensaje = "Modificacion los datos de seguimiento correctamente";
                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar los datos de seguimiento en el control de boletos: " + ex.Message });
            }
        }
        #endregion

        [HttpPost]
        public JsonResult ControlMasivo(ControlDeBoletosRegistrarAccionesDto controlDeBoletosRegistrarAcciones)
        {
            try
            {
                var accion = (EnumControlDeBoletosAcciones)controlDeBoletosRegistrarAcciones.AccionControlDeBoletos;
                var resultado = _controlDeBoletosManager.RegistrarAcciones(controlDeBoletosRegistrarAcciones.ControlDeBoletoIds, accion);
                string mensaje = "Se modifico la accion en el control de boletos";
                if (resultado.HayError)
                {
                    mensaje = resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next);
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al registrar la accion en el control de boletos: " + ex.Message });
            }
        }

        [HttpGet]
        public ActionResult ExportarExcel(FiltrosBoletoModel filtros)
        {
            try
            {
                // TODO: Implementar exportación real a Excel
                // var datos = _boletoService.GetBoletosParaExport(filtros);
                // return new ExcelResult(datos, "ControlBoletos_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");

                TempData["Success"] = "Exportación en desarrollo";
                return RedirectToAction("Index");
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
                var result = new
                {
                    Data = tracking,
                    Total = tracking.Count
                };
                return Json(result, JsonRequestBehavior.AllowGet);
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

        #region Modificacion de Contrato
        [HttpPost]
        public JsonResult ModificarContrato(ControlDeBoletosModificacionContratoDto controlDeBoletosModificacion)
        {
            try
            {
                controlDeBoletosModificacion.Usuario = PermisosHelper.ObtenerUsuario();
                var resultado = _controlDeBoletosManager.ModificacionContrato(controlDeBoletosModificacion);
                string mensaje = "Contrato modificado correctamente";
                if (resultado.HayError)
                {
                    mensaje = resultado.ListaErrores.ToArray().Select(e => e.Message).Aggregate((current, next) => current + "; " + next);
                    return Json(new { success = false, message = mensaje });
                }

                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al modificar el contrato: " + ex.Message });
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
        [OutputCache(Duration = 300, VaryByParam = "none")] // Cache por 5 minutos
        public JsonResult GetMateriales()
        {
            try
            {
                var material = _controlDeBoletosManager.GetMaterial();
                var materialesListItems = material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
                ViewBag.Material = materialesListItems;

                return Json(materialesListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log del error
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetEstadosControl()
        {
            try
            {
                var listaEstados = this._controlDeBoletosEstadoManager.ListarTodo();
                var estadoItems = listaEstados.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value
                    );

                return Json(estadoItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetBolsaCompraNet()
        {
            try
            {
                var listaEstados = this._controlDeBoletosManager.GetBolsaCompraNet();
                var estadoItems = listaEstados.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value
                    );

                return Json(estadoItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetBolsaCompraNetSAP()
        {
            try
            {
                var listaEstados = this._controlDeBoletosManager.GetBolsaCompraNet();
                var estadoItems = listaEstados.Select(
                    x => new SelectListItem
                    {
                        Text = x.Id.ToString(),
                        Value = x.CodigoSap,
                        Selected = false
                    }).OrderBy(x => x.Value
                    );

                return Json(estadoItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetComerciales()
        {
            try
            {

                var comercial = _controlDeBoletosManager.GetComercial().OrderBy(x => x.Apellido);

                var comercialesListItems = comercial.Select(
                    x => new SelectListItem
                    {
                        Text = x.Apellido,
                        Value = x.ComercialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
                return Json(comercialesListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetProveedores()
        {
            try
            {
                var proveedores = _controlDeBoletosManager.GetProveedorPorComercial(GlobalVariables.Equipo).OrderBy(x => x.RazonSocial);
                var proveedoresListItems = proveedores.Select(comercial =>
                    new SelectListItem
                    {
                        Text = comercial.RazonSocial,
                        Value = comercial.ProveedorId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);

                return Json(proveedoresListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetProvincias()
        {
            try
            {
                var provincias = _controlDeBoletosManager.GetProvincias();
                var provinciasListItems = provincias.Select(provincia =>
                    new SelectListItem
                    {
                        Text = provincia.Nombre,
                        Value = provincia.ProvinciaId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(provinciasListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetCosechas()
        {
            try
            {
                var campanias = _controlDeBoletosManager.GetCosechas();
                var campaniasListItems = campanias.Select(campania =>
                    new SelectListItem
                    {
                        Text = campania.Descripcion,
                        Value = campania.CampaniaId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(campaniasListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetClasificaciones()
        {
            try
            {
                var clasificaciones = _controlDeBoletosManager.GetClasificaciones();
                var clasificacionesListItems = clasificaciones.Select(clasificacion =>
                    new SelectListItem
                    {
                        Text = clasificacion.Descripcion,
                        Value = clasificacion.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(clasificacionesListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetProcedencias(int provinciaId)
        {
            try
            {
                var localidades = _controlDeBoletosManager.GetProcedencias(provinciaId);
                var localidadesListItems = localidades.Select(localidad =>
                    new SelectListItem
                    {
                        Text = localidad.Nombre,
                        Value = localidad.LocalidadId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(localidadesListItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }
        
        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetTipoOblea()
        {
            try
            {
                var tipoOblea = _controlDeBoletosManager.GetTipoOblea();
                var tipoObleaItems = tipoOblea.Select(tipo =>
                    new SelectListItem
                    {
                        Text = tipo.Descripcion,
                        Value = tipo.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(tipoObleaItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")]
        public JsonResult GetBoletoCompraNet()
        {
            try
            {
                var boletosCompraNet = _controlDeBoletosManager.GetBoletoCompraNet();
                var boletosCompraNetItems = boletosCompraNet.Select(tipo =>
                    new SelectListItem
                    {
                        Text = tipo.Descripcion,
                        Value = tipo.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Text);

                return Json(boletosCompraNetItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
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