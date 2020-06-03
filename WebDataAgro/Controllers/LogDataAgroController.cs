using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.LogDataAgro)]
    public class LogDataAgroController : Controller
    {
        private readonly ILogDataAgroManager logDataAgroManager;

        public LogDataAgroController(ILogDataAgroManager logDataAgroManager)
        {
            this.logDataAgroManager = logDataAgroManager;
        }

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BuscarDatosLogDataAgro(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort> {
                    new Sort {Field= "Fecha",Dir="desc" }
                    //new Sort { Field="Material",Dir="desc" } };
                    };
            }

            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = logDataAgroManager.ListarDatosLogDataAgro(request, equipo);
            //var asd = new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            return Json(model);
            ///*return asd*/;
        }

    }
}