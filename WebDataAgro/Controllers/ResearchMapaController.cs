using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        // GET: ResearchMapa
        [Autorizacion(PermisosDataAgro.DatosResearch)]
        public ActionResult Index()
        {
            return View();
        }
    }
}