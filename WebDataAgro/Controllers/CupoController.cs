using Autofac.Extras.NLog;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class CupoController : Controller
    {
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IZonaCupoManager zonaCupoManager;
        private readonly IProveedorManager proveedorManager;
        private readonly ICupoManager cupoManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public CupoController(ICentroManager centroManager, IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, IProveedorManager proveedorManager, ICupoManager cupoManager)
        {
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.zonaCupoManager = zonaCupoManager;
            this.proveedorManager = proveedorManager;
            this.cupoManager = cupoManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        public ActionResult Index()
        {
            ViewBag.perfil = GlobalVariables.Perfil.DisplayEnum();
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            ViewBag.comercialId = GlobalVariables.ComercialId;
            return View();
        }
        public ActionResult CrearCupo()
        {
            CargarViewBag();
            return View(new CupoModel());
        }
        
        [HttpPost]
        public ActionResult CrearCupo(CupoModel cupo)
        {
            var cupoGrabado = cupoManager.GrabarCupo(TransformarAEntidad(cupo), cupo.CantidadCupos);
            if (cupoGrabado.HayError)
            {
                foreach(var e in cupoGrabado.Errores)
                {
                    ModelState.AddModelError(e.ErrorCode.ToString(), e.Message);
                }
                CargarViewBag();
                return View(cupo);
            }
            return RedirectToAction("Index");            
        }
        public JsonResult BuscarProveedor(string filtroProveedor)
        {
            return Json(proveedorManager.DevolverProveedores(filtroProveedor, false, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
        }
        private void CargarViewBag() {
            var centros = centroManager.TraerTodoCentro();
            var listaCentro = new List<SelectListItem>();
            foreach (var i in centros.Centro)
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.Id.ToString(),
                    Selected = false
                });
            }
            ViewBag.Centro = listaCentro.OrderBy(x => x.Value);
            var material = materialManager.TraerTodoMaterial();
            var listaMaterial = new List<SelectListItem>();
            foreach (var i in material.Material)
            {
                listaMaterial.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.MaterialId.ToString(),
                    Selected = false
                });
            }
            ViewBag.Material = listaMaterial.OrderBy(x => x.Value);
            var zona = zonaCupoManager.TraerTodoZonaCupo();
            var listaZona = new List<SelectListItem>();
            foreach (var i in zona.ZonaCupo)
            {
                listaZona.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.Id.ToString(),
                    Selected = false
                });
            }
            ViewBag.Zona = listaZona.OrderBy(x => x.Value);
            ViewBag.Flete = new List<SelectListItem>() { new SelectListItem { Text = "Si", Value = "1",Selected =false},
                new SelectListItem { Text = "No", Value = "2",Selected =false } };
            ViewBag.Calidad = new List<SelectListItem>() { new SelectListItem { Text = "Camara", Value = "1",Selected =false},
                new SelectListItem { Text = "Fabrica", Value = "2",Selected =false } };
        }
        private Cupo TransformarAEntidad(CupoModel cupo)
        {
            var entidad = new Cupo
            {
                ProveedorId = cupo.Proveedor,
                MaterialId = cupo.Material,
                FechaIngreso = cupo.FechaEntrega,
                CentroId = cupo.Planta,
                FleteProcedencia = cupo.FleteAcarreo == 1 ? true : false,
                ZonaCupoId = cupo.Zona,
                Calidad = cupo.Calidad == 1 ? "Camara" : cupo.Calidad == 2 ? "Fabrica" : "",
                Observaciones = cupo.Observacion,
                Fason = cupo.Fason,
                Destinatario = cupo.CUIT,
                ComercialId = GlobalVariables.ComercialId,
                FechaGeneracion = DateTime.Now
            };
            return entidad;
        }
        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            var equipo = GlobalVariables.EquipoReal;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            return Json(model);
        }
        public ActionResult EliminarCupo(int id)
        {
            var model = cupoManager.EliminarCupo(id);
            return Json(model);
        }
    }
}