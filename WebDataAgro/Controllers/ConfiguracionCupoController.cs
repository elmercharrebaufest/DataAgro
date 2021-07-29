using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ConfiguracionCupoController : Controller
    {
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IZonaCupoManager zonaCupoManager;
        private readonly IProveedorManager proveedorManager;
        private readonly IConfiguracionCupoManager configuracionCupoManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ConfiguracionCupoController(ICentroManager centroManager, IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, IProveedorManager proveedorManager, IConfiguracionCupoManager configuracionCupoManager)
        {
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.zonaCupoManager = zonaCupoManager;
            this.proveedorManager = proveedorManager;
            this.configuracionCupoManager = configuracionCupoManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        [Autorizacion(PermisosDataAgro.AdministracionCupos)]
        public ActionResult Index()
        {

            ViewBag.comercialId = GlobalVariables.ComercialId;
            CargarViewBag();
            return View();
        }
        public ActionResult TablaCuposPartial()
        {
            var model = new ConfiguracionCupoModel { Resultado = new Resultado() };
            return PartialView("_ListaCupo", model);
        }
        [HttpPost]
        public ActionResult GrabarCupos(ConfiguracionCupoModel cupo)
        {
            CargarViewBag();
            if (cupo.FechaHasta < cupo.Fecha)
            {
                ModelState.AddModelError("", "La Fecha Hasta no puede ser menor a la fecha desde");
            }
            if (!ModelState.IsValid)
            {
                return View("Index", cupo);
            }
            var dias = new List<DiaCupo>() { };
            for (var dt = cupo.Fecha; dt <= cupo.FechaHasta; dt = dt.AddDays(1))
            {
                dias.Add(new DiaCupo { Cantidad = cupo.CantidadCupo, Fecha = dt });
            }
            //ok
            var resultado = configuracionCupoManager.GrabarConfiguracionCupo(new ConfiguracionCupo
            {
                Id = cupo.Id,
                CentroId = cupo.CentroId,
                MaterialId = cupo.MaterialId,
                Fecha = cupo.Fecha,
                LimiteCupo = cupo.CantidadCupo,
                CierreCupera = cupo.CierreCupera
            }, dias);
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
            var model = configuracionCupoManager.TraerTodaConfiguracionCupo(request);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        private void CargarViewBag()
        {
            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;
            var centro = centroManager.TraerTodoCentro();
            var centroListItems = centro.Centro.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = x.CodigoSap == "1029" ? true : false
                    }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;
        }
        public ActionResult TraerZonaCupo()
        {
            var model = zonaCupoManager.TraerTodoZonaCupo();
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult TraerLimitesCupo(int id)
        {
            var model = configuracionCupoManager.TraerLimites(id);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult GrabarLimitesCupo(List<LimiteCupo> limites )
        {
            var model = configuracionCupoManager.GrabarLimites(limites);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult GrabarLimitesCupoMasivo(List<LimiteCupo> limites, List<int> configuracionesIds)
        {
            var model = configuracionCupoManager.GrabarLimitesMasivo(limites, configuracionesIds);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult EditarConfiguracionCupo(int id)
        {
            var cupo = configuracionCupoManager.TraerConfiguracionCupo(id);
            return new JsonResult() { Data = cupo, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult CambioMasivo(List<int> ids, bool aceptar)
        {
            var resultado = configuracionCupoManager.CambioMasivo(ids, aceptar);
            return new JsonResult() { Data = resultado, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}