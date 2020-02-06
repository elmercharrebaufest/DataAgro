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
    public class ConfiguracionEspacioDinamicoController : Controller
    {
        private readonly IComercialManager comercialManager;
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IProveedorManager proveedorManager;
        private readonly IConfiguracionEspacioDinamicoManager configuracionEspacioDinamicoManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ConfiguracionEspacioDinamicoController(IComercialManager comercialManager, ICentroManager centroManager, IMaterialManager materialManager, IProveedorManager proveedorManager, IConfiguracionEspacioDinamicoManager configuracionEspacioDinamicoManager)
        {
            this.comercialManager = comercialManager;
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.proveedorManager = proveedorManager;
            this.configuracionEspacioDinamicoManager = configuracionEspacioDinamicoManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        [Autorizacion(PermisosDataAgro.AdministracionEspacioDinamico)]
        public ActionResult Index()
        {

            ViewBag.comercialId = GlobalVariables.ComercialId;
            CargarViewBag();
            return View();
        }
        public ActionResult TablaConfiguracionEspacioDinamico()
        {
            var model = new ConfiguracionEspacioDinamicoModel { Resultado = new Resultado() };
            return PartialView("_ListaConfiguracionEspacioDinamico", model);
        }

        [HttpPost]
        public ActionResult Grabar(ConfiguracionEspacioDinamicoModel espacioDinamico)
        {
            CargarViewBag();
            if (!ModelState.IsValid)
            {
                return View("Index", espacioDinamico);
            }
            var resultado = configuracionEspacioDinamicoManager.GrabarConfiguracionEspacioDinamico(new ConfiguracionEspacioDinamico
            {
                Id = espacioDinamico.Id,
                CentroId = espacioDinamico.CentroId,
                MaterialId = espacioDinamico.MaterialId,
                Fecha = espacioDinamico.Fecha,
                ProveedorId = espacioDinamico.ProveedorId,
                ComercialId = espacioDinamico.ComercialId,
                CantidadDeCupo = espacioDinamico.CantidadCupo,
                Calidad = espacioDinamico.CalidadId == 1 ? "Camara" : espacioDinamico.CalidadId == 2 ? "Fabrica" : "",
            });
            if (resultado.HayError)
            {
                foreach (var e in resultado.Errores)
                {
                    ModelState.AddModelError("400", e.Message);
                }
                return View("Index", espacioDinamico);
            }
            return RedirectToAction("Index");
        }
        public ActionResult DatosConfiguracion(KendoGridMvcRequest request)
        {
            var model = configuracionEspacioDinamicoManager.TraerTodaConfiguracionEspacioDinamico(request);
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
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var comerciales = comercialManager.TraerTodoComercial();
            var comercialesListItems = comerciales.Comercial.Select(
                    x => new SelectListItem
                    {
                        Text = x.Nombres + " " + x.Apellido,
                        Value = x.ComercialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Comercial = comercialesListItems;
            ViewBag.Calidad = new List<SelectListItem>() { new SelectListItem { Text = "Camara", Value = "1",Selected =false},
                new SelectListItem { Text = "Fabrica", Value = "2",Selected =true } };
        }

        public ActionResult Eliminar(int id)
        {
            var espacioDinamico = configuracionEspacioDinamicoManager.EliminarConfiguracionEspacioDinamico(id);
            return new JsonResult() { Data = espacioDinamico, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}