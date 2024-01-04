using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.DynamicLinq;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ResearchMapaController : Controller
    {
        private readonly IResearchManager researchManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ResearchMapaController(IResearchManager researchManager)
        {
            this.researchManager = researchManager;
        }

        // GET: ResearchMapa
        [Autorizacion(PermisosDataAgro.DatosResearch)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field= "Material",Dir="desc" }
                };
            }

            DataSourceResult model = researchManager.BuscaDatosTabla(filtro);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}