using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class PrecioPizarraController : Controller
    {
        private readonly IPrecioPizarraManager oPrecioPizarraManager;
        private readonly IMaterialManager oMaterialManager;
        private readonly IPizarraManager oPizarraManager;
        private readonly ILogger logger;

        public PrecioPizarraController(IPrecioPizarraManager oPrecioPizarraManager, IMaterialManager oMaterialManager, IPizarraManager oPizarraManager, ILogger logger)
        {
            this.oPrecioPizarraManager = oPrecioPizarraManager;
            this.oMaterialManager = oMaterialManager;
            this.oPizarraManager = oPizarraManager;
            this.logger = logger;
        }

        // GET: PrecioPizarra
        [Autorizacion(PermisosDataAgro.VisualizarPizarra)]
        public ActionResult Index()
        {
            FillViewBag();
            return View("Index", new PrecioPizarraModel
            {
                Precios = TransformarAModel(oPrecioPizarraManager.TraerTodoPrecioPizarra()),
                HistorialPrecioPizarra = new List<PrecioPizarraDto>()
            });
        }

        [HttpPost]
        public ActionResult GrabarPrecioPizarra(PrecioPizarraModel precioPizarraModel)
        {
            var precioPizarra = TransformarAEntidad(precioPizarraModel);

            var resultado = oPrecioPizarraManager.GrabarPrecioPizarra(precioPizarra);

            return PartialView("_ListaPrecioPizarra", new PrecioPizarraModel
            {
                Precios = TransformarAModel(oPrecioPizarraManager.TraerTodoPrecioPizarra()),
                HistorialPrecioPizarra = oPrecioPizarraManager.TraerPrecioPizarraPorMaterialYPizarra(precioPizarra.MaterialId, precioPizarra.PizarraId),
                Resultado = resultado
            });
        }

        private PrecioPizarra TransformarAEntidad(PrecioPizarraModel precioPizarraModel)
        {
            var precioPizarra = new PrecioPizarra
            {
                Id = precioPizarraModel.Id,
                MaterialId = precioPizarraModel.MaterialId,
                FechaDesde = DateTime.ParseExact(precioPizarraModel.FechaDesde.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                FechaHasta = DateTime.ParseExact(precioPizarraModel.FechaHasta.ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture),
                PizarraId = precioPizarraModel.PizarraId,
                Precio = precioPizarraModel.Precio,
                MonedaId = precioPizarraModel.MonedaId,
                UnidadMedida = precioPizarraModel.UnidadMedida,
                ComercialId = GlobalVariables.ComercialId
            };
            return precioPizarra;
        }

        private List<PrecioPizarraModel> TransformarAModel(List<PrecioPizarraDto> precioPizarra)
        {
            var lista = new List<PrecioPizarraModel>();
            foreach (var i in precioPizarra)
            {
                var precioPizarraModel = new PrecioPizarraModel
                {
                    MaterialId = i.MaterialId,
                    Id = i.Id,
                    FechaDesde = i.FechaDesde.ToString(),
                    FechaHasta = i.FechaHasta.ToString(),
                    MonedaId = i.MonedaId,
                    PizarraId = i.PizarraId,
                    Precio = i.Precio,
                    UnidadMedida = i.UnidadMedida
                };
                lista.Add(precioPizarraModel);
            }
            return lista;
        }

        public ActionResult BuscarPorPizarraYMaterial(int materialId, int pizarraId)
        {
            return new JsonResult()
            {
                Data = oPrecioPizarraManager.TraerPrecioPizarraPorMaterialYPizarra(materialId, pizarraId)
            };
        }

        public ActionResult ActualizarPrecioPizarra(DateTime fecha, bool manual)
        {
            try
            {
                oPrecioPizarraManager.ActualizarPrecioPizarra(fecha, manual);
                return new JsonResult()
                {
                    Data = "Ok"
                };
            }
            catch (Exception e)
            {
                logger.Error("Error en ActualizarPrecioPizarra(): ", e);
                throw e;
            }
        }

        public ActionResult EliminarPrecio(int id)
        {
            var precioPizarra = oPrecioPizarraManager.TraerPrecioPizarraPorId(id);
            var resultado = oPrecioPizarraManager.EliminarPizarra(id);
            return PartialView("_ListaPrecioPizarra", new PrecioPizarraModel
            {
                Precios = TransformarAModel(oPrecioPizarraManager.TraerTodoPrecioPizarra()),
                HistorialPrecioPizarra = oPrecioPizarraManager.TraerPrecioPizarraPorMaterialYPizarra(precioPizarra.MaterialId, precioPizarra.PizarraId),
                Resultado = resultado
            });
        }

        private void FillViewBag()
        {
            var material = oMaterialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var pizarra = oPizarraManager.TraerTodoPizarra();
            var pizarraListItems = pizarra.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.Id.ToString(),
                    Selected = x.Codigo == "ROS"
                }).OrderBy(x => x.Value);
            ViewBag.Pizarra = pizarraListItems;

            var moneda = oPrecioPizarraManager.TraerTodoMoneda();

            var monedaListItems = moneda.Select(
                x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.MonedaId.ToString(),
                    Selected = x.Descripcion == "ARP"
                }).OrderBy(x => x.Value);
            ViewBag.Moneda = monedaListItems;
        }

        public ActionResult CompletarPrecioPizarraEnNegocios(string fecha)
        {
            logger.Info("INICIO CompletarPrecioPizarraEnNegocios");
            oPrecioPizarraManager.CompletarPrecioPizarraEnNegocios(DateTime.ParseExact(fecha, "yyyyMMdd", null));
            logger.Info("FIN CompletarPrecioPizarraEnNegocios");
            return Content("ok");
        }
    }
}