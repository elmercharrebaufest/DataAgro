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
    public class ReportePrecioMoaPizarraController : Controller
    {
        private IReportesManager reportesManager;

        public ReportePrecioMoaPizarraController(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarReportePrecioMoaPizarra)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
           if (request.Sort == null)
            {
                request.Sort = new List<Sort> { new Sort { Field = "HastaVigencia", Dir = "desc" }, new Sort { Field = "TipoConfiguracion", Dir = "desc" } };
            }
            var model = reportesManager.TraerTodoPrecioMoaPizarra(request);
            return Json(model);
        }

    }
}