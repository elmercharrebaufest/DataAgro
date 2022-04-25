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
using System.Configuration;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CupoNoPropioController : Controller
    {
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly ICupoNoPropioManager cupoNoPropioManager;
        private readonly IComercialManager comercialManager;
        private readonly IHabilitacionCupoManager habilitacionManager;

        public CupoNoPropioController(ICentroManager centroManager, IMaterialManager materialManager, ICupoNoPropioManager cupoManager, IComercialManager comercialManager, IHabilitacionCupoManager habilitacionManager)
        {
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.cupoNoPropioManager = cupoManager;
            this.habilitacionManager = habilitacionManager;
            this.comercialManager = comercialManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        public ActionResult Index()
        {
            CargarViewBag();
            return View(new CupoModel());
        }
       
        public ActionResult CrearCupo(CupoDto cupoDto)
        {
            var result = cupoNoPropioManager.GrabarCupoNoPropio(cupoDto);
            return Json(result);
        }     
       
        private void CargarViewBag()
        {
            var centros = centroManager.TraerTodoCentro().Centro.Where(x => x.CargaCupos == true && x.NoPropio == true);            
            var listaCentro = centros.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.CodigoSap.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);

            var listaCentroFiltro = centros.Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.Id.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);

            ViewBag.Centro = listaCentro.OrderBy(x => x.Value);
            ViewBag.CentroFiltro = listaCentroFiltro.OrderBy(x => x.Value);

            var material = materialManager.TraerTodoMaterial();

            var listaMaterial = new List<SelectListItem>();
            foreach (var i in material.Material)
            {
                listaMaterial.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.MaterialId.ToString(),
                    Selected = i.MaterialId == 3 ? true : false
                });
            }
            ViewBag.Material = listaMaterial.OrderBy(x => x.Value);


        }

        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort> {
                    new Sort {Field= "FechaIngreso", Dir="desc" },
                    new Sort {Field="Material", Dir="desc" } };
            }
            if (request.Filter != null)
            {
                foreach (var item in request.Filter.Filters)
                {
                    if (item.Field == "Utilizado")
                    {
                        if (item.Value.ToString() == "1")
                        {
                            item.Value = true;
                        }
                        if (item.Value.ToString() == "0")
                        {
                            item.Value = false;
                        }
                    }
                    if (item.Field == "Disponible")
                    {
                        if (item.Value.ToString() == "1")
                        {
                            item.Value = true;
                        }
                        if (item.Value.ToString() == "0")
                        {
                            item.Value = false;
                        }
                    }
                }
            }
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosCupos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = cupoNoPropioManager.TraerCuposNoPropioTabla(request, equipo);
            return Json(model);
        }

        public ActionResult ModificarDisponible(int id, bool disponible)
        {
            var resultado = cupoNoPropioManager.GrabarDisponibilidadCupoNoPropio(id, disponible);
            return new JsonResult() { Data = resultado, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}