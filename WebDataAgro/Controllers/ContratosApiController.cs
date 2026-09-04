using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
    [RoutePrefix("api/contratos")]
    public class ContratosApiController : Controller
    {
        private readonly ISapArchivoParserManager sapArchivoParserManager;
        private readonly ILogger logger;

        public ContratosApiController(ISapArchivoParserManager sapArchivoParserManager, ILogger logger)
        {
            this.sapArchivoParserManager = sapArchivoParserManager;
            this.logger = logger;
        }

        [HttpPost]
        [Route("importar")]
        public ActionResult Importar(HttpPostedFileBase file, string fecha)
        {
            if (file == null || file.ContentLength <= 0)
            {
                return ApiError(HttpStatusCode.BadRequest, "Debe seleccionar un archivo.");
            }

            var extension = Path.GetExtension(file.FileName) ?? string.Empty;
            if (!EsExtensionValida(extension))
            {
                return ApiError(HttpStatusCode.BadRequest, "Formato de archivo no válido. Solo se aceptan .xls, .xlsx, .txt, .tsv");
            }

            if (file.ContentLength > 10 * 1024 * 1024)
            {
                return ApiError(HttpStatusCode.BadRequest, "El archivo supera el tamaño máximo permitido de 10 MB");
            }

            DateTime fechaBase = DateTime.Today;
            if (!string.IsNullOrWhiteSpace(fecha) && !DateTime.TryParse(fecha, out fechaBase))
            {
                return ApiError(HttpStatusCode.BadRequest, "La fecha debe tener formato yyyy-MM-dd");
            }

            try
            {
                var resultado = sapArchivoParserManager.ParsearArchivo(file.InputStream, extension, fechaBase.Date);
                return ApiJson(resultado);
            }
            catch (InvalidOperationException ex)
            {
                logger.Error(ex, "Importar contratos inválido");
                return ApiError((HttpStatusCode)422, ex.Message);
            }
        }

        private static bool EsExtensionValida(string extension)
        {
            var normalizada = (extension ?? string.Empty).ToLowerInvariant();
            return normalizada == ".xls" || normalizada == ".xlsx" || normalizada == ".txt" || normalizada == ".tsv";
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
