using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ResearchController : Controller
    {
        private readonly IResearchManager oResearchManager;
        private readonly IMaterialManager oMaterialManager;
        public ResearchController(IResearchManager oResearchManager, IMaterialManager oMaterialManager)
        {
            this.oResearchManager = oResearchManager;
            this.oMaterialManager = oMaterialManager;
        }
        // GET: ResearchAvanceSiembra
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AvanceSiembraPartial()
        {
            FillViewBag();
            return PartialView("_AvanceSiembra", new ResearchAvanceSiembraModel
            {
                AvanceSiembra = TransformarAModel(oResearchManager.TraerTodoResearchAvanceSiembra()),
                HistorialAvanceSiembra = oResearchManager.TraerTodoResearchAvanceSiembra().OrderByDescending(x => x.FechaHora).ToList()

            });
        }
        public ActionResult AvanceCosechaPartial()
        {
            FillViewBag();
            return PartialView("_AvanceCosecha", new ResearchAvanceCosechaModel
            {
                AvanceCosecha = TransformarAModel(oResearchManager.TraerTodoResearchAvanceCosecha()),
                HistorialAvanceCosecha = oResearchManager.TraerTodoResearchAvanceCosecha().OrderByDescending(x => x.FechaHora).ToList()

            });
        }
        public ActionResult SituacionCultivoPartial()
        {
            FillViewBag();
            return PartialView("_SituacionCultivo", new ResearchSituacionCultivoModel
            {
                SituacionCultivo = TransformarAModel(oResearchManager.TraerTodoResearchSituacionCultivo()),
                HistorialSituacionCultivo = oResearchManager.TraerTodoResearchSituacionCultivo().OrderByDescending(x => x.FechaHora).ToList()

            });
        }
        public ActionResult VentaStockPartial()
        {
            FillViewBag();
            return PartialView("_VentaStock", new ResearchVentaStockModel
            {
                VentaStock = TransformarAModel(oResearchManager.TraerTodoResearchVentaStock()),
                HistorialVentaStock = oResearchManager.TraerTodoResearchVentaStock().OrderByDescending(x => x.FechaHora).ToList()

            });
        }
        [HttpPost]
        public ActionResult GrabarAvanceSiembra(ResearchAvanceSiembraModel researchAvanceSiembraModel)
        {
            var comercialId = GlobalVariables.ComercialId;
            var researchAvanceSiembra = TransformarAEntidad(researchAvanceSiembraModel);
            var resultado = oResearchManager.GrabarResearchAvanceSiembra(researchAvanceSiembra, comercialId);
            return PartialView("_ListaAvanceSiembra", new ResearchAvanceSiembraModel
            {
                AvanceSiembra = TransformarAModel(oResearchManager.TraerTodoResearchAvanceSiembra()),
                HistorialAvanceSiembra = oResearchManager.TraerTodoResearchAvanceSiembra().OrderByDescending(x => x.FechaHora).ToList(),
                Resultado = resultado
            });
        }

        [HttpPost]
        public ActionResult GrabarAvanceCosecha(ResearchAvanceCosechaModel researchAvanceCosechaModel)
        {
            var comercialId = GlobalVariables.ComercialId;
            var researchAvanceCosecha = TransformarAEntidad(researchAvanceCosechaModel);
            var resultado = oResearchManager.GrabarResearchAvanceCosecha(researchAvanceCosecha, comercialId);
            return PartialView("_ListaAvanceCosecha", new ResearchAvanceCosechaModel
            {
                AvanceCosecha = TransformarAModel(oResearchManager.TraerTodoResearchAvanceCosecha()),
                HistorialAvanceCosecha = oResearchManager.TraerTodoResearchAvanceCosecha().OrderByDescending(x => x.FechaHora).ToList(),
                Resultado = resultado
            });
        }
        [HttpPost]
        public ActionResult GrabarSituacionCultivo(ResearchSituacionCultivoModel researchSituacionCultivoModel)
        {
            var comercialId = GlobalVariables.ComercialId;
            var researchSituacionCultivo = TransformarAEntidad(researchSituacionCultivoModel);
            var resultado = oResearchManager.GrabarResearchSituacionCultivo(researchSituacionCultivo, comercialId);
            return PartialView("_ListaSituacionCultivo", new ResearchSituacionCultivoModel
            {
                SituacionCultivo = TransformarAModel(oResearchManager.TraerTodoResearchSituacionCultivo()),
                HistorialSituacionCultivo = oResearchManager.TraerTodoResearchSituacionCultivo().OrderByDescending(x => x.FechaHora).ToList(),
                Resultado = resultado
            });
        }
        [HttpPost]
        public ActionResult GrabarVentaStock(ResearchVentaStockModel researchVentaStockModel)
        {
            var comercialId = GlobalVariables.ComercialId;
            var researchVentaStock = TransformarAEntidad(researchVentaStockModel);
            var resultado = oResearchManager.GrabarResearchVentaStock(researchVentaStock, comercialId);
            return PartialView("_ListaVentaStock", new ResearchVentaStockModel
            {
                VentaStock = TransformarAModel(oResearchManager.TraerTodoResearchVentaStock()),
                HistorialVentaStock = oResearchManager.TraerTodoResearchVentaStock().OrderByDescending(x => x.FechaHora).ToList(),
                Resultado = resultado
            });
        }
        public ActionResult EliminarAvanceSiembra(int id)
        {
            return PartialView("_ListaAvanceSiembra", new ResearchAvanceSiembraModel
            {
                Resultado = oResearchManager.EliminarResearchAvanceSiembra(id),
                HistorialAvanceSiembra = oResearchManager.TraerTodoResearchAvanceSiembra().OrderByDescending(x => x.FechaHora).ToList()

            });
        }

        public ActionResult EliminarAvanceCosecha(int id)
        {
            return PartialView("_ListaAvanceCosecha", new ResearchAvanceCosechaModel
            {
                Resultado = oResearchManager.EliminarResearchAvanceCosecha(id),
                HistorialAvanceCosecha = oResearchManager.TraerTodoResearchAvanceCosecha().OrderByDescending(x => x.FechaHora).ToList()

            });
        }

        public ActionResult EliminarSituacionCultivo(int id)
        {
            return PartialView("_ListaSituacionCultivo", new ResearchSituacionCultivoModel
            {
                Resultado = oResearchManager.EliminarResearchSituacionCultivo(id),
                HistorialSituacionCultivo = oResearchManager.TraerTodoResearchSituacionCultivo().OrderByDescending(x => x.FechaHora).ToList()

            });
        }

        public ActionResult EliminarVentaStock(int id)
        {
            return PartialView("_ListaVentaStock", new ResearchVentaStockModel
            {
                Resultado = oResearchManager.EliminarResearchVentaStock(id),
                HistorialVentaStock = oResearchManager.TraerTodoResearchVentaStock().OrderByDescending(x => x.FechaHora).ToList()

            });
        }

        public ResearchAvanceSiembra TransformarAEntidad(ResearchAvanceSiembraModel researchAvanceSiembraModel)
        {
            var researchAvanceSiembra = new ResearchAvanceSiembra
            {
                MaterialId = researchAvanceSiembraModel.MaterialId,
                LocalidadId = researchAvanceSiembraModel.LocalidadId,
                IntencionSiembra = researchAvanceSiembraModel.IntencionSiembra,
                Avance = researchAvanceSiembraModel.Avance,
                CambioAA = researchAvanceSiembraModel.CambioAA,
                Observaciones = researchAvanceSiembraModel.Observaciones,
                FechaHora = researchAvanceSiembraModel.FechaHora
            };
            return researchAvanceSiembra;
        }

        public ResearchAvanceCosecha TransformarAEntidad(ResearchAvanceCosechaModel researchAvanceCosechaModel)
        {
            var researchAvanceCosecha = new ResearchAvanceCosecha
            {
                MaterialId = researchAvanceCosechaModel.MaterialId,
                LocalidadId = researchAvanceCosechaModel.LocalidadId,
                Rendimiento = researchAvanceCosechaModel.Rendimiento,
                Avance = researchAvanceCosechaModel.Avance,
                RangoDesde = researchAvanceCosechaModel.RangoDesde,
                RangoHasta = researchAvanceCosechaModel.RangoHasta,
                Observaciones = researchAvanceCosechaModel.Observaciones,
                FechaHora = researchAvanceCosechaModel.FechaHora
            };
            return researchAvanceCosecha;
        }

        public ResearchSituacionCultivo TransformarAEntidad(ResearchSituacionCultivoModel researchSituacionCultivoModel)
        {
            var researchSituacionCultivo = new ResearchSituacionCultivo
            {
                MaterialId = researchSituacionCultivoModel.MaterialId,
                LocalidadId = researchSituacionCultivoModel.LocalidadId,
                Estadio = researchSituacionCultivoModel.Estadio,
                Situacion = researchSituacionCultivoModel.Situacion,
                Observaciones = researchSituacionCultivoModel.Observaciones,
                FechaHora = researchSituacionCultivoModel.FechaHora
            };
            return researchSituacionCultivo;
        }
        public ResearchVentaStock TransformarAEntidad(ResearchVentaStockModel researchVentaStockModel)
        {
            var researchVentaStock = new ResearchVentaStock
            {
                MaterialId = researchVentaStockModel.MaterialId,
                LocalidadId = researchVentaStockModel.LocalidadId,
                Almacenado = researchVentaStockModel.Almacenado,
                VendidoAPrecio = researchVentaStockModel.VendidoAPrecio,
                Observaciones = researchVentaStockModel.Observaciones,
                FechaHora = researchVentaStockModel.FechaHora
            };
            return researchVentaStock;
        }

        public List<ResearchAvanceSiembraModel> TransformarAModel(List<ResearchAvanceSiembraDto> researchAvanceSiembra)
        {
            var lista = new List<ResearchAvanceSiembraModel>();
            foreach (var i in researchAvanceSiembra)
            {
                var researchAvanceSiembraModel = new ResearchAvanceSiembraModel
                {
                    MaterialId = i.MaterialId,
                    LocalidadNombre = i.Localidad,
                    MaterialDescripcion = i.Material,
                    LocalidadId = i.LocalidadId,
                    IntencionSiembra = i.IntencionSiembra,
                    Avance = i.Avance,
                    CambioAA = i.CambioAA,
                    Observaciones = i.Observaciones,
                    FechaHora = i.FechaHora
                };
                lista.Add(researchAvanceSiembraModel);

            }

            return lista;
        }
        public List<ResearchSituacionCultivoModel> TransformarAModel(List<ResearchSituacionCultivoDto> researchSituacionCultivo)
        {
            var lista = new List<ResearchSituacionCultivoModel>();
            foreach (var i in researchSituacionCultivo)
            {
                var researchSituacionCultivoModel = new ResearchSituacionCultivoModel
                {
                    MaterialId = i.MaterialId,
                    LocalidadNombre = i.Localidad,
                    MaterialDescripcion = i.Material,
                    LocalidadId = i.LocalidadId,
                    Situacion = i.Situacion,
                    Estadio = i.Estadio,
                    Observaciones = i.Observaciones,
                    FechaHora = i.FechaHora
                };
                lista.Add(researchSituacionCultivoModel);

            }

            return lista;
        }
        public List<ResearchAvanceCosechaModel> TransformarAModel(List<ResearchAvanceCosechaDto> researchAvanceCosecha)
        {
            var lista = new List<ResearchAvanceCosechaModel>();
            foreach (var i in researchAvanceCosecha)
            {
                var researchAvanceCosechaModel = new ResearchAvanceCosechaModel
                {
                    MaterialId = i.MaterialId,
                    LocalidadNombre = i.Localidad,
                    MaterialDescripcion = i.Material,
                    LocalidadId = i.LocalidadId,
                    Rendimiento = i.Rendimiento,
                    Avance = i.Avance,
                    RangoDesde = i.RangoDesde,
                    RangoHasta = i.RangoHasta,
                    Observaciones = i.Observaciones,
                    FechaHora = i.FechaHora
                };
                lista.Add(researchAvanceCosechaModel);

            }

            return lista;
        }
        public List<ResearchVentaStockModel> TransformarAModel(List<ResearchVentaStockDto> researchVentaStock)
        {
            var lista = new List<ResearchVentaStockModel>();
            foreach (var i in researchVentaStock)
            {
                var researchVentaStockModel = new ResearchVentaStockModel
                {
                    MaterialId = i.MaterialId,
                    LocalidadNombre = i.Localidad,
                    MaterialDescripcion = i.Material,
                    LocalidadId = i.LocalidadId,
                    Almacenado = i.Almacenado,
                    VendidoAPrecio = i.VendidoAPrecio,
                    Observaciones = i.Observaciones,
                    FechaHora = i.FechaHora
                };
                lista.Add(researchVentaStockModel);

            }

            return lista;
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
                    }).OrderBy(x=>x.Value);
            ViewBag.Material = materialesListItems;

            var estadio = new List<SelectListItem>() {
                new SelectListItem { Text= "Emergencia", Value= "Emergencia", Selected = false },
                new SelectListItem { Text = "Macollaje", Value = "Macollaje", Selected = false },
                new SelectListItem { Text = "Floracion", Value = "Floracion", Selected = false },
                new SelectListItem { Text = "Llenado", Value = "Llenado", Selected = false }
                };
            ViewBag.Estadio = estadio;
            var situacion = new List<SelectListItem>() {
                new SelectListItem { Text= "Malo", Value= "Malo", Selected = false },
                new SelectListItem { Text = "Regular", Value = "Regular", Selected = false },
                new SelectListItem { Text = "Bueno", Value = "Bueno", Selected = false },
                new SelectListItem { Text = "Muy Bueno", Value = "Muy Bueno", Selected = false },
                new SelectListItem { Text = "Excelente", Value = "Excelente", Selected = false }
                };
            ViewBag.Situacion = situacion;
        }
        [HttpPost]
        public ActionResult BuscaAvanceSiembra(KendoGridMvcRequest request)
        {
            var model = oResearchManager.TraerAvanceSiembra(request);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }

}


