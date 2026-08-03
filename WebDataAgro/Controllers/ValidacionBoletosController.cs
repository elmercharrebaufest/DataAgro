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
        private readonly IControlDeBoletosValidacionIAManagerIA controlDeBoletosEstadoManager;
        public ValidacionBoletosDAController(IControlDeBoletosValidacionIAManagerIA controlDeBoletosEstadoManager)
        {
            this.controlDeBoletosEstadoManager = controlDeBoletosEstadoManager;
        }

        [HttpGet]
        [Route("GetContrato")]
        public JsonResult GetContrato(string contratoSAP)
        {
            if (string.IsNullOrWhiteSpace(contratoSAP))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new
                {
                    ok = false,
                    mensaje = "El parámetro contratoSAP es obligatorio."
                }, JsonRequestBehavior.AllowGet);
            }

            var contrato = controlDeBoletosEstadoManager.ObtenerDatosDeContrato(contratoSAP);
            if (contrato == null)
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    ok = false,
                    mensaje = "No se encontró el contrato.",
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(contrato, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetClausulas")]
        public JsonResult GetClausulas(string contratoSAP)
        {
            if (string.IsNullOrWhiteSpace(contratoSAP))
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json(new
                {
                    ok = false,
                    mensaje = "El parámetro contratoSAP es obligatorio."
                }, JsonRequestBehavior.AllowGet);
            }

            var clausulas = controlDeBoletosEstadoManager.ObtenerClausulas(contratoSAP);
            if (clausulas == null || !clausulas.Any())
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                return Json(new
                {
                    ok = false,
                    mensaje = "No se encontraron las cláusulas."
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(clausulas, JsonRequestBehavior.AllowGet);
        }
    }
}
