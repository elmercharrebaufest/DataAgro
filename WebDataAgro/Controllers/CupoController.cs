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
        private readonly IComercialManager comercialManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public CupoController(ICentroManager centroManager, IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, IProveedorManager proveedorManager, ICupoManager cupoManager, IComercialManager comercialManager)
        {
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.zonaCupoManager = zonaCupoManager;
            this.proveedorManager = proveedorManager;
            this.cupoManager = cupoManager;
            this.comercialManager = comercialManager;
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
            int zona;
            int.TryParse(ViewBag.ZonaSeleccionada, out zona);
            return View(new CupoModel() { ZonaId = zona, FechaEntrega = DateTime.Now.Date});
        }
        
        [HttpPost]
        public ActionResult CrearCupo(CupoModel cupo)
        {
            var cupoGrabado = cupoManager.GrabarCupo(TransformarAEntidad(cupo), cupo.CantidadCupos);
            if (cupoGrabado.HayError)
            {
                foreach (var e in cupoGrabado.Errores)
                {
                    ModelState.AddModelError(e.ErrorCode.ToString(), e.Message);
                }
            }
            if (!ModelState.IsValid)
            {
                CargarViewBag();
                return View(cupo);
            }
            return RedirectToAction("Index");            
        }
        public JsonResult BuscarProveedor(string filtroProveedor)
        {
            return Json(proveedorManager.DevolverProveedores(filtroProveedor, 2, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
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
                    Selected = i.CodigoSap == "1029" ? true : false
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
            var listaZona = new List<SelectListItem>() { new SelectListItem { Value= "0", Text= "Seleccione Zona",Selected= false} };
            var comercial = comercialManager.TraerComercial(GlobalVariables.ComercialId);
            foreach (var i in zona.ZonaCupo)
            {
                listaZona.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.Id.ToString(),
                    Selected = comercial.GrupoDeCompras.ToLower() == i.Descripcion.ToLower() ? true : false
                });
            }
            ViewBag.Zona = listaZona;
            ViewBag.ZonaSeleccionada = listaZona.FirstOrDefault(x => comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()) != null ? listaZona.FirstOrDefault(x => comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()).Value : "0";
            
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
                FleteProcedencia = cupo.FleteAcarreo,
                ZonaCupoId = cupo.ZonaId,
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