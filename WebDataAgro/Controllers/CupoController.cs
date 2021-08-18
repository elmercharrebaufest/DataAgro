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
        private readonly IHabilitacionCupoManager habilitacionManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public CupoController(ICentroManager centroManager, IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, IProveedorManager proveedorManager, ICupoManager cupoManager, IComercialManager comercialManager, IHabilitacionCupoManager habilitacionManager)
        {
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.zonaCupoManager = zonaCupoManager;
            this.proveedorManager = proveedorManager;
            this.cupoManager = cupoManager;
            this.comercialManager = comercialManager;
            this.habilitacionManager = habilitacionManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        [Autorizacion(PermisosDataAgro.VisualizarCupos)]
        public ActionResult Index()
        {
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            ViewBag.comercialId = GlobalVariables.ComercialId;
            ViewBag.mostrarMaterial = habilitacionManager.HayMaterialDisponibleExterno(comercialManager.TraerZonaDelComercialAsociado());

            return View();
        }

        [Autorizacion(PermisosDataAgro.AltaCupos, PermisosDataAgro.AltaCupo_Externo)]
        public ActionResult CrearCupo(int? id, string siguientes)
        {
            ViewBag.mostrarMaterial = habilitacionManager.HayMaterialDisponibleExterno(comercialManager.TraerZonaDelComercialAsociado());
            CargarViewBag();
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                return RedirectToAction("CrearCupoTercero");
            }
            if (id == null)
            {
                ViewBag.Titulo = "Nuevo Cupo";
                int zona;
                int.TryParse(ViewBag.ZonaSeleccionada, out zona);
                return View(new CupoModel() { CantidadCupos = null, MaterialId = 3, ZonaId = zona, FechaEntrega = DateTime.Now.Date, FechaHastaEntrega = DateTime.Now.Date, CalidadId = 2 });
            }
            else
            {
                var cupo = cupoManager.ObtenerCupo(id.Value, null);
                var cupoModel = new CupoModel
                {
                    Id = cupo.Id,
                    CalidadId = cupo.Calidad == "Camara" ? 1 : cupo.Calidad == "Fabrica" ? 2 : 0,
                    CantidadCupos = null,
                    FasonId = cupo.Fason ?? false,
                    FechaEntrega = cupo.FechaIngreso,
                    FechaHastaEntrega = cupo.FechaIngreso,
                    FleteAcarreo = cupo.FleteProcedencia ?? false,
                    MaterialId = cupo.MaterialId,
                    ProveedorDescripcion = cupo.Proveedor,
                    Proveedor = cupo.ProveedorId,
                    Observacion = cupo.Observaciones,
                    ZonaId = cupo.ZonaCupoId,
                    PlantaId = cupo.CentroCodigo,
                    CuitId = cupo.Destinatario,
                    Siguientes = siguientes,
                    NegocioId = cupo.NegocioId
                };
                ViewBag.Titulo = "Código Cupo " + cupo.CupoSap;
                return View(cupoModel);
            }
        }

        [Autorizacion(PermisosDataAgro.AltaCupo_Externo)]
        public ActionResult CrearCupoTercero(int? id, string siguientes)
        {
            CargarViewBag();
            if (id == null)
            {
                ViewBag.Titulo = "Nuevo Cupo";
                var proveedorId = proveedorManager.ObtenerIdProveedorPorCuit(PermisosHelper.ObtenerCuit());
                return View(new CupoModel() { Proveedor = proveedorId, CantidadCupos = null, MaterialId = 3, ZonaId = comercialManager.TraerZonaDelComercialAsociado(), FechaEntrega = DateTime.Now.Date, FechaHastaEntrega = DateTime.Now.Date, CalidadId = 2 });
            }
            else
            {
                var cupo = cupoManager.ObtenerCupo(id.Value, null);
                var cupoModel = new CupoModel
                {
                    Id = cupo.Id,
                    CalidadId = cupo.Calidad == "Camara" ? 1 : cupo.Calidad == "Fabrica" ? 2 : 0,
                    CantidadCupos = null,
                    FasonId = cupo.Fason ?? false,
                    FechaEntrega = cupo.FechaIngreso,
                    FechaHastaEntrega = cupo.FechaIngreso,
                    FleteAcarreo = cupo.FleteProcedencia ?? false,
                    MaterialId = cupo.MaterialId,
                    Proveedor = cupo.ProveedorId,
                    Observacion = cupo.Observaciones,
                    PlantaId = cupo.CentroCodigo,
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
            if (cupo.NegocioId == 0)
            {
                cupo.NegocioId = null;
            }
            if (cupo.Negocio == 0)
            {
                cupo.Negocio = null;
            }
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
            else if (ViewData.ModelState.IsValid)
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
                if (!modificado)
                {
                    ViewBag.Titulo = "Nuevo Cupo";
                }
                else
                {
                    ViewBag.Titulo = "Código Cupo " + cupoManager.ObtenerCodigoSap(cupo.Id);
                    cupo.FechaHastaEntrega = cupo.FechaEntrega;
                }
                CargarViewBag();

                if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                {
                    if (ViewData.ModelState.IsValid)
                    {
                        return RedirectToAction("Index");
                    }
                    return View("CrearCupoTercero", cupo);

                }
                return View(cupo);
            }
            if (siguientes != null && siguientes.Count != 0)
            {
                var id = siguientes[0];
                siguientes.RemoveAt(0);
                if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                {
                    return RedirectToAction("CrearCupoTercero", new { id, siguientes = JsonConvert.SerializeObject(siguientes) });
                }
                return RedirectToAction("CrearCupo", new { id, siguientes = JsonConvert.SerializeObject(siguientes) });
            }
            return RedirectToAction("Index");
        }

        public JsonResult BuscarProveedor(string filtroProveedor)
        {
            return Json(proveedorManager.DevolverProveedoresCorredores(filtroProveedor), JsonRequestBehavior.AllowGet);
        }
        private void CargarViewBag()
        {
            var centros = centroManager.TraerTodoCentro();
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                centros.Centro = centros.Centro.Where(x => x.CodigoSap == "1029" || x.CodigoSap == "1600").ToList();
            }
            var listaCentro = new List<SelectListItem>();
            foreach (var i in centros.Centro)
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == "1029" ? true : false
                });
            }
            ViewBag.Centro = listaCentro.OrderBy(x => x.Value);

            var material = materialManager.TraerTodoMaterial();
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                var materialExterno = habilitacionManager.TraerTodoMaterialRetirado(comercialManager.TraerZonaDelComercialAsociado());
                material.Material = material.Material.Where(x => materialExterno.Where(y => y.Descripcion == "Disponible".ToUpper()).Any(y => y.Id == x.MaterialId)).ToList();

            }

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
            var listaZona = new List<SelectListItem>() { new SelectListItem { Value = "0", Text = "Seleccione Zona", Selected = false } };
            var comercial = comercialManager.TraerComercial(GlobalVariables.ComercialId);
            foreach (var i in zona.ZonaCupo)
            {
                listaZona.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.Id.ToString(),
                    Selected = comercial.GrupoDeComprasId != null && comercial.GrupoDeCompras.ToLower() == i.Descripcion.ToLower() ? true : false
                });
            }
            ViewBag.Zona = listaZona;
            ViewBag.ZonaSeleccionada = listaZona.FirstOrDefault(x => comercial.GrupoDeComprasId != null && comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()) != null ? listaZona.FirstOrDefault(x => comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()).Value : "0";

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
                CentroId = centroManager.ObtenerCentroPorCodigoSap(cupo.PlantaId).Id,
                FleteProcedencia = cupo.FleteAcarreo,
                Calidad = cupo.CalidadId == 1 ? "Camara" : cupo.CalidadId == 2 ? "Fabrica" : "",
                Observaciones = cupo.Observacion,
                Fason = cupo.FasonId,
                Destinatario = cupo.CuitId ?? "30715118773",
                ComercialId = GlobalVariables.ComercialId,
                FechaGeneracion = DateTime.Now,
                NegocioId = cupo.Negocio,
                ZonaCupoId = cupo.ZonaId
            };
            if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                entidad.ComercialId = comercialManager.ComercialAsociado(cupo.Proveedor);
            }
            else
            {
                entidad.ComercialId = GlobalVariables.ComercialId;
            }
            return entidad;
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort> {
                    new Sort {Field= "FechaIngreso",Dir="desc" },
                    new Sort { Field="Material",Dir="desc" } };
            }
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosCupos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = cupoManager.TraerCuposTabla(request, equipo);
            return Json(model);
        }

        [Autorizacion(PermisosDataAgro.AnularCupos)]
        public ActionResult EliminarCupo(int id)
        {
            var model = cupoManager.EliminarCupo(id, GlobalVariables.IdActiveDirectory, true);
            return Json(model);
        }
        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = proveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = !string.IsNullOrEmpty(x.Alias) ? (x.Alias + " - " + x.RazonSocial) : x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProveedorTodos(string text = "")
        {
            var proveedores = proveedorManager.ListarProveedorTodos(text);
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = !string.IsNullOrEmpty(x.Alias) ? (x.Alias + " - " + x.RazonSocial) : x.RazonSocial }), JsonRequestBehavior.AllowGet);
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


        [Autorizacion(PermisosDataAgro.DisponibilidadDeCupos)]
        public ActionResult Disponibilidad()
        {
            FillViewBag();
            return View();
        }

        private void FillViewBag()
        {
            var comercial = comercialManager.TraerComercial(GlobalVariables.ComercialId);

            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Codigo.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;
            var centro = centroManager.TraerTodoCentro();
            var centroListItems = centro.Centro.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.CodigoSap.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;

            var zonas = zonaCupoManager.TraerTodoZonaCupo();
            var zonaListItems = zonas.ZonaCupo.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.CodigoSap.ToString(),
                    Selected = comercial.GrupoDeCompras == x.Descripcion
                }).OrderBy(x => x.Value);
            ViewBag.Zona = zonaListItems;
            var zona = zonas.ZonaCupo.Where(a => a.Descripcion == comercial.GrupoDeCompras).SingleOrDefault();
            if (zona != null)
            {
                ViewBag.GrupoDeCompras = zona.CodigoSap;
            }
        }

        [HttpPost]
        public ActionResult BuscaDatosTablaDisponibilidad(string FechaDesde, string FechaHasta, string ZonaId, List<string> CentroId, string MaterialId)
        {
            DateTime fechaDesde = DateTime.Now.Date;
            if (!String.IsNullOrEmpty(FechaDesde))
            {
                DateTime.TryParseExact(FechaDesde, "dd-MM-yyyy", new CultureInfo("es-AR"), DateTimeStyles.AdjustToUniversal, out fechaDesde);

            }
            DateTime fechaHasta = DateTime.Now.Date;
            if (!String.IsNullOrEmpty(FechaHasta))
            {
                DateTime.TryParseExact(FechaHasta, "dd-MM-yyyy", new CultureInfo("es-AR"), DateTimeStyles.AdjustToUniversal, out fechaHasta);

            }
            List<DisponibilidadCuposDto> model = cupoManager.TraerCupoDisponibilidad(fechaDesde, fechaHasta, ZonaId, CentroId, MaterialId);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult TraerTodasHabilitacionesActivas()
        {
            return new JsonResult()
            {
                Data = habilitacionManager.TraerTodasHabilitacionesActivas(comercialManager.TraerZonaDelComercialAsociado()),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerTodoMaterialRetirado()
        {
            return new JsonResult()
            {
                Data = habilitacionManager.TraerTodoMaterialRetirado(comercialManager.TraerZonaDelComercialAsociado()),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult RechazarCupo(Cupo cupo)
        {
            var model = cupoManager.RechazarCupo(cupo, GlobalVariables.IdActiveDirectory);
            return Json(model);
        }

        public ActionResult AceptarCupo(Cupo cupo)
        {
            var model = cupoManager.AceptarCupo(cupo);
            return Json(model);
        }

        public JsonResult ListarCupo(string text = "")
        {
            var cupo = cupoManager.ListarCupo(text);
            return Json(cupo.Select(x => new { Id = x.Id, CupoSap = x.CupoSap }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult TraerNegocioConCupoDisponible(string cuitProveedor, int materialId, int centro, string filtro, DateTime desde, DateTime hasta)
        {
            return new JsonResult()
            {
                Data = cupoManager.TraerNegocioConCupoDisponible(cuitProveedor, materialId, centro, filtro, desde, hasta),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerEstablecimientos(string cuitProveedor)
        {
            return new JsonResult()
            {
                Data = cupoManager.TraerEstablecimientos(cuitProveedor),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}