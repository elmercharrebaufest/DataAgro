using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Helpers.Excel;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class HedgeController : Controller
    {
        private readonly IHedgeManager oHedgeManager;
        private readonly IReportesManager mobjReportesManager;
        private readonly IDiferencialManager diferencialManager;
        private readonly ICentroManager centroManager;

        public HedgeController(IHedgeManager oHedgeManager, IReportesManager mobjReportesManager, IDiferencialManager diferencialManager, ICentroManager centroManager)
        {
            this.oHedgeManager = oHedgeManager;
            this.mobjReportesManager = mobjReportesManager;
            this.diferencialManager = diferencialManager;
            this.centroManager = centroManager;
        }

        [Autorizacion(PermisosDataAgro.VisualizarHedge)]
        public ActionResult Index()
        {

            var model = new HedgeModel();
            var dia = oHedgeManager.Dia();
            model.Dia = dia;
            ViewBag.DiaCerrado = dia == null ? false : dia.Cerrado;
            ViewBag.Diferencial = dia?.Diferencial;
            return View(model);
        }

        public ActionResult HedgeMaterialPartial(Resultado res)
        {
            var mat = oHedgeManager.TraerTodosHedgeMaterial();
            var model = new HedgeModel
            {
                HedgeMaterial = TransformarAModel(mat),
                HistorialMaterial = mat,
                Resultado = res
            };
            return PartialView("_HedgeMaterial", model);
        }

        public ActionResult HedgeObjetivoPartial(Resultado res)
        {
            var mat = oHedgeManager.TraerTodosHedgeObjetivo();
            var model = new HedgeModel
            {
                HedgeObjetivo = TransformarAModel(mat),
                HistorialObjetivo = mat,
                Resultado = res
            };
            return PartialView("_HedgeObjetivo", model);
        }

        public ActionResult HedgeTCPartial(Resultado res)
        {
            var model = new HedgeModel
            {
                HedgeTC = TransformarAModel(oHedgeManager.TraerTodosHedgeTC()),
                Resultado = res
            };
            return PartialView("_HedgeTC", model);
        }

        public ActionResult HedgeMargenMoliendaPartial(Resultado res)
        {
            var mol = oHedgeManager.TraerTodosHedgeMargenMolienda();
            var model = new HedgeModel
            {
                HedgeMargenMolienda = TransformarAModel(mol),
                HistorialMargenMolienda = mol,
                Resultado = res
            };
            return PartialView("_HedgeMargenMolienda", model);
        }

        [HttpPost]
        public ActionResult GrabarHedgeMaterial(HedgeModel hedgeMat)
        {
            var comercialId = GlobalVariables.ComercialId;
            var hedge = TransformarAEntidad(hedgeMat.HedgeMaterial);
            var res = oHedgeManager.GrabarHedgeMaterial(hedge, comercialId);

            return HedgeMaterialPartial(res);
        }

        [HttpPost]
        public ActionResult GrabarHedgeObjetivo(HedgeModel hedgeObj)
        {
            var comercialId = GlobalVariables.ComercialId;
            var hedge = TransformarAEntidad(hedgeObj.HedgeObjetivo);
            var res = oHedgeManager.GrabarHedgeObjetivo(hedge, comercialId);

            return HedgeObjetivoPartial(res);
        }

        [HttpPost]
        public ActionResult GrabarHedgeTC(HedgeModel hedgeTC)
        {
            var comercialId = GlobalVariables.ComercialId;
            var hedge = TransformarAEntidad(hedgeTC.TCModel);
            var res = oHedgeManager.GrabarHedgeTC(hedge, comercialId);

            return HedgeTCPartial(res);
        }

        [HttpPost]
        public ActionResult GrabarHedgeMargenMolienda(HedgeModel hedgeMargen)
        {
            var comercialId = GlobalVariables.ComercialId;
            var hedge = TransformarAEntidad(hedgeMargen.HedgeMargenMolienda);
            var res = oHedgeManager.GrabarHedgeMargenMolienda(hedge, comercialId);

            return HedgeMargenMoliendaPartial(res);
        }

        public ActionResult EliminarHedgeTC(int id)
        {
            var res = oHedgeManager.EliminarHedgeTC(id);
            return HedgeTCPartial(res);
        }

        public ActionResult CerrarDia(bool mail, string observaciones)
        {
            var mailEnviar = mail ? ExcelReporteCompleto.GenerarExcel(this.ObtenerDatosReporte(), mobjReportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true) : new byte[1];

            var diferencial = diferencialManager.TraerDiferencial();

            oHedgeManager.CerrarDia(GlobalVariables.ComercialId, mailEnviar, GlobalVariables.IdActiveDirectory, mail, oHedgeManager.GenerarCuerpoMail(observaciones),
                                    (diferencial == null ? 0 : diferencial.DiferencialDefault));
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult ReabrirDia(HedgeModel diferencialHedge)
        {
            var diferencial = diferencialManager.TraerDiferencial();
            oHedgeManager.ReabrirDia(GlobalVariables.ComercialId, diferencial == null ? 0 : diferencial.DiferencialDefault);
            return RedirectToAction("Index");
        }

        public ActionResult Diferencial()
        {
            return new JsonResult()
            {
                Data = oHedgeManager.Diferencial(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        private List<HedgeMaterialModel> TransformarAModel(List<HedgeMaterialDto> hedgeMat)
        {
            var lista = new List<HedgeMaterialModel>()
            {
                new HedgeMaterialModel {MaterialId = 1, MaterialDescripcion ="Hedge Maíz",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad)},
                new HedgeMaterialModel {MaterialId = 3, MaterialDescripcion ="Hedge Soja",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad) }
            };
            return lista;
        }

        private List<HedgeObjetivoModel> TransformarAModel(List<HedgeObjetivoDto> hedgeMat)
        {
            var lista = new List<HedgeObjetivoModel>()
            {
                new HedgeObjetivoModel {MaterialId = 1, MaterialDescripcion ="Maíz",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 1 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 1 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)},
                new HedgeObjetivoModel {MaterialId = 2, MaterialDescripcion ="Trigo",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 2 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 2 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)},
                new HedgeObjetivoModel {MaterialId = 3, MaterialDescripcion ="Soja",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 3 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 3 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)},
                new HedgeObjetivoModel {MaterialId = 4, MaterialDescripcion ="Girasol",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 4 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 4 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)},
                new HedgeObjetivoModel {MaterialId = 5, MaterialDescripcion ="Girasol AO",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 5 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 5 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)},
                new HedgeObjetivoModel {MaterialId = 6, MaterialDescripcion ="Sorgo",
                    ARemitir = hedgeMat.Where(x => x.MaterialId == 6 && x.TipoObjetivoId == 1).Sum(x => x.Cantidad),
                    Pricing = hedgeMat.Where(x => x.MaterialId == 6 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)}
            };
            return lista;
        }

        private HedgeTCModel TransformarAModel(List<HedgeTCDto> hedgeTC)
        {
            var hedge = new HedgeTCModel
            {
                HedgeTC = hedgeTC,
                TotalHedge = 0,
                TotalTC = 0
            };
            decimal sumProd = 0;
            foreach (var hT in hedgeTC)
            {
                sumProd += hT.TC * hT.HedgePesos;
                hedge.TotalHedge += hT.HedgePesos;
            }
            if (hedge.TotalHedge > 0)
            {
                hedge.TotalTC = sumProd / hedge.TotalHedge;
            }
            return hedge;
        }

        private HedgeMargenMoliendaModel TransformarAModel(List<HedgeMargenMoliendaDto> hedgeMargen)
        {
            var mar = hedgeMargen.Where(x => x.Fecha.Date == DateTime.Today).FirstOrDefault();
            var margenMolienda = new HedgeMargenMoliendaModel
            {
                MargenMolienda = mar != null ? mar.MargenMolienda : 0
            };
            return margenMolienda;
        }

        private List<HedgeMaterial> TransformarAEntidad(List<HedgeMaterialModel> hedgeMat)
        {
            var listaDto = new List<HedgeMaterial>();

            var matAnterior = oHedgeManager.TraerTodosHedgeMaterial();

            foreach (var mat in hedgeMat)
            {
                if (listaDto != null)
                {
                    listaDto.AddRange(new List<HedgeMaterial>(){
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 1,
                            Cantidad = mat.Disponible - matAnterior
                                                        .Where(x=>x.MaterialId == mat.MaterialId &&x.TipoHedgeMaterialId== 1)
                                                        .Sum(x=>x.Cantidad) },
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 2,
                            Cantidad = mat.Forward - matAnterior
                                                        .Where(x=>x.MaterialId == mat.MaterialId &&x.TipoHedgeMaterialId== 2)
                                                        .Sum(x=>x.Cantidad)},
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 3,
                            Cantidad = mat.NewCrop - matAnterior
                                                        .Where(x=>x.MaterialId == mat.MaterialId &&x.TipoHedgeMaterialId== 3)
                                                        .Sum(x=>x.Cantidad)}
                    });
                }
            }
            return listaDto;
        }

        private List<HedgeObjetivo> TransformarAEntidad(List<HedgeObjetivoModel> hedgeMat)
        {
            var listaDto = new List<HedgeObjetivo>();

            var matAnterior = oHedgeManager.TraerTodosHedgeObjetivo();

            foreach (var mat in hedgeMat)
            {
                if (listaDto != null)
                {
                    listaDto.AddRange(new List<HedgeObjetivo>(){
                        new HedgeObjetivo { MaterialId = mat.MaterialId, TipoObjetivoId = 1,
                            Cantidad = mat.Pricing - matAnterior
                                                        .Where(x=>x.MaterialId == mat.MaterialId &&x.TipoObjetivoId== 1)
                                                        .Sum(x=>x.Cantidad) },
                        new HedgeObjetivo { MaterialId = mat.MaterialId, TipoObjetivoId = 2,
                            Cantidad = mat.ARemitir - matAnterior
                                                        .Where(x=>x.MaterialId == mat.MaterialId &&x.TipoObjetivoId== 2)
                                                        .Sum(x=>x.Cantidad)},

                    });
                }
            }
            return listaDto;
        }

        private HedgeTC TransformarAEntidad(TcModel hedgeTC)
        {
            var TC = new HedgeTC
            {
                TipoCambio = hedgeTC.TC,
                HedgePesos = hedgeTC.HedgePesos
            };

            return TC;
        }

        private HedgeMargenMolienda TransformarAEntidad(HedgeMargenMoliendaModel hedgeMargen)
        {
            var margenMolienda = new HedgeMargenMolienda
            {
                MargenMolienda = hedgeMargen.MargenMolienda
            };

            return margenMolienda;
        }

        private ReporteCompraNetModel ObtenerDatosReporte()
        {
            var hoy = DateTime.Now.Date;
            var agentes = mobjReportesManager.TraerAgenteDeCompra(hoy, hoy, null);
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(hoy, hoy, null),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(hoy, hoy),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(hoy, hoy, null),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(hoy, hoy, null),
                HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(hoy, hoy, null)),
                HedgeObjetivo = mobjReportesManager.TraerHedgeObjetivo(hoy, hoy, null),
                TCPromedioDto = mobjReportesManager.TraerTcPromedio(hoy, hoy, null),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
                SojaEPAyEUDR = mobjReportesManager.TraerToneladasSojaEPAyEUDR(hoy, hoy),
            };
        }
    }
}