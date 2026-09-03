using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
    [RoutePrefix("api/distribucion")]
    public class DistribucionApiController : Controller
    {
        private readonly IDistribucionCuposManager distribucionCuposManager;
        private readonly ICupoManager cupoManager;
        private readonly ILogger logger;

        public DistribucionApiController(IDistribucionCuposManager distribucionCuposManager, ICupoManager cupoManager, ILogger logger)
        {
            this.distribucionCuposManager = distribucionCuposManager;
            this.cupoManager = cupoManager;
            this.logger = logger;
        }

        [HttpPost]
        [Route("calcular")]
        public ActionResult Calcular()
        {
            try
            {
                var request = LeerJsonBody<DistribucionCuposRequestDto>();
                if ((request.LimitesPorMaterial ?? new Dictionary<string, int>()).Any(x => x.Value < 0))
                {
                    return ApiError(HttpStatusCode.BadRequest, "Los límites por material no pueden ser negativos");
                }

                return ApiJson(distribucionCuposManager.Calcular(request));
            }
            catch (ArgumentException ex)
            {
                return ApiError(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Route("calcular-multi-dia")]
        public ActionResult CalcularMultiDia()
        {
            try
            {
                var request = LeerJsonBody<DistribucionMultiDiaRequestDto>();
                if ((request.LimitesPorDia ?? new List<LimiteDiaDto>()).SelectMany(x => x.Limites ?? new Dictionary<string, int>()).Any(x => x.Value < 0))
                {
                    return ApiError(HttpStatusCode.BadRequest, "Los límites por material no pueden ser negativos");
                }

                return ApiJson(distribucionCuposManager.CalcularMultiDia(request));
            }
            catch (ArgumentException ex)
            {
                return ApiError(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ApiError((HttpStatusCode)422, ex.Message);
            }
        }

        [HttpPost]
        [Route("procesar-en-dataagro")]
        public ActionResult ProcesarEnDataAgro()
        {
            try
            {
                List<string> errores = new List<string>();
                var request = LeerJsonBody<ProcesarEnDataAgroRequestDto>();
                if (request == null || request.Sap == null || !request.Sap.Any())
                {
                    return Json(new { resultado = false, error = "El array 'sap' no puede estar vacío" });
                }

                var dataTable = new DataTable();
                dataTable.Columns.Add("ContratoSAP", typeof(string));
                dataTable.Columns.Add("FechaSugerida", typeof(double));
                dataTable.Columns.Add("CantidadDeCupos", typeof(int));

                foreach (var fila in request.Sap)
                {
                    var fecha = DateTime.ParseExact(fila.FechaSugerida, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    dataTable.Rows.Add(fila.ContratoSAP, fecha.ToOADate(), fila.CantidadDeCupos);
                }

                var dataSet = new DataSet();
                dataSet.Tables.Add(dataTable);
                var resultado = cupoManager.AltaMasivaSugerenciaCuposV2(dataSet);
                if (resultado.Count != 0 && resultado.First().IsFatal == true)
                {
                    errores.Add("Ninguno de los contratos ingresados existe en la base.");
                    return Json(new { Resume = errores, Resultado = false });
                }

                return Json(new { resultado = true, resume = resultado, mensaje = "Cupos procesados correctamente" });
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ProcesarEnDataAgro error");
                return Json(new { resultado = false, error = "Error al procesar: " + ex.Message });
            }
        }

        private T LeerJsonBody<T>()
        {
            Request.InputStream.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(Request.InputStream))
            {
                return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
            }
        }

        private JsonResult ApiJson(object data)
        {
            var json = Json(data, JsonRequestBehavior.AllowGet);
            json.MaxJsonLength = int.MaxValue;
            return json;
        }

        private JsonResult ApiError(HttpStatusCode statusCode, string error)
        {
            Response.StatusCode = (int)statusCode;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
        }
    }
}
