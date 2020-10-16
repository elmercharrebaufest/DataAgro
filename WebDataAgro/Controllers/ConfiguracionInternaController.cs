using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ConfiguracionInternaController : Controller
    {
        private readonly IConfiguracionInternaManager configuracionManager;
        private readonly IMaterialManager materialManager;
        private readonly IPrecioPizarraManager precioPizarraManager;

        public ConfiguracionInternaController(IConfiguracionInternaManager configuracionManager, IMaterialManager materialManager,IPrecioPizarraManager precioPizarraManager )
        {
            this.configuracionManager = configuracionManager;
            this.materialManager = materialManager;
            this.precioPizarraManager = precioPizarraManager;
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
            return PartialView("_GrabarPrecioPartial",new ConfiguracionInternaModel
            {
                MonedaId = "ARP  ",
                DesdeVigencia = hoy.ToShortDateString()+" 00:00",
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
            configuracion.ResultadoPrecio = configuracionManager.GrabarPrecio(TransformarAEntidadPrecio(configuracion));            
            configuracion.PrecioMoa = configuracionManager.TraerPrecios();
            return PartialView("_ListaPrecio", configuracion);
        }
       
        [HttpPost]
        public ActionResult GuardarPizarra(ConfiguracionInternaModel configuracion)
        {
            configuracion.ResultadoPizarra = configuracionManager.GrabarPizarra(TransformarAEntidadPizarra(configuracion));
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

        private PrecioMoa TransformarAEntidadPrecio(ConfiguracionInternaModel configuracion)
        {
            var entidad = new PrecioMoa
            {
               MaterialId = configuracion.MaterialId,
               DesdeVigencia= DateTime.Parse( configuracion.DesdeVigencia),
               HastaVigencia = DateTime.Parse(configuracion.HastaVigencia),
               MonedaId =configuracion.MonedaId,
               Precio= configuracion.Precio
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
                MaterialId = configuracion.MaterialPizarra
            };
            entidad.DesdeVigencia = new DateTime(entidad.Dia.Year, entidad.Dia.Month, entidad.Dia.Day, entidad.DesdeVigencia.Hour, entidad.DesdeVigencia.Minute, entidad.DesdeVigencia.Second);
            entidad.HastaVigencia = new DateTime(entidad.Dia.Year, entidad.Dia.Month, entidad.Dia.Day, entidad.HastaVigencia.Hour, entidad.HastaVigencia.Minute, entidad.HastaVigencia.Second);
            return entidad;
        }
        private HabilitacionFijacion TransformarAEntidadFijacion(ConfiguracionInternaModel configuracion)
        {
            var entidad = new HabilitacionFijacion
            {
                Dia = DateTime.Parse(configuracion.FijacionDia ),
                MaterialId= configuracion.MaterialFijacionId
            };
            return entidad;
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
            var moneda = precioPizarraManager.TraerTodoMoneda();
            var MonedaListItems = moneda.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MonedaId.ToString(),
                        Selected = x.Descripcion.Contains("ARP")? true : false
                    }).OrderBy(x => x.Value);
            ViewBag.Moneda = MonedaListItems;
        }
        public ActionResult EliminarPrecio(int id)
        {
            return PartialView("_ListaPrecio", new ConfiguracionInternaModel
            {
                ResultadoPrecio = configuracionManager.EliminarPrecio(id),
                PrecioMoa = configuracionManager.TraerPrecios()
            });
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
    }
}