using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using System;
using static WebDataAgro.MvcApplication;
using System.Web.Script.Serialization;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using KendoGridBinder.ModelBinder.Mvc;
using KendoGridBinder;
using Molinos.DataAgro.Entities.Dto;
using System.ComponentModel;
using System.Linq;
using WebDataAgro.Atributos;
using Molinos.DataAgro.Entities.Seguridad;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class SugerenciaCupoController : Controller
    {
        private readonly ILogger logger;
        private readonly ICupoManager cupoManager;

        public SugerenciaCupoController(ILogger logger, ICupoManager cupoManager)

        {
            this.logger = logger;
            this.cupoManager = cupoManager;
        }

        [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DatosConfiguracion(KendoGridMvcRequest request)
        {
            var lista = cupoManager.ObtenerSugerenciaCupo(GlobalVariables.ComercialId);
            var result = new KendoGrid<SugerenciaCupoDto>(request, lista);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult Aceptar(List<SugerenciaCupoDto> ids)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (ids != null)
            {
                resultado = cupoManager.AceptarSugerenciaCupo(ids);
            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Sugerencia."));
                resultado.Add(error);
            }
            return Json(resultado);
        }

        public JsonResult Rechazar(List<int> ids,string motivo)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (ids != null)
            {
                cupoManager.RechazarSugerenciaCupo(ids, motivo);

            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Sugerencia."));
                resultado.Add(error);
            }
            return Json(resultado);
        }
    }
}