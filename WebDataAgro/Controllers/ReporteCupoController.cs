using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ReporteCupoController : Controller
    {
        private readonly ICupoManager cupoManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ReporteCupoController(ICupoManager cupoManager)
        {
            this.cupoManager = cupoManager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteCupo)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            var equipo = GlobalVariables.EquipoReal;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            return Json(model);
        }
    }
}
