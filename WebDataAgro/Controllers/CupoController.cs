
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

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.VisualizarCupos)]
        public ActionResult Index()
        {          
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            ViewBag.comercialId = GlobalVariables.ComercialId;
            return View();
        }

        [Autorizacion(PermisosDataAgro.AltaCupos)]
        public ActionResult CrearCupo(int? id,string siguientes)
        {
            CargarViewBag();
            if (id == null)
            {
                ViewBag.Titulo = "Nuevo Cupo";
                int zona;
                int.TryParse(ViewBag.ZonaSeleccionada, out zona);
                return View(new CupoModel() { CantidadCupos = null, MaterialId = 3, ZonaId = zona, FechaEntrega = DateTime.Now.Date, FechaHastaEntrega = DateTime.Now.Date, CalidadId = 2 });
            }
            else
            {
                var cupo = cupoManager.ObtenerCupo(id.Value);
                var cupoModel = new CupoModel
                {
                    Id = cupo.Id,
                    CalidadId= cupo.Calidad == "Camara" ? 1 : cupo.Calidad == "Fabrica" ? 2 : 0,
                    CantidadCupos = null,
                    FasonId = cupo.Fason ?? false,
                    FechaEntrega = cupo.FechaIngreso,
                    FechaHastaEntrega = cupo.FechaIngreso,
                    FleteAcarreo = cupo.FleteProcedencia?? false,
                    MaterialId = cupo.MaterialId,
                    ProveedorDescripcion = cupo.Proveedor,
                    Proveedor = cupo.ProveedorId,
                    Observacion = cupo.Observaciones,
                    ZonaId = cupo.ZonaCupoId,
                    PlantaId = cupo.CentroId,
                    CuitId = cupo.Destinatario,
                    Siguientes = siguientes
                };
                ViewBag.Titulo = "Código Cupo " + cupo.CupoSap;
                return View(cupoModel);
            }
        }
        
        [HttpPost]
        public ActionResult CrearCupo(CupoModel cupo)
        {
            var modificado = cupo.Id != 0;
            cupo.CantidadCupos = cupo.CantidadCupos
                                 != null ? cupo.CantidadCupos : 0;
            var cupoNuevo = TransformarAEntidad(cupo);
            var error = cupoManager.Validar(cupoNuevo, cupo.CantidadCupos.Value, cupo.FechaHastaEntrega);
            if (error.HayError)
            {
                foreach (var e in error.Errores)
                {
                    if (ViewData.ModelState["Proveedor"].Errors.Count == 0 || ViewData.ModelState["Proveedor"].Errors.Any(x => x.ErrorMessage != e.Message))
                    {
                        ModelState.AddModelError("Error", e.Message);
                    }
                }
            }
            else
            {
                var cupoGrabado = cupoManager.GrabarCupo(cupoNuevo, cupo.Dias);
                if (cupoGrabado.HayError)
                {
                    foreach (var e in cupoGrabado.Errores)
                    {
                        if (ViewData.ModelState["Proveedor"].Errors.Count == 0 || ViewData.ModelState["Proveedor"].Errors.Any(x => x.ErrorMessage != e.Message))
                        {
                            ModelState.AddModelError(e.Source, e.Message);
                        }
                    }
                }
                cupo.Resultado = cupoGrabado;
            }
            var siguientes = JsonConvert.DeserializeObject<List<int>>(cupo.Siguientes ?? ""); 
            if (!ViewData.ModelState.IsValid || !modificado)
            {   
                if(!modificado)
                {
                    ViewBag.Titulo="Nuevo Cupo";
                }
                else
                {
                    ViewBag.Titulo = "Código Cupo " + cupoManager.ObtenerCodigoSap(cupo.Id);
                    cupo.FechaHastaEntrega = cupo.FechaEntrega;
                }
                CargarViewBag();
                return View(cupo);
            }
            if (siguientes != null && siguientes.Count != 0 )
            {
                var id = siguientes[0];
                siguientes.RemoveAt(0);
                return RedirectToAction("CrearCupo", new { id, siguientes = JsonConvert.SerializeObject(siguientes) });
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
                    Selected = i.MaterialId == 3 ? true : false
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
                new SelectListItem { Text = "Fabrica", Value = "2",Selected =true } };
        }
        private Cupo TransformarAEntidad(CupoModel cupo)
        {
            var entidad = new Cupo
            {
                Id = cupo.Id,
                ProveedorId = cupo.Proveedor,
                MaterialId = cupo.MaterialId,
                FechaIngreso = cupo.FechaEntrega,
                CentroId = cupo.PlantaId,
                FleteProcedencia = cupo.FleteAcarreo,
                ZonaCupoId = cupo.ZonaId,
                Calidad = cupo.CalidadId == 1 ? "Camara" : cupo.CalidadId == 2 ? "Fabrica" : "",
                Observaciones = cupo.Observacion,
                Fason = cupo.FasonId,
                Destinatario = cupo.CuitId ?? "30715118773",
                ComercialId = GlobalVariables.ComercialId,
                FechaGeneracion = DateTime.Now
            };
            return entidad;
        }
        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosCupos)? GlobalVariables.EquipoReal: GlobalVariables.Equipo;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            return Json(model);
        }
        [Autorizacion(PermisosDataAgro.AnularCupos)]
        public ActionResult EliminarCupo(int id)
        {
            var model = cupoManager.EliminarCupo(id, GlobalVariables.IdActiveDirectory);
            return Json(model);
        }
        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = proveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarComercial(string text = "")
        {
            var comerciales = comercialManager.ListarComercial(text, GlobalVariables.Equipo);
            return Json(comerciales.Select(x => new { x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }), JsonRequestBehavior.AllowGet);
        }      
        
        [HttpPost]
        public ActionResult TransmitirCupos(List<string> cupos)
        {
            var resultado = cupoManager.TransmitirCupos(cupos);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EliminarVarios(List<int> listaCupos)
        {
            var resultado = cupoManager.EliminarVarios(listaCupos, GlobalVariables.IdActiveDirectory);
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }
    }
}