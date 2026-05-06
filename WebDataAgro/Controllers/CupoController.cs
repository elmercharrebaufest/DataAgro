using Kendo.DynamicLinq;
using Molinos.DataAgro.Agent.ScatoRepositorio;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
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
        private readonly IHabilitacionCupoManager habilitacionManager;


        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public CupoController(ICentroManager centroManager, IMaterialManager materialManager, IZonaCupoManager zonaCupoManager, IProveedorManager proveedorManager,
            ICupoManager cupoManager, IComercialManager comercialManager, IHabilitacionCupoManager habilitacionManager)
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
            ViewBag.CentrosTodos = centroManager.TraerTodoCentro().Centro.Where(x => x.CargaCupos).Select(x => x.Descripcion).OrderByDescending(x => x).ToList();
            CargarFiltros();
            return View();
        }

        [Autorizacion(PermisosDataAgro.AltaCupos, PermisosDataAgro.AltaCupo_Externo)]
        public ActionResult CrearCupo(int? id, string siguientes)
        {
            ViewBag.mostrarMaterial = habilitacionManager.HayMaterialDisponibleExterno(comercialManager.TraerZonaDelComercialAsociado());
            ViewBag.ActivarSojaEUDR = ConfigurationManager.AppSettings["ActivarSojaEUDR"];
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
                    NegocioId = cupo.NegocioId,
                    ConDescarga = cupo.ConDescarga,
                    Sustentable = cupo.Sustentable,
                    EPA = cupo.EPA,
                    EUDR = cupo.EUDR
                };
                ViewBag.Titulo = "Código Cupo " + cupo.CupoSap;
                var centro = centroManager.TraerCentro(cupo.CentroId);
                cupoModel.NoPropio = centro.NoPropio;
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
            ViewBag.ActivarSojaEUDR = ConfigurationManager.AppSettings["ActivarSojaEUDR"];
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

            if (cupoNuevo.CentroId == 1)
            {
                switch (cupoNuevo.MaterialId)
                {
                    case 1:
                        if (ConfigurationManager.AppSettings["CupoMaizPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de maíz para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 2:
                        if (ConfigurationManager.AppSettings["CupoTrigoPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de trigo para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 3:
                        if (ConfigurationManager.AppSettings["CupoSojaNoSustPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de soja para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 4:
                        if (ConfigurationManager.AppSettings["CupoGirasolPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de girasol para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 5:
                        if (ConfigurationManager.AppSettings["CupoGirasolPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de girasol alto oleico para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 6:
                        if (ConfigurationManager.AppSettings["CupoSorgoPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de sorgo para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                }
            }
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
                //if (cupo.PlantaId == "1074")
                //{
                //    var cupoExterno = TransformarACupoExterno(cupoNuevo);
                //    var cupoexternoResult = cupoExternoManager.GrabarCupoExterno(cupoExterno);
                //    if (cupoexternoResult.HayError)
                //    {
                //        foreach (var e in cupoexternoResult.Errores)
                //        {
                //            ModelState.AddModelError(e.Source, e.Message);
                //        }
                //    }
                //}
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

        [HttpPost]
        public ActionResult AltaMasivaCuposExcel()
        {
            List<string> errores = new List<string>();
            try
            {
                if (Request.Files.Count == 0)
                {
                    errores.Add(string.Concat("Debe seleccionar el archivo."));
                    return Json(new { Resume = errores, Resultado = false });
                }
                if (Request.Files.Count > 1)
                {
                    errores.Add(string.Concat("Debe seleccionar un solo archivo."));
                    return Json(new { Resume = errores, Resultado = false });
                }

                var fileSubido = Request.Files[0];
                var extension = Path.GetExtension(fileSubido.FileName).ToUpper();
                if (extension != ".XLSX")
                {
                    errores.Add(string.Concat("Archivo no soportado. Debe subir un Excel en formato xlsx."));
                    return Json(new { Resume = errores, Resultado = false });
                }

                if (fileSubido.ContentLength > 0)
                {
                    var dsExcel = ExcelImport.LeerExcelDesdeHttpRequest(Request);

                    if (dsExcel != null)
                    {
                        var resultado = cupoManager.AltaMasivaSugerenciaCuposV2(dsExcel);

                        return Json(new { Resume = resultado, Resultado = true });
                    }
                }
                else
                {
                    errores.Add(string.Concat("El archivo ", fileSubido.FileName, " está vacío."));
                }

                if (errores.Count > 0)
                {
                    return Json(new { Resume = errores, Resultado = false });
                }

                return Json(new { data = "" });
            }
            catch (Exception e)
            {
                errores.Add(e.Message);
                return Json(new { Resume = errores, Resultado = false }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GuardarCupo(CupoModel cupo)
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
            cupo.CantidadCupos = cupo.CantidadCupos != null ? cupo.CantidadCupos : 0;
            var cupoNuevo = TransformarAEntidad(cupo);

            int sumaCuposCargaMasiva = 0;
            bool cargaMasiva = cupo.Dias != null && cupo.Dias.Count() > 1;
            if (cargaMasiva && !cupo.Dias.Any(x => x.Cantidad == null)) sumaCuposCargaMasiva = cupo.Dias.Sum(x => (int)x.Cantidad);

            var error = cupoManager.Validar(cupoNuevo, cargaMasiva ? sumaCuposCargaMasiva : cupo.CantidadCupos.Value, cupo.FechaHastaEntrega);

            if (cupoNuevo.CentroId == 1)
            {
                switch (cupoNuevo.MaterialId)
                {
                    case 1:
                        if (ConfigurationManager.AppSettings["CupoMaizPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de maíz para San Lorenzo deben gestionarse en la pantalla “Sugerencia de cupos”."));
                        }
                        break;
                    case 2:
                        if (ConfigurationManager.AppSettings["CupoTrigoPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de trigo para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 3:
                        if (ConfigurationManager.AppSettings["CupoSojaNoSustPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de soja para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 4:
                        if (ConfigurationManager.AppSettings["CupoGirasolPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de girasol para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 5:
                        if (ConfigurationManager.AppSettings["CupoGirasolPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de girasol alto oleico para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                    case 6:
                        if (ConfigurationManager.AppSettings["CupoSorgoPorSugerencias"] == "Si")
                        {
                            if (error.Errores == null) error.Errores = new List<ErrorMessage>();
                            error.Errores.Add(new ErrorMessage("Los cupos de sorgo para San Lorenzo deben gestionarse en la pantalla “Sugerencia de Cupos”."));
                        }
                        break;
                }
            }
            if (!error.HayError)
            {
                var cupoGrabado = cupoManager.GrabarCupo(cupoNuevo, cupo.Dias);
                //if (cupoGrabado.HayError)
                //{
                //    foreach (var e in cupoGrabado.Errores)
                //    {
                //        if (ViewData.ModelState["Proveedor"].Errors.Count == 0 || ViewData.ModelState["Proveedor"].Errors.Any(x => x.ErrorMessage != e.Message))
                //        {
                //            ModelState.AddModelError(e.Source, e.Message);
                //        }
                //    }
                //}
                cupo.Resultado = cupoGrabado;
                error.ListaErrores.AddRange(cupoGrabado.ListaErrores);
            }
            var siguientes = JsonConvert.DeserializeObject<List<int>>(cupo.Siguientes ?? "");
            if (!ViewData.ModelState.IsValid || !modificado)
            {
                if (modificado)
                {
                    cupo.FechaHastaEntrega = cupo.FechaEntrega;
                }
                //CargarViewBag();

                if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                {
                    if (ViewData.ModelState.IsValid)
                    {
                        return Json(new { Result = cupo, Error = error, irA = "/Cupo/" });
                    }
                    return Json(new { Result = cupo, Error = error, irA = "/Cupo/CrearCupoTercero" });

                }
                return Json(new { Result = cupo, Error = error, irA = "" });
            }
            if (siguientes != null && siguientes.Count != 0)
            {
                var id = siguientes[0];
                siguientes.RemoveAt(0);
                if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                {
                    return Json(new { Result = cupo, Error = error, irA = "/Cupo/CrearCupoTercero?id=" + id.ToString() + "&siguientes=" + JsonConvert.SerializeObject(siguientes) });
                }
                return Json(new { Result = cupo, Error = error, irA = "/Cupo/CrearCupo?id=" + id.ToString() + "&siguientes=" + JsonConvert.SerializeObject(siguientes) });
            }
            return Json(new { Result = cupo, Error = error, irA = "/Cupo/" });
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
            centros.Centro = centros.Centro.OrderBy(x => x.Orden).ToList();
            var listaCentro = new List<SelectListItem>();
            foreach (var i in centros.Centro.Where(x => x.CargaCupos == true && x.Orden != null && x.Descripcion.Contains("SUSTENTABLE") == false).OrderBy(y => y.Orden))
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == "1029"
                });
            }
            foreach (var i in centros.Centro.Where(x => x.CargaCupos == true && x.Orden == null && x.Descripcion.Contains("SUSTENTABLE") == false).OrderBy(y => y.Descripcion))
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == "1029"
                });
            }
            ViewBag.Centro = listaCentro;//.OrderBy(x => x.Value);

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
                    Selected = i.MaterialId == (int)EnumMateriales.SOJA
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
                    Selected = comercial.GrupoDeComprasId != null && comercial.GrupoDeCompras.ToLower() == i.Descripcion.ToLower()
                });
            }
            ViewBag.Zona = listaZona;
            ViewBag.ZonaSeleccionada = listaZona.FirstOrDefault(x => comercial.GrupoDeComprasId != null && comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()) != null ? listaZona.FirstOrDefault(x => comercial.GrupoDeCompras.ToLower() == x.Text.ToLower()).Value : "0";

            ViewBag.Calidad = new List<SelectListItem>()
            {
                new SelectListItem { Text = "Camara", Value = "1", Selected = false },
                new SelectListItem { Text = "Fabrica", Value = "2", Selected = true }
            };
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
                ZonaCupoId = cupo.ZonaId,
                ConDescarga = cupo.ConDescarga,
                Sustentable = cupo.Sustentable,
                EPA = cupo.EPA,
                EUDR = cupo.EUDR
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

        private CupoNoPropio TransformarACupoExterno(Cupo externo)
        {
            var entidad = new CupoNoPropio
            {
                Id = externo.Id,
                Codigo = externo.CupoSap,
                MaterialId = externo.MaterialId,
                CentroId = externo.CentroId,
                FechaAlta = externo.FechaGeneracion,
                FechaIngreso = externo.FechaIngreso,
                //Estado = externo.EstadoCupo,
                //Disponible = externo.Disponible,
            };
            return entidad;
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort>
                {
                    new Sort { Field = "FechaIngreso", Dir = "desc" },
                    new Sort { Field = "Material", Dir = "desc" }
                };
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


        public ActionResult AltaMasivaCupos()
        {
            FillViewBag();
            return View();
        }

        [Autorizacion(PermisosDataAgro.DisponibilidadDeCuposDescarga)]
        public ActionResult DisponibilidadDescarga()
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
        public ActionResult BuscaDatosTablaDisponibilidad(string FechaDesde, string FechaHasta, List<string> CentroId, string MaterialId)
        {
            DateTime fechaDesde = DateTime.Today;
            if (!String.IsNullOrEmpty(FechaDesde))
            {
                DateTime.TryParseExact(FechaDesde, "dd-MM-yyyy", new CultureInfo("es-AR"), DateTimeStyles.AdjustToUniversal, out fechaDesde);
            }
            DateTime fechaHasta = DateTime.Today;
            if (!String.IsNullOrEmpty(FechaHasta))
            {
                DateTime.TryParseExact(FechaHasta, "dd-MM-yyyy", new CultureInfo("es-AR"), DateTimeStyles.AdjustToUniversal, out fechaHasta);
            }
            List<DisponibilidadCuposDto> model = cupoManager.TraerDisponibilidadCupo(fechaDesde, fechaHasta, CentroId, MaterialId);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult BuscaDatosDisponibilidadDescarga(string FechaDesde, string FechaHasta, string ZonaId, List<string> CentroId, string MaterialId)
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

            List<DisponibilidadCuposDto> model = cupoManager.TraerCupoDisponibilidadDescarga(fechaDesde, fechaHasta, ZonaId, CentroId, MaterialId);

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

        public ActionResult TraerEstablecimientos(string cuitProveedor, bool esEPAoEUDR)
        {
            return new JsonResult()
            {
                Data = cupoManager.TraerEstablecimientos(cuitProveedor, esEPAoEUDR),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarNegociosParaSolicitarCupo(string contratoSap, int proveedorId, int materialId, int estadoId, bool sustentable, bool epa, bool eudr)
        {
            return new JsonResult()
            {
                Data = cupoManager.ListarNegociosParaSolicitarCupo(contratoSap, proveedorId, materialId, estadoId, sustentable, epa, eudr),
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private void CargarFiltros()
        {
            var esExterno = PermisosHelper.Is(PermisosDataAgro.IngresoExterno);
            var proveedores = proveedorManager.ListarProveedorTodos(string.Empty);
            ViewBag.Proveedores = proveedores.Select(
                x => new SelectListItem
                {
                    Text = x.RazonSocial,
                    Value = x.ProveedorId.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value);
            var material = materialManager.TraerTodoMaterial();
            ViewBag.Materiales = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            var estados = cupoManager.TraerTodoLosEstados();
            ViewBag.Estados = estados.Select(
               x => new SelectListItem
               {
                   Text = esExterno ? (x.Id == (int)EnumEstadoCupo.SinCTG ? "Aceptado" : (x.Id == (int)EnumEstadoCupo.SinSTOP ? "Pendiente" : x.Descripcion)) : x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
        }
    }
}
