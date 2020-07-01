using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using WebDataAgro.Models;
using Molinos.DataAgro.Entities.Extensions;
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
    public class HabilitacionCupoController : Controller
    {
        private readonly IZonaCupoManager zonaCupoManager;
        private readonly IHabilitacionCupoManager habilitacionManager;
        private readonly IMaterialManager materialManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public HabilitacionCupoController(IHabilitacionCupoManager habilitacionManager, IZonaCupoManager zonaCupoManager, IMaterialManager materialManager)
        {
           this.habilitacionManager = habilitacionManager;
            this.zonaCupoManager = zonaCupoManager;
            this.materialManager = materialManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        [Autorizacion(PermisosDataAgro.HabilitacionDeCupos)]
        public ActionResult Index()
        {

            ViewBag.comercialId = GlobalVariables.ComercialId;
            CargarViewBag();
            return View(new HabilitacionCupoModel());
        }
        public ActionResult TablaCuposPartial()
        {
            var model = new HabilitacionCupoModel { Resultado = new Resultado() };
            return PartialView("_ListaCupo", model);
        }
        [HttpPost]
        public ActionResult GrabarHabilitacion(HabilitacionCupoModel cupo)
        {
            CargarViewBag();
            if (!ModelState.IsValid)
            {
                return View("Index", cupo);
            }
            var resultado = habilitacionManager.GrabarHabilitacionCupo(new HabilitacionCupo
            {
                Id = cupo.Id,
                FechaDesde = cupo.FechaDesde,
                FechaHasta = cupo.FechaHasta,
                ZonaCupoId = cupo.ZonaCupoId ?? null,
                MaterialId = cupo.MaterialId
                
            });
            if (resultado.HayError)
            {
                foreach (var e in resultado.Errores)
                {
                    ModelState.AddModelError("400", e.Message);
                }
            return View("Index", cupo);
            }
            return RedirectToAction("Index");
        }
        public ActionResult DatosConfiguracion(KendoGridMvcRequest request)
        {
            var model = habilitacionManager.TraerTodaHabilitacionCupo(request);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        private void CargarViewBag()
        {
            var zona = zonaCupoManager.TraerTodoZonaCupo();
            var zonasListItems = zona.ZonaCupo.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Zona = zonasListItems;

            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

        }
        public ActionResult TraerZonaCupo()
        {
            var model = zonaCupoManager.TraerTodoZonaCupo();
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult EditarHabilitacionCupo(int id)
        {
            var cupo = habilitacionManager.TraerHabilitacionCupo(id);           
            return new JsonResult() { Data = cupo, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult EliminarHabilitacionCupo(int id)
        {
            var cupo = habilitacionManager.EliminarHabilitacionCupo(id);
            return new JsonResult() { Data = cupo, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }    
}