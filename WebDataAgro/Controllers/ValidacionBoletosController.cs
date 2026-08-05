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
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using System.Web.Mvc;
using WebDataAgro.Filters;

namespace WebDataAgro.Controllers
{
    [ApiTokenAuthorize]
    [RoutePrefix("api/ValidacionBoletos")]
    public class ValidacionBoletosDAController : Controller
    {
        private readonly IControlDeBoletosValidacionIAManager controlDeBoletosValidacionIAManager;
        public ValidacionBoletosDAController(IControlDeBoletosValidacionIAManager controlDeBoletosEstadoManager)
        {
            this.controlDeBoletosValidacionIAManager = controlDeBoletosEstadoManager;
        }

        [HttpGet]
        [Route("GetContrato")]
        public JsonResult GetContrato(string contratoSAP)
        {
            if (string.IsNullOrWhiteSpace(contratoSAP))
            {
                return ApiError(HttpStatusCode.BadRequest, "El parámetro contratoSAP es obligatorio.");
            }

            var contrato = controlDeBoletosValidacionIAManager.ObtenerDatosDeContrato(contratoSAP);
            if (contrato == null)
            {
                return ApiError(HttpStatusCode.NotFound, "No se encontró el contrato.");
            }
            return Json(contrato, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetClausulas")]
        public JsonResult GetClausulas(string contratoSAP)
        {
            if (string.IsNullOrWhiteSpace(contratoSAP))
            {
                return ApiError(HttpStatusCode.BadRequest, "El parámetro contratoSAP es obligatorio.");
            }

            var clausulas = controlDeBoletosValidacionIAManager.ObtenerClausulas(contratoSAP);
            if (clausulas == null || !clausulas.Any())
            {
                return ApiError(HttpStatusCode.NotFound, "No se encontraron las cláusulas.");
            }
            return Json(clausulas, JsonRequestBehavior.AllowGet);
        }

        private JsonResult ApiError(HttpStatusCode statusCode, string mensaje)
        {
            Response.StatusCode = (int)statusCode;
            Response.SuppressFormsAuthenticationRedirect = true;
            Response.TrySkipIisCustomErrors = true;

            return Json(new
            {
                ok = false,
                mensaje = mensaje
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
