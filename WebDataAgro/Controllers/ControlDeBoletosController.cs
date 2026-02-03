using Kendo.DynamicLinq;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
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


        [HttpPost]
        public JsonResult GetBoletos(string filtros)
        {
            try
            {
                // Deserializar filtros si vienen como JSON
                ControlDeBoletoFiltroBusquedaDto filtrosBusqueda = null;
                if (!string.IsNullOrEmpty(filtros))
                {
                    try
                    {
                        filtrosBusqueda = JsonConvert.DeserializeObject<ControlDeBoletoFiltroBusquedaDto>(filtros);
                    }
                    catch
                    {
                        // Si falla la deserialización, crear objeto vacío
                        filtrosBusqueda = new ControlDeBoletoFiltroBusquedaDto();
                    }
                }
                else
                {
                    filtrosBusqueda = new ControlDeBoletoFiltroBusquedaDto();
                }

                // TODO: Implementar paginación real con Kendo DataSourceRequest

                var boletos = this._controlDeBoletosManager.GetControlBoletosPendientes(filtrosBusqueda);

                //var boletos = GenerarDatosEjemplo()
                //    .Where(b => AplicarFiltros(b, filtrosBusqueda))
                //    .ToList();

                var result = new
                {
                    Data = boletos,
                    Total = boletos.Count
                };

                return Json(result);
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

        [HttpPost]
        public JsonResult ControlMasivo()
        {
            try
            {
                // Leer datos del cuerpo de la petición
                var requestBody = "";
                using (var reader = new System.IO.StreamReader(Request.InputStream))
                {
                    requestBody = reader.ReadToEnd();
                }

                if (string.IsNullOrEmpty(requestBody))
                {
                    return Json(new { success = false, message = "No se recibieron datos" });
                }

                dynamic data = JsonConvert.DeserializeObject(requestBody);
                string accion = data.accion;
                List<int> ids = ((Newtonsoft.Json.Linq.JArray)data.ids).ToObject<List<int>>();

                if (ids == null || !ids.Any())
                {
                    return Json(new { success = false, message = "No se han seleccionado boletos" });
                }

                // TODO: Implementar lógica real de control masivo
                // _boletoService.ProcesarControlMasivo(accion, ids);

                var mensaje = accion == "iniciar"
                    ? $"Control iniciado correctamente para {ids.Count} boleto(s)"
                    : $"Control finalizado correctamente para {ids.Count} boleto(s)";

                return Json(new { success = true, message = mensaje });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error procesando solicitud: " + ex.Message });
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
                var result = new
                {
                    Data = new List<object>(),
                    Total = 0
                };

                return Json(result);
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
        #endregion

        #region Vistas Parciales
        [HttpGet]
        public PartialViewResult _ModificarDatosDelContrato(int id)
        {
            var model = new ModificarControlBoletoViewModel
            {
                Id = id
                // Si quieres, aquí puedes precargar datos del boleto
            };

            return PartialView("_ModificarDatosDelContrato", model);
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