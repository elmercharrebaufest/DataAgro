using Molinos.DataAgro.Entities.Dto.Distribucion;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using NLog;
using System;
using System.IO;
using System.Net;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
    [RoutePrefix("api/configuracion")]
    public class ConfiguracionDistribucionController : Controller
    {
        private readonly IConfiguracionDistribucionManager configuracionDistribucionManager;
        private readonly ILogger logger;

        public ConfiguracionDistribucionController(IConfiguracionDistribucionManager configuracionDistribucionManager, ILogger logger)
        {
            this.configuracionDistribucionManager = configuracionDistribucionManager;
            this.logger = logger;
        }

        [HttpGet]
        [Route("distribucion")]
        public ActionResult ObtenerConfiguracion()
        {
            return Json(configuracionDistribucionManager.ObtenerConfiguracion(), JsonRequestBehavior.AllowGet);
        }

        [HttpPut]
        [Route("distribucion")]
        public ActionResult GuardarConfiguracion()
        {
            try
            {
                var dto = LeerJsonBody<ConfiguracionDistribucionDto>();
                configuracionDistribucionManager.GuardarConfiguracion(dto);
                return Json(configuracionDistribucionManager.ObtenerConfiguracion(), JsonRequestBehavior.AllowGet);
            }
            catch (ArgumentException ex)
            {
                logger.Error(ex, "GuardarConfiguracionDistribucion inválida");
                return ApiError(HttpStatusCode.BadRequest, ex.Message);
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

        private JsonResult ApiError(HttpStatusCode statusCode, string error)
        {
            Response.StatusCode = (int)statusCode;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;
            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
        }
    }
}
