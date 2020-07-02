using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;
using Kendo.DynamicLinq;
using Filter = Kendo.DynamicLinq.Filter;
using KendoGridBinder.Containers;
using System.Collections;
using System.Globalization;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteRangoConfirmacionAutomaticaController : Controller
    {
        private readonly IRangoConfirmacionAutomaticaManager rangoManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ReporteRangoConfirmacionAutomaticaController(IRangoConfirmacionAutomaticaManager rangoManager)
        {
            this.rangoManager = rangoManager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteRango)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
           if (request.Sort == null)
            {
                request.Sort = new List<Sort> { new Sort { Field = "Id", Dir = "desc" } };
            }
            var model = rangoManager.TraerTodoRango(request);
            return Json(model);
        }

    }
}