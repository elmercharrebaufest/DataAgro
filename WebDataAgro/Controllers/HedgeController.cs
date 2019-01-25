using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class HedgeController : Controller
    {
        private readonly IHedgeManager oHedgeManager;

        public HedgeController(IHedgeManager oHedgeManager)
        {
            this.oHedgeManager = oHedgeManager;
        }
        public ActionResult Index()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Mesa)
            {
                ViewBag.DiaCerrado = oHedgeManager.Dia();
                return View();
            }
            else
            {
                return View("ErrorDePermisos");
            }
        }
        public ActionResult HedgeMaterialPartial(Resultado res)
        {
            var model = new HedgeModel
            {
                HedgeMaterial = TransformarAModel(oHedgeManager.TraerTodosHedgeMaterial()),
                Resultado = res
            };
            return PartialView("_HedgeMaterial", model);
        }
        public ActionResult HedgeObjetivoPartial(Resultado res)
        {
            var model = new HedgeModel
            {
                HedgeObjetivo = TransformarAModel(oHedgeManager.TraerTodosHedgeObjetivo()),
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
        public ActionResult EliminarHedgeTC(int id)
        {
            var res = oHedgeManager.EliminarHedgeTC(id);
            return HedgeTCPartial(res);
        }
        public ActionResult CerrarDia()
        {
            oHedgeManager.CerrarDia(GlobalVariables.ComercialId);
            return RedirectToAction("Index");
        }
        private List<HedgeMaterialModel> TransformarAModel(List<HedgeMaterialDto> hedgeMat)
        {
            var lista = new List<HedgeMaterialModel>()
            {
                new HedgeMaterialModel {MaterialId = 1, MaterialDescripcion ="Hedge Maíz"},
                new HedgeMaterialModel {MaterialId = 3, MaterialDescripcion ="Hedge Soja" }
            };
            foreach (var hedge in hedgeMat)
            {
                var elemLista = lista.Where(x => x.MaterialId == hedge.MaterialId).FirstOrDefault();
                if (elemLista != null)
                {
                    if (hedge.TipoHedgeMaterialId == 1)
                    {
                        elemLista.Disponible = hedge.Cantidad;
                    }
                    else if (hedge.TipoHedgeMaterialId == 2)
                    {
                        elemLista.Forward = hedge.Cantidad;
                    }
                    else if (hedge.TipoHedgeMaterialId == 3)
                    {
                        elemLista.NewCrop = hedge.Cantidad;
                    }
                }
            }
            return lista;
        }
        private List<HedgeObjetivoModel> TransformarAModel(List<HedgeObjetivoDto> hedgeMat)
        {
            var lista = new List<HedgeObjetivoModel>()
            {
                new HedgeObjetivoModel {MaterialId = 1, MaterialDescripcion ="Maíz"},
                new HedgeObjetivoModel {MaterialId = 2, MaterialDescripcion ="Trigo" },
                new HedgeObjetivoModel {MaterialId = 3, MaterialDescripcion ="Soja" }
            };
            foreach (var hedge in hedgeMat)
            {
                var elemLista = lista.Where(x => x.MaterialId == hedge.MaterialId).FirstOrDefault();
                if (elemLista != null)
                {
                    if (hedge.TipoObjetivoId == 1)
                    {
                        elemLista.Pricing = hedge.Cantidad;
                    }
                    else if (hedge.TipoObjetivoId == 2)
                    {
                        elemLista.ARemitir = hedge.Cantidad;
                    }
                }
            }
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
            if(hedge.TotalHedge>0)
            {
                hedge.TotalTC = sumProd / hedge.TotalHedge;
            }
            return hedge;
        }
        private List<HedgeMaterial> TransformarAEntidad(List<HedgeMaterialModel> hedgeMat)
        {
            var listaDto = new List<HedgeMaterial>();
            foreach(var mat in hedgeMat)
            {
                if (listaDto!= null)
                {
                    listaDto.AddRange(new List<HedgeMaterial>(){
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 1, Cantidad = mat.Disponible },
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 2, Cantidad = mat.Forward },
                        new HedgeMaterial { MaterialId = mat.MaterialId, TipoHedgeMaterialId = 3, Cantidad = mat.NewCrop }
                    });
                }
            }
            return listaDto;
        }
        private List<HedgeObjetivo> TransformarAEntidad(List<HedgeObjetivoModel> hedgeMat)
        {
            var listaDto = new List<HedgeObjetivo>();
            foreach (var mat in hedgeMat)
            {
                if (listaDto != null)
                {
                    listaDto.AddRange(new List<HedgeObjetivo>(){
                        new HedgeObjetivo { MaterialId = mat.MaterialId, TipoObjetivoId = 1, Cantidad = mat.Pricing },
                        new HedgeObjetivo { MaterialId = mat.MaterialId, TipoObjetivoId = 2, Cantidad = mat.ARemitir }
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
    }
}