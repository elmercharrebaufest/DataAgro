using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
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
    public class ConfiguracionInternaController : Controller
    {
        private readonly IConfiguracionInternaManager configuracionManager;
        private readonly IMaterialManager materialManager;
        private readonly IPrecioPizarraManager precioPizarraManager;
        private readonly ICampañaManager campañaManager;
        private readonly ITipoNegocioManager tipoNegocioManager;
        private readonly ICentroManager centroManager;

        public ConfiguracionInternaController(IConfiguracionInternaManager configuracionManager, IMaterialManager materialManager,
            IPrecioPizarraManager precioPizarraManager, ICampañaManager campañaManager, ITipoNegocioManager tipoNegocioManager, ICentroManager centroManager)
        {
            this.configuracionManager = configuracionManager;
            this.materialManager = materialManager;
            this.precioPizarraManager = precioPizarraManager;
            this.campañaManager = campañaManager;
            this.tipoNegocioManager = tipoNegocioManager;
            this.centroManager = centroManager;
        }

        [Autorizacion(PermisosDataAgro.ConfiguracionesInternas)]
        public ActionResult Index()
        {
            CargarViewBag();
            return View();
        }
        public ActionResult GrabarPrecioPartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarPrecioPartial", new ConfiguracionInternaModel
            {
                MonedaId = "ARP  ",
                DesdeVigencia = hoy.ToShortDateString() + " 00:00",
                HastaVigencia = hoy.ToShortDateString() + " 23:59",
                PrecioMoa = configuracionManager.TraerPrecios()
            });
        }
        public ActionResult GrabarPizarraPartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarPizarraPartial", new ConfiguracionInternaModel
            {
                PizarraDesde = hoy.ToShortTimeString(),
                PizarraHasta = hoy.AddDays(1).AddMinutes(-1).ToShortTimeString(),
                DiaPizarra = hoy.ToShortDateString(),
                HabilitacionPizarra = configuracionManager.TraerPizarra()
            });
        }
        public ActionResult GrabarPagoDiferidoPartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarPagoDiferidoPartial", new ConfiguracionInternaModel
            {
                HabilitacionPagoDiferido = configuracionManager.TraerPagoDiferido()
            });
        }
        public ActionResult GrabarFijacionPartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarFijacionPartial", new ConfiguracionInternaModel
            {
                FijacionDia = hoy.ToShortDateString(),
                HabilitacionFijacion = configuracionManager.TraerFijaciones()
            });
        }
        [HttpPost]
        public ActionResult GuardarPrecio(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoPrecio = configuracionManager.GrabarPrecio(TransformarAEntidadPrecio(configuracion), GlobalVariables.IdActiveDirectory);
            configuracion.PrecioMoa = configuracionManager.TraerPrecios();
            return PartialView("_ListaPrecio", configuracion);
        }

        [HttpPost]
        public ActionResult GuardarPizarra(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoPizarra = configuracionManager.GrabarPizarra(TransformarAEntidadPizarra(configuracion), GlobalVariables.IdActiveDirectory, DateTime.Parse(configuracion.DiaPizarraHasta));
            configuracion.HabilitacionPizarra = configuracionManager.TraerPizarra();
            return PartialView("_ListaPizarra", configuracion);
        }
        [HttpPost]
        public ActionResult GuardarFijacion(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoFijacion = configuracionManager.GrabarFijacion(TransformarAEntidadFijacion(configuracion));
            configuracion.HabilitacionFijacion = configuracionManager.TraerFijaciones();
            return PartialView("_ListaFijacion", configuracion);
        }

        [HttpPost]
        public ActionResult GuardarPagoDiferido(ConfiguracionInternaModel configuracion)
        {
            var resultado = new Resultado();
            if (!string.IsNullOrEmpty(configuracion.DesdeVigencia) && !string.IsNullOrEmpty(configuracion.HastaVigencia))
            {
                configuracion.ResultadoPago = configuracionManager.GrabarPagoDiferido(TransformarAEntidadPago(configuracion), GlobalVariables.IdActiveDirectory);
            }
            else
            {
                resultado.Error("FechaValida", "Debe ingresar una fecha de vigencia válida");
                configuracion.ResultadoPago = resultado;
            }
            configuracion.HabilitacionPagoDiferido = configuracionManager.TraerPagoDiferido();
            return PartialView("_ListaPago", configuracion);
        }

        private PrecioMoa TransformarAEntidadPrecio(ConfiguracionInternaModel configuracion)
        {
            var entidad = new PrecioMoa
            {
                MaterialId = configuracion.MaterialId,
                DesdeVigencia = DateTime.Parse(configuracion.DesdeVigencia),
                HastaVigencia = DateTime.Parse(configuracion.HastaVigencia),
                MonedaId = configuracion.MonedaId,
                Precio = configuracion.Precio,
                TipoNegocioId = configuracion.TipoNegocioId,
                DesdeEntrega = configuracion.DesdeEntrega,
                HastaEntrega = configuracion.HastaEntrega,
                DesdeFijacion = configuracion.DesdeFijacion,
                HastaFijacion = configuracion.HastaFijacion,
                Habilitado = configuracion.Pausar,
                DestinoId = configuracion.DestinoId
            };
            return entidad;
        }
        private HabilitacionPagoDiferido TransformarAEntidadPago(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionPagoDiferido
            {
                //MaterialId = configuracion.MaterialId,
                DesdeVigencia = DateTime.Parse(configuracion.DesdeVigencia),
                HastaVigencia = DateTime.Parse(configuracion.HastaVigencia),
                //TipoNegocioId = configuracion.TipoNegocioId,
                CantidadDia = configuracion.CantidadDia,
                Tasa = configuracion.Tasa
            };
            return entidad;
        }
        private HabilitacionPizarra TransformarAEntidadPizarra(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionPizarra
            {
                Dia = DateTime.Parse(configuracion.DiaPizarra),
                DesdeVigencia = DateTime.Parse(configuracion.PizarraDesde),
                HastaVigencia = DateTime.Parse(configuracion.PizarraHasta),
                MaterialId = configuracion.MaterialPizarra,
                TipoNegocioId = configuracion.TipoNegocioIdPizarra,
                DesdeEntrega = configuracion.DesdeEntregaPizarra,
                HastaEntrega = configuracion.HastaEntregaPizarra,
            };
            entidad.DesdeVigencia = new DateTime(entidad.Dia.Year, entidad.Dia.Month, entidad.Dia.Day, entidad.DesdeVigencia.Hour, entidad.DesdeVigencia.Minute, entidad.DesdeVigencia.Second);
            entidad.HastaVigencia = new DateTime(entidad.Dia.Year, entidad.Dia.Month, entidad.Dia.Day, entidad.HastaVigencia.Hour, entidad.HastaVigencia.Minute, entidad.HastaVigencia.Second);
            return entidad;
        }
        private HabilitacionFijacion TransformarAEntidadFijacion(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionFijacion
            {
                Dia = DateTime.Parse(configuracion.FijacionDia),
                MaterialId = configuracion.MaterialFijacionId
            };
            return entidad;
        }
        public ActionResult ModalHabilitarMaterial()
        {
            CargarViewBag();
            return PartialView("ModalHabilitarMaterial");
        }
        private void CargarViewBag()
        {
            var material = materialManager.TraerTodoMaterial();
            ViewBag.MaterialDesc = configuracionManager.TraerEstadoPrecioMOA();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;
            var moneda = precioPizarraManager.TraerTodoMoneda();
            var MonedaListItems = moneda.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MonedaId.ToString(),
                        Selected = x.Descripcion.Contains("ARP") ? true : false
                    }).OrderBy(x => x.Value);
            ViewBag.Moneda = MonedaListItems;
            var tiponegocio = tipoNegocioManager.TraerTodoTipoNegocio().Where(a => a.TipoNegocioId == 1 || a.TipoNegocioId == 2 || a.TipoNegocioId == 3);
            var tiponegociolist = tiponegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString()
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = tiponegociolist;

            var tiponegociopizarralist = tiponegocio.Where(a => a.TipoNegocioId == 3 || a.TipoNegocioId == 2).Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString()
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocioPizarra = tiponegociopizarralist;

            var tiponegocioSustentalbelist = tiponegocio.Where(a => a.TipoNegocioId == 1 || a.TipoNegocioId == 2).Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString()
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocioSustentable = tiponegocioSustentalbelist;

            var campaña = campañaManager.TraerTodoCampania();
            var campañaList = campaña.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.CampañaId.ToString()
                    }).OrderBy(x => x.Value);
            ViewBag.Campaña = campañaList;
            ViewBag.Habilitado = configuracionManager.TraerPausadoGeneral();

            var centro = centroManager.TraerTodoCentro();
            var centroListItems = centro.Centro.Where(x => x.Id != 9 && x.Id != 10 && x.Id != 13).Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.Id.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);
            ViewBag.Centro = centroListItems;

          
        }
        public ActionResult EliminarPrecio(int id)
        {
            //return PartialView("_ListaPrecio", new ConfiguracionInternaModel
            //{
            //    ResultadoPrecio = configuracionManager.EliminarPrecio(id),
            //    PrecioMoa = configuracionManager.TraerPrecios()
            //});
            return Json(configuracionManager.EliminarPrecio(id));
        }
        public ActionResult EliminarPizarra(int id)
        {
            return PartialView("_ListaPizarra", new ConfiguracionInternaModel
            {
                ResultadoPizarra = configuracionManager.EliminarPizarra(id),
                HabilitacionPizarra = configuracionManager.TraerPizarra()
            });
        }
        public ActionResult EliminarFijacion(int id)
        {
            return PartialView("_ListaFijacion", new ConfiguracionInternaModel
            {
                ResultadoFijacion = configuracionManager.EliminarFijacion(id),
                HabilitacionFijacion = configuracionManager.TraerFijaciones()
            });
        }

        [HttpPost]
        public ActionResult GuardarCampaña(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoCampaña = configuracionManager.GrabarCampaña(TransformarAEntidadCampaña(configuracion));
            configuracion.HabilitacionCampaña = configuracionManager.TraerCampaña();
            return PartialView("_ListaCampaña", configuracion);
        }
        public ActionResult GrabarCampañaPartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarCampañaPartial", new ConfiguracionInternaModel
            {
                //CampañaDesde = hoy.ToShortTimeString(),
                //CampañaHasta = hoy.AddDays(1).AddMinutes(-1).ToShortTimeString(),
                //DiaCampaña = hoy.ToShortDateString(),
                HabilitacionCampaña = configuracionManager.TraerCampaña()
            });
        }

        private HabilitacionCampaña TransformarAEntidadCampaña(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionCampaña
            {

                MaterialId = configuracion.MaterialCampañaId,
                CampañaId = configuracion.CampañaId,
            };
            return entidad;
        }

        public ActionResult EliminarCampaña(int id)
        {
            return PartialView("_ListaCampaña", new ConfiguracionInternaModel
            {
                ResultadoCampaña = configuracionManager.EliminarCampaña(id),
                HabilitacionCampaña = configuracionManager.TraerCampaña()
            });
        }

        public ActionResult EliminarPagoDiferido(int id)
        {
            return PartialView("_ListaPago", new ConfiguracionInternaModel
            {
                ResultadoPago = configuracionManager.EliminarHabilitacionPagoDiferido(id),
                HabilitacionPagoDiferido = configuracionManager.TraerPagoDiferido()
            });
        }

        public ActionResult PausarPrecios(List<EstadoPrecioMOADto> lista)
        {
            configuracionManager.CambiarEstadoPrecioMOA(lista);
            CargarViewBag();
            return new JsonResult()
            {
                Data = "",
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult GrabarSustentablePartial()
        {
            var hoy = DateTime.Today;
            CargarViewBag();
            return PartialView("_GrabarSustentablePartial", new ConfiguracionInternaModel
            {
                MonedaId = "USDM ",
                DesdeVigenciaSustentable = hoy.ToShortDateString() + " 00:00",
                HastaVigenciaSustentable = hoy.ToShortDateString() + " 23:59",
                HabilitacionSustentable = configuracionManager.TraerSustentables()
            });
        }

        [HttpPost]
        public ActionResult GuardarSustentable(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoSustentable = configuracionManager.GrabarSustentable(TransformarAEntidadSustentable(configuracion), GlobalVariables.IdActiveDirectory);
            configuracion.HabilitacionSustentable = configuracionManager.TraerSustentables();
            return PartialView("_ListaSustentable", configuracion);
        }
        public ActionResult EliminarSustentable(int id)
        {
            return PartialView("_ListaSustentable", new ConfiguracionInternaModel
            {
                ResultadoSustentable = configuracionManager.EliminarSustentable(id),
                HabilitacionSustentable = configuracionManager.TraerSustentables()
            });
        }

        private HabilitacionSustentable TransformarAEntidadSustentable(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionSustentable
            {
                DesdeVigencia = DateTime.Parse(configuracion.DesdeVigenciaSustentable),
                HastaVigencia = DateTime.Parse(configuracion.HastaVigenciaSustentable),
                MonedaId = configuracion.MonedaId,
                Precio = configuracion.PrecioSustentable,
                TipoNegocioId = configuracion.TipoNegocioId,
                DesdeEntrega = configuracion.DesdeEntregaSustentable,
                HastaEntrega = configuracion.HastaEntregaSustentable
            };
            return entidad;
        }

        public JsonResult TraerPrecios(KendoGridMvcRequest request)
        {
            var result = new KendoGrid<PrecioMoaDto>(request, configuracionManager.TraerPrecios());
            return Json(result);
        }

        [HttpPost]
        public JsonResult UpdatePrecio(List<PrecioMoaDto> models)
        {
            foreach (var item in models)
            {
                Resultado ResultadoPrecio = configuracionManager.ActualizarPrecio(item.Id, item.Precio == null ? 0 : item.Precio.Value, GlobalVariables.IdActiveDirectory);
            }
            return Json("");
        }
    }
}