using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace WebDataAgro.Controllers
{
    public class ControlDeBoletosController : Controller
    {
        // TODO: Inyectar servicios reales
        // private readonly IBoletoService _boletoService;
        private readonly IMaterialManager _materialManager;
        private readonly IControlDeBoletosEstadoManager _controlDeBoletosEstadoManager;
        // private readonly IEstadoService _estadoService;
        // private readonly IComercialService _comercialService;

        public ControlDeBoletosController(IMaterialManager materialManager, IControlDeBoletosEstadoManager controlDeBoletosEstadoManager)
        {
            this._materialManager = materialManager;
            this._controlDeBoletosEstadoManager = controlDeBoletosEstadoManager;
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

        [HttpGet]
        [OutputCache(Duration = 300, VaryByParam = "none")] // Cache por 5 minutos
        public JsonResult GetMateriales()
        {
            try
            {
                var material = _materialManager.TraerTodoMaterial();
                var materialesListItems = material.Material.Select(
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
        public JsonResult GetComerciales()
        {
            try
            {
                var comerciales = new List<object>
                {
                    new { Value = "1", Text = "Juan Pérez" },
                    new { Value = "2", Text = "María García" },
                    new { Value = "3", Text = "Carlos López" },
                    new { Value = "4", Text = "Ana Martínez" },
                    new { Value = "5", Text = "Luis Rodríguez" }
                };

                return Json(comerciales, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetBoletos(string filtros)
        {
            try
            {
                // Deserializar filtros si vienen como JSON
                FiltrosBoletoModel filtrosObj = null;
                if (!string.IsNullOrEmpty(filtros))
                {
                    try
                    {
                        filtrosObj = JsonConvert.DeserializeObject<FiltrosBoletoModel>(filtros);
                    }
                    catch
                    {
                        // Si falla la deserialización, crear objeto vacío
                        filtrosObj = new FiltrosBoletoModel();
                    }
                }
                else
                {
                    filtrosObj = new FiltrosBoletoModel();
                }

                // TODO: Implementar paginación real con Kendo DataSourceRequest
                var boletos = GenerarDatosEjemplo()
                    .Where(b => AplicarFiltros(b, filtrosObj))
                    .ToList();

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

        public ActionResult Detalle(int id)
        {
            try
            {
                // TODO: Implementar vista de detalle real
                ViewBag.BoletoId = id;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar detalle: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        #region Métodos Privados

        private List<BoletoViewModel> GenerarDatosEjemplo()
        {
            var random = new Random();
            var estados = new[] { "Pendiente", "En Proceso", "Completado", "Certificado" };
            var materiales = new[] { "Soja", "Maíz", "Trigo", "Girasol", "Sorgo" };
            var proveedores = new[] { "Proveedor A S.A.", "Proveedor B S.R.L.", "Proveedor C S.A.", "Cooperativa XYZ", "Agropecuaria ABC" };
            var comerciales = new[] { "Juan Pérez", "María García", "Carlos López", "Ana Martínez", "Luis Rodríguez" };

            var boletos = new List<BoletoViewModel>();

            for (int i = 1; i <= 100; i++)
            {
                boletos.Add(new BoletoViewModel
                {
                    Id = i,
                    ContratoSAP = (1000000 + i).ToString(),
                    Material = materiales[random.Next(materiales.Length)],
                    Estado = estados[random.Next(estados.Length)],
                    FechaCarga = DateTime.Today.AddDays(-random.Next(0, 30)),
                    Proveedor = proveedores[random.Next(proveedores.Length)],
                    Comercial = comerciales[random.Next(comerciales.Length)]
                });
            }

            return boletos;
        }

        private bool AplicarFiltros(BoletoViewModel boleto, FiltrosBoletoModel filtros)
        {
            if (filtros == null) return true;

            // Filtro por rango de contrato SAP

            /*
            if (!string.IsNullOrEmpty(filtros.ContratoSAPDesde) &&
                int.TryParse(boleto.ContratoSAP, out int contratoNum) &&
                int.TryParse(filtros.ContratoSAPDesde, out int contratoDesde) &&
                contratoNum < contratoDesde)
                return false;

            if (!string.IsNullOrEmpty(filtros.ContratoSAPHasta) &&
                int.TryParse(boleto.ContratoSAP, out contratoNum) &&
                int.TryParse(filtros.ContratoSAPHasta, out int contratoHasta) &&
                contratoNum > contratoHasta)
                return false;
            */

            // Filtro por material
            if (!string.IsNullOrEmpty(filtros.MaterialId) &&
                boleto.Material != GetMaterialPorId(filtros.MaterialId))
                return false;

            // Filtro por estado
            if (!string.IsNullOrEmpty(filtros.EstadoControlId) &&
                boleto.Estado != GetEstadoPorId(filtros.EstadoControlId))
                return false;

            // Filtro por proveedor
            if (!string.IsNullOrEmpty(filtros.Proveedor) &&
                !boleto.Proveedor.ToLower().Contains(filtros.Proveedor.ToLower()))
                return false;

            // Filtro por comercial
            if (!string.IsNullOrEmpty(filtros.ComercialId) &&
                boleto.Comercial != GetComercialPorId(filtros.ComercialId))
                return false;

            // Filtro por fecha
            if (filtros.FechaCargaDesde.HasValue &&
                boleto.FechaCarga < filtros.FechaCargaDesde.Value)
                return false;

            if (filtros.FechaCargaHasta.HasValue &&
                boleto.FechaCarga > filtros.FechaCargaHasta.Value)
                return false;

            return true;
        }

        private string GetMaterialPorId(string id)
        {
            var materiales = new Dictionary<string, string>
            {
                {"1", "Soja"}, {"2", "Maíz"}, {"3", "Trigo"},
                {"4", "Girasol"}, {"5", "Sorgo"}, {"6", "Cebada"}
            };
            return materiales.ContainsKey(id) ? materiales[id] : "";
        }

        private string GetEstadoPorId(string id)
        {
            var estados = new Dictionary<string, string>
            {
                {"1", "Pendiente"}, {"2", "En Proceso"},
                {"3", "Completado"}, {"4", "Certificado"}
            };
            return estados.ContainsKey(id) ? estados[id] : "";
        }

        private string GetComercialPorId(string id)
        {
            var comerciales = new Dictionary<string, string>
            {
                {"1", "Juan Pérez"}, {"2", "María García"}, {"3", "Carlos López"},
                {"4", "Ana Martínez"}, {"5", "Luis Rodríguez"}
            };
            return comerciales.ContainsKey(id) ? comerciales[id] : "";
        }

        #endregion
    }

    #region ViewModels y DTOs

    public class BoletoViewModel
    {
        public int Id { get; set; }
        public string ContratoSAP { get; set; }
        public string Material { get; set; }
        public string Estado { get; set; }
        public DateTime FechaCarga { get; set; }
        public string Proveedor { get; set; }
        public string Comercial { get; set; }
    }

    public class FiltrosBoletoModel
    {
        public string ContratoSAPDesde { get; set; }
        public string ContratoSAPHasta { get; set; }
        public string MaterialId { get; set; }
        public string EstadoControlId { get; set; }
        public bool EsConfirma { get; set; }
        public DateTime? FechaCargaDesde { get; set; }
        public DateTime? FechaCargaHasta { get; set; }
        public string Proveedor { get; set; }
        public string ComercialId { get; set; }
    }

    #endregion
}