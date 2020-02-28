using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionResult CerrarDia(bool mail, string observaciones)
        {
            var mailEnviar = mail ? ExcelReporteCompleto.GenerarExcel(this.ObtenerDatosReporte(), mobjReportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true) : new byte[1];

            var diferencial = diferencialManager.TraerDiferencial();
            oHedgeManager.CerrarDia(GlobalVariables.ComercialId,
                                    mailEnviar,
                                    GlobalVariables.IdActiveDirectory,
                                    mail,
                                    GenerarCuerpoMail(observaciones),
                                    diferencial.DiferencialDefault);
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
                    Pricing = hedgeMat.Where(x => x.MaterialId == 3 && x.TipoObjetivoId == 2).Sum(x => x.Cantidad)}
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

        private ReporteCompraNetModel ObtenerDatosReporte()
        {
            var hoy = DateTime.Now.Date;
            var agentes = mobjReportesManager.TraerAgenteDeCompra(hoy, hoy);
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(hoy, hoy),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(hoy, hoy),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(hoy, hoy),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(hoy, hoy),
                HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(hoy, hoy)),
                HedgeObjetivo = mobjReportesManager.TraerHedgeObjetivo(hoy, hoy),
                TCPromedioDto = mobjReportesManager.TraerTcPromedio(hoy, hoy),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
            };
        }

        private string GenerarCuerpoMail_Viejo(string observaciones)
        {
            var model = this.ObtenerDatosReporte();

            var dispAFijar = model.ToneladasGranoTipo.Any(x => x.DispAFijar > 0);
            var dispAPrecio = model.ToneladasGranoTipo.Any(x => x.DispAPrecio > 0);
            var dispFijacion = model.ToneladasGranoTipo.Any(x => x.DispFijac > 0);
            var dispFason = model.ToneladasGranoTipo.Any(x => x.DispFason > 0);
            var disp = 4 - (dispAFijar ? 0 : 1) - (dispAPrecio ? 0 : 1) - (dispFijacion ? 0 : 1) - (dispFason ? 0 : 1);
            var forwAFijar = model.ToneladasGranoTipo.Any(x => x.FrwAFijar > 0);
            var forwAPrecio = model.ToneladasGranoTipo.Any(x => x.FrwAPrecio > 0);
            var forwFijacion = model.ToneladasGranoTipo.Any(x => x.FrwFijac > 0);
            var forwFason = model.ToneladasGranoTipo.Any(x => x.FrwFason > 0);
            var dispFwd = dispAPrecio || dispFijacion || forwAPrecio || forwFijacion;
            var forw = 4 - (forwAFijar ? 0 : 1) - (forwAPrecio ? 0 : 1) - (forwFijacion ? 0 : 1) - (forwFason ? 0 : 1);
            var newcAFijar = model.ToneladasGranoTipo.Any(x => x.NewAFijar > 0);
            var newcAPrecio = model.ToneladasGranoTipo.Any(x => x.NewAPrecio > 0);
            var newcFijacion = model.ToneladasGranoTipo.Any(x => x.NewFijac > 0);
            var newcFason = model.ToneladasGranoTipo.Any(x => x.NewFason > 0);
            var newc = 4 - (newcAFijar ? 0 : 1) - (newcAPrecio ? 0 : 1) - (newcFijacion ? 0 : 1) - (newcFason ? 0 : 1);

            var hedgeMat = model.HedgeMaterial.Any(x => x.Disponible != 0 || x.Forward != 0 || x.NewCrop != 0);
            //var hedgeObj = model.HedgeObjetivo.RemitirObjetivo != 0 && model.HedgeObjetivo.PricingObjetivo != 0;

            var titulo = " border: 1px solid black; background: #017940; color: white; ";
            var datoIzquierda = " font-weight:bold; border: 1px solid black; text-align:left; ";
            var datoCentro = " border: 1px solid black; text-align:center; ";
            var colorHedge = " background: #008b8b; ";
            //var colorObjetivo = " background: #dda0dd; ";
            var colorPosicion = "";
            //var colorAgente = " background: #FFD700; ";

            var htmlBody = "";

            htmlBody += "Estimados,";
            htmlBody += "<br></br>";
            htmlBody += "A continuación, se detallan las compras correspondientes al cierre del día.";
            htmlBody += "<br></br>";
            htmlBody += "<br></br>";


            if (hedgeMat)
            {
                htmlBody += "<table style=\" width: 100%; border-collapse:unset\">";
                htmlBody += "<tbody>";
                htmlBody += "<tr style=\" " + colorHedge + " color: white;\">";
                htmlBody += "<th style=\" border: 1px solid black;\" colspan=\"4\" > HEDGE </th>";
                htmlBody += "</tr>";
                htmlBody += "<tr> <th style=\" " + titulo + colorHedge + " \">Producto</th>";
                htmlBody += "<th style=\" " + titulo + colorHedge + " \">Disponible</th> ";
                htmlBody += "<th style=\" " + titulo + colorHedge + " \">Forward</th> ";
                htmlBody += "<th style=\" " + titulo + colorHedge + "\">New Crop</th></tr>";
                foreach (var mat in model.HedgeMaterial)
                {
                    if (mat.Disponible != 0 || mat.Forward != 0 || mat.NewCrop != 0)
                    {
                        htmlBody += "<tr>";
                        htmlBody += "<td style=\" " + datoIzquierda + " \" >" + mat.MaterialDescripcion + "</td>";
                        htmlBody += "<td style=\" " + datoCentro + " \" >" + mat.Disponible.ToString("n0") + "</td>";
                        htmlBody += "<td style=\" " + datoCentro + " \" >" + mat.Forward.ToString("n0") + "</td>";
                        htmlBody += "<td style=\" " + datoCentro + " \" >" + mat.NewCrop.ToString("n0") + "</td>";
                        htmlBody += "</tr>";
                    }
                };
                htmlBody += "</tbody>";
                htmlBody += "</table>";
                htmlBody += "<br></br>";
            }



            //if (hedgeObj)
            //{
            //    htmlBody += "  <table  style=\" width: 50%; border-collapse:unset;\">";
            //    htmlBody += "     <tr>";
            //    htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \"></th>";
            //    htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Dia</th>";
            //    htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Objetivo</th>";
            //    htmlBody += "     </tr>";
            //    htmlBody += "     <tr>";
            //    htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Pricing</th>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.PricingCumplido.ToString("n0") + "</td>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.PricingObjetivo.ToString("n0") + "</td>";
            //    htmlBody += "     </tr>";
            //    htmlBody += "     <tr>";
            //    htmlBody += "         <th  style=\"  " + titulo + colorObjetivo + "  \">A remitir</th>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.RemitirCumplido.ToString("n0") + "</td>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.RemitirObjetivo.ToString("n0") + "</td>";
            //    htmlBody += "     </tr>";
            //    htmlBody += " </table>";

            //    htmlBody += "<br></br>";

            //    htmlBody += " <table style=\" width: 25%; border-collapse:unset;\">";
            //    htmlBody += "     <tr>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \" >Hedge TC: $" + model.TCPromedioDto.PromedioTC.ToString("N0") + "</td>";
            //    htmlBody += "         <td style=\" " + datoCentro + " \">$" + model.TCPromedioDto.TotalTC.ToString("N0") + "</td>";
            //    htmlBody += "     </tr>";
            //    htmlBody += "</table>";

            //}

            htmlBody += "<br></br>";

            if (disp + forw + newc != 0)
            {
                htmlBody += "<table style=\"width:100%; border-collapse:unset;\">";
                htmlBody += "<tbody>";
                htmlBody += "<tr>";
                htmlBody += "<td style=\"  " + titulo + " \"></td>";

                if (disp != 0)
                {
                    htmlBody += "<th  style=\"  " + titulo + " \" colspan=\" " + (disp) + "\">DISPONIBLE</th>";
                    htmlBody += "<th  style=\"  " + titulo + " \" rowspan=\" " + 2 + "\">TOTAL DISP</th>";
                }

                if (forw != 0)
                {
                    htmlBody += "<th  style=\" " + titulo + "\" colspan=\" " + (forw) + "\">FORWARD</th>";
                    htmlBody += "<th  style=\" " + titulo + "\" rowspan=\" " + 2 + "\">TOTAL FORWARD</th>";
                }

                if (newc != 0)
                {
                    htmlBody += "<th  style=\" " + titulo + "\" colspan=\" " + (newc) + "\">NEW CROP</th>";
                    htmlBody += "<th  style=\" " + titulo + "\" rowspan=\" " + 2 + "\">TOTAL NEW CROP</th>";
                }

                htmlBody += "</tr>";
                htmlBody += "<tr>";
                htmlBody += "<th  style=\" " + titulo + "\">PRODUCTO</th>";

                if (dispAFijar)
                {
                    htmlBody += "<th  style=\" " + titulo + "\" >A Fijar</th>";
                }

                if (dispAPrecio)
                {
                    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";
                }

                if (dispFijacion || dispFason)
                {
                    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";
                }


                if (forwAFijar)
                {
                    htmlBody += "<th style=\" " + titulo + "\">A Fijar</th>";
                }

                if (forwAPrecio)
                {
                    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";
                }

                if (forwFason || forwFijacion)
                {
                    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";
                }


                if (newcAFijar)
                {
                    htmlBody += "<th style=\" " + titulo + "\">A Fijar</th>";
                }

                if (newcAPrecio)
                {
                    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";
                }

                if (newcFason || newcFijacion)
                {
                    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";
                }




                htmlBody += "</tr>";

                foreach (var toneladaPrecio in model.ToneladasGranoTipo)
                {
                    if (toneladaPrecio.Total > 0)
                    {
                        htmlBody += "<tr>";
                        htmlBody += " <th  style=\" " + datoIzquierda + " \" >" + toneladaPrecio.Material + "</th>";

                        //DISPONIBLE
                        if (dispAFijar)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.DispAFijar.ToString("N0") + "</td>";
                        }
                        if (dispAPrecio)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.DispAPrecio.ToString("N0") + "</td>";
                        }
                        if (dispFason || dispFijacion)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.DispFijac + toneladaPrecio.DispFason).ToString("N0") + "</td>";
                        }
                        if (disp != 0)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.DispAFijar + toneladaPrecio.DispAPrecio + toneladaPrecio.DispFijac + toneladaPrecio.DispFason).ToString("N0") + "</td>";
                        }

                        //FORWARD
                        if (forwAFijar)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.FrwAFijar.ToString("N0") + "</td>";
                        }
                        if (forwAPrecio)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.FrwAPrecio.ToString("N0") + "</td>";
                        }
                        if (forwFason || forwFijacion)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.FrwFijac + toneladaPrecio.FrwFason).ToString("N0") + "</td>";
                        }
                        if (forw != 0)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.FrwAFijar + toneladaPrecio.FrwAPrecio + toneladaPrecio.FrwFijac + toneladaPrecio.FrwFason).ToString("N0") + "</td>";
                        }

                        //NEW CROP
                        if (newcAFijar)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" > " + toneladaPrecio.NewAFijar.ToString("N0") + "</td>";
                        }
                        if (newcAPrecio)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.NewAPrecio.ToString("N0") + "</td>";
                        }
                        if (newcFason || newcFijacion)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.NewFijac + toneladaPrecio.NewFason).ToString("N0") + "</td>";
                        }
                        if (newc != 0)
                        {
                            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.NewAFijar + toneladaPrecio.NewAPrecio + toneladaPrecio.NewFijac + toneladaPrecio.NewFason).ToString("N0") + "</td>";
                        }

                        htmlBody += "</tr>";
                    }
                }
                htmlBody += "</tbody>";
                htmlBody += "</table>";
            }

            htmlBody += "<br></br>";

            if (model.PosicionCompras.Any(x => x.Total > 0))
            {

                htmlBody += "<table  style=\"width:" + 25 * model.PosicionCompras.Count(x => x.Total > 0) + "%; border-collapse:unset;\">";
                htmlBody += "<tbody>";
                htmlBody += "<tr>";
            }
            foreach (var material in model.PosicionCompras)
            {

                if (material.Total > 0)
                {
                    switch (material.Material)
                    {
                        case "Soja":
                            colorPosicion = " background: #99cc00; ";
                            break;
                        case "Maíz":
                            colorPosicion = " background: #ffcc99 ;";
                            break;
                        case "Trigo Cámara":
                            colorPosicion = " background: #9999ff ;";
                            break;
                        case "Trigo Calidad":
                            colorPosicion = " background: #99ccff ;";
                            break;
                        default:
                            break;
                    }
                    htmlBody += "<td valign =\"top\">";
                    htmlBody += "<table  style=\"width:100%; border-collapse:unset;\">";
                    htmlBody += "<tbody>";

                    htmlBody += "<tr><th style=\" " + titulo + colorPosicion + "\" colspan = \"2\">" + material.Material.ToUpper() + "</th></tr>";
                    htmlBody += "<tr>";
                    htmlBody += "<th style=\" " + titulo + colorPosicion + "\">Posición</th>";
                    htmlBody += "<th style=\" " + titulo + colorPosicion + "\">Ton</th>";
                    htmlBody += "</tr>";
                    foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                    {
                        int mesActual = (int)(EnumMeses)Enum.Parse(typeof(EnumMeses), mes.Mes.ToString());
                        htmlBody += "<tr>";
                        htmlBody += "<td  style=\" " + datoCentro + " \" >" + mes.Mes + " - " + mes.Anio + "</td>";
                        htmlBody += "<td style=\" " + datoCentro + " \" >" + mes.KilosPesos.ToString("N0") + "</td>";
                        htmlBody += "</tr>";
                    }
                    htmlBody += "<tr>";
                    htmlBody += "<th style=\" " + datoIzquierda + " \" >Total</th>";
                    htmlBody += "<th style=\" " + datoCentro + " \">" + material.Total.ToString("N0") + "</th>";

                    htmlBody += "</tr>";
                    htmlBody += "</tbody>";
                    htmlBody += "</table>";
                    htmlBody += "</td>";

                }
            }
            if (model.PosicionCompras.Any(x => x.Total > 0))
            {
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table>";
                htmlBody += "<br></br>";
            }

            //if (model.AgenteCompras.ListaAgenteCompras.Count > 0)
            //{
            //    htmlBody += "<table  style=\"width:70%; border-collapse:unset;\" > ";
            //    htmlBody += "<tr>";
            //    htmlBody += "<th style=\" " + titulo + colorAgente + "\" colspan = \" " + (model.AgenteCompras.ListaOperadores.Count + 4) + " \">AGENTE DE COMPRAS</th>";
            //    htmlBody += "</tr>";
            //    htmlBody += "<tr>";
            //    htmlBody += "<th style=\" " + titulo + colorAgente + "\">Agente de Compra</th>";
            //    htmlBody += "<th style=\" " + titulo + colorAgente + "\">Producto</th>";
            //    htmlBody += "<th style=\" " + titulo + colorAgente + "\">Posición</th>";
            //    foreach (var op in model.AgenteCompras.ListaOperadores)
            //    {
            //        htmlBody += "<th style=\" " + titulo + colorAgente + "\">" + op.OperadorDesc + "</th>";
            //    }
            //    htmlBody += "<th style=\" " + titulo + colorAgente + "\">Total</th>";
            //    htmlBody += "</tr>";
            //    foreach (var agente in model.AgenteCompras.ListaAgenteCompras)
            //    {
            //        var pos = agente.Posicion.Split('.');
            //        htmlBody += "<tr>";

            //        htmlBody += "<td  style=\" " + datoCentro + " \" >" + agente.TipoAgenteDesc + "</td>";

            //        htmlBody += "<td  style=\" " + datoCentro + " \" >" + agente.MaterialDesc + "</td>";

            //        htmlBody += "<td  style=\" " + datoCentro + " \" >" + (EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1] + "</td>";
            //        foreach (var op in model.AgenteCompras.ListaOperadores)
            //        {
            //            var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
            //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (cantidad != null ? cantidad : "0") + "</td>";
            //        }
            //        htmlBody += "<td style=\" " + datoCentro + " \" >" + agente.Operador.Sum(x => x.Cantidad).ToString("N0") + "</td>";
            //        htmlBody += "</tr>";
            //    }
            //    htmlBody += "</table>";
            //}

            htmlBody += "<br></br>";

            if (model.SojaSustentable.Total > 0)
            {
                htmlBody += "<table style=\"width:50%; border-collapse:unset;\">";
                htmlBody += "<tbody>";
                htmlBody += "<tr>";
                htmlBody += "<th style=\" " + titulo + "\" colspan=\"" + (1 + Convert.ToInt32(model.SojaSustentable.Precio > 0) + Convert.ToInt32(model.SojaSustentable.Fijar > 0)) + "\">SOJA SUSTENTABLE</th>";
                htmlBody += "</tr>";
                htmlBody += "<tr>";
                if (model.SojaSustentable.Precio > 0)
                {
                    htmlBody += "<th  style=\" " + titulo + "\" >A Precio</th>";
                }
                if (model.SojaSustentable.Fijar > 0)
                {
                    htmlBody += "<th  style=\" " + titulo + "\" >A Fijar</th>";
                }
                htmlBody += "<th  style=\" " + titulo + "\" >Total</th>";
                htmlBody += "</tr>";
                htmlBody += "<tr>";
                if (model.SojaSustentable.Precio > 0)
                {
                    htmlBody += "<td  style=\" " + datoCentro + " \" >" + model.SojaSustentable.Precio.ToString("N0") + "</td>";
                }
                if (model.SojaSustentable.Fijar > 0)
                {

                    htmlBody += "<td style=\" " + datoCentro + " \" >" + model.SojaSustentable.Fijar.ToString("N0") + "</td>";
                }

                htmlBody += "<th style=\" " + datoCentro + " \" >" + model.SojaSustentable.Total.ToString("N0") + "</th>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table>";
            }

            htmlBody += "<br></br>";

            htmlBody += "<table style=\"width:50%; border-collapse:unset;\" >";
            htmlBody += "<tbody>";
            foreach (var moneda in model.PrecioCantidad)
            {
                if (moneda.Cantidad > 0)
                {
                    htmlBody += "<tr>";
                    htmlBody += "<th style=\" " + titulo + "\">" + moneda.Moneda + "</th>";
                    htmlBody += "<td  style=\" " + datoCentro + " \" >" + moneda.Cantidad.Value.ToString("N2") + "</td>";
                    htmlBody += "</tr>";
                }
            }
            htmlBody += "</tbody>";
            htmlBody += "</table>";

            htmlBody += "<br></br>";
            htmlBody += "Observaciones: " + (String.IsNullOrEmpty(observaciones) ? "Sin observaciones." : observaciones);

            return htmlBody;
        }

        private string GenerarCuerpoMail(string observaciones)
        {
            var hoy = DateTime.Now.Date;
            var Model = ObtenerDatosReporte(hoy, hoy, "0");
            var dispAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispAFijar > 0));
            var dispAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispAPrecio > 0));
            var dispFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.DispFijac > 0));
            var dispAgente = Model.ToneladasGranoTipo.Any(x => x.DispAgente != 0);
            var disp = 4 - (dispAFijar ? 0 : 1) - (dispAPrecio ? 0 : 1) - (dispFijacion ? 0 : 1) - (dispAgente ? 0 : 1);
            var forwAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwAFijar > 0));
            var forwAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwAPrecio > 0));
            var forwFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.FrwFijac > 0));
            var forwAgente = Model.ToneladasGranoTipo.Any(x => x.FrwAgente != 0);
            var forw = 4 - (forwAFijar ? 0 : 1) - (forwAPrecio ? 0 : 1) - (forwFijacion ? 0 : 1) - (forwAgente ? 0 : 1);
            var newcAFijar = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewAFijar > 0));
            var newcAPrecio = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewAPrecio > 0));
            var newcFijacion = Model.PosicionCompras.Any(x => x.PosicionKilos.Any(y => y.NewFijac > 0));
            var newcAgente = Model.ToneladasGranoTipo.Any(x => x.NewAgente != 0);
            var newc = 4 - (newcAFijar ? 0 : 1) - (newcAPrecio ? 0 : 1) - (newcFijacion ? 0 : 1) - (newcAgente ? 0 : 1);

            var hedgeMat = Model.HedgeMaterial.Any(x => x.Disponible != 0 || x.Forward != 0 || x.NewCrop != 0);
            var hedgeObj = Model.HedgeObjetivo.RemitirObjetivo != 0 && Model.HedgeObjetivo.PricingObjetivo != 0 ? 1 : 0;
            var pricing = Model.PricingCampania.Count != 0;

            var htmlBody = "";
            htmlBody += @"<style>
table {
   
    text-align: center;
    border-bottom: 2px solid black;
    cellspacing:0;
}

    table tr {
        border-top: 1px solid black;
        border-bottom: 1px solid black;
    }

.titulos {
    border-top: 2px solid black;
    border-bottom: 2px solid black;
    background: #017940;
    color: white;
}
.row {
    background: white;
}

#MonedaKilo th {
    background: #017940;
    color: white;
}

#MonedaKilo td {
    background: white;
    color: black;
}


#Soja .titulosPosicion {
    background: rgb(153, 204, 0);
}
#Maiz .titulosPosicion {
    background: rgb(255, 204, 153);
}
#MaizTable {
    z-index: 175;
}
#TrigoCámara .titulosPosicion {
    background: rgb(153, 153, 255);
}
#TrigoCámaraTable {
    z-index: 150;
}
#TrigoCalidad .titulosPosicion {
    background: rgb(153, 204, 255);
}
#TrigoCalidadTable {
    z-index: 125;
}
#TrigoGrado2 .titulosPosicion {
    background: rgb(106, 230, 190);
}
#TrigoGrado2Table {
    z-index: 100;
}
#Girasol .titulosPosicion {
    background: rgb(211, 96, 212);
}
#GirasolTable {
    z-index: 75;
}
#GirasolAltoOleico .titulosPosicion {
    background: rgb(244, 193, 247);
}

.trPar {
    background-color: #eaeaea;
}

.titulos.hedgemat {
    background: darkcyan;
}

.titulos.hedgeobj {
    background: plum;
}

.titulos.agente {
    background: gold;
    color: black;
}

.titulos.pricing {
    background: mediumvioletred;
}

#grillaAgente > .k-grid-header, #grillaAgente .k-grid-header .k-header {
    background-color: yellow;
    font-weight: bold;
}

#grilla > .k-grid-header, #grilla .k-grid-header .k-header {
    background-color: yellowgreen;
    font-weight: bold;
}



.espacio {
    background-color: white;
    border-top: 2px solid white;
    border-bottom: 2px solid white;
    border-right: 2px solid black;
    border-left:  1px solid  black;
}

</style>";//style

            htmlBody += "Estimados,";
            htmlBody += "<br></br>";
            htmlBody += "A continuación, se detallan las compras correspondientes al cierre del día.";
            htmlBody += "<br></br>";
            htmlBody += "<br></br>";

            if (hedgeMat)
            {
                htmlBody += @"<table style='' id='HedgeMaterial'>
                <tbody>
                    <tr class='titulos hedgemat'>
                        <th colspan='4'>HEDGE</th>
                    </tr>
                    <tr class='titulos hedgemat'>
                        <th>Producto</th>
                        <th>Disponible</th>
                        <th>Forward</th>
                        <th>New Crop</th>
                    </tr>";
                foreach (var mat in Model.HedgeMaterial)
                {
                    if (mat.Disponible != 0 || mat.Forward != 0 || mat.NewCrop != 0)
                    {
                        htmlBody += @"<tr>" +
                                "<td>" + mat.MaterialDescripcion + "</td>" +
                                "<td>" + mat.Disponible.ToString("N0") + "</td>" +
                                "<td>" + mat.Forward.ToString("N0") + "</td>" +
                                "<td>" + mat.NewCrop.ToString("N0") + "</td>" +
                            "</tr>";
                    }
                }
                htmlBody += @"</tbody></table><br><br>";
            }

            if (pricing)
            {
                htmlBody += @"<table style='' id='Pricing'>
                    <tr class='titulos pricing' style='cellspacing:0; background: mediumvioletred;border: 2px solid mediumvioletred;  color: white;'>
                        <th>MATERIAL</th>
                        <th>CAMPAÑA</th>
                        <th>PRICING</th>";
                htmlBody += Model.PricingCampania.Any(x => x.SanLorenzo > 0) ? "<th>SL</th>" : "";
                htmlBody += Model.PricingCampania.Any(x => x.Acopio > 0) ? "<th>Acopios</th>" : "";
                htmlBody += "</tr>";
                foreach (var datos in Model.PricingCampania)
                {
                    string sumaPricing;
                    switch (datos.Id)
                    {
                        case 11:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 12:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 21:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 22:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 31:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 32:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 41:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 42:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        case 51:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.DispAgente + x.FrwAgente)).ToString("N0");
                            break;
                        case 52:
                            sumaPricing = (Model.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + Model.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.NewAgente)).ToString("N0");
                            break;
                        default:
                            sumaPricing = "0";
                            break;
                    }
                    htmlBody += @"<tr>
                            <td>" + datos.Material + @"</td>
                            <td>" + datos.Campania + @" </td>
                            <td>" + sumaPricing + @"</td>";

                    htmlBody += "<td>" + (Model.PricingCampania.Any(x => x.SanLorenzo > 0 && x.Id == datos.Id) ? datos.SanLorenzo.ToString("N0") : "") + "</td>";
                    htmlBody += "<td>" + (Model.PricingCampania.Any(x => x.Acopio > 0 && x.Id == datos.Id) ? datos.Acopio.ToString("N0") : "") + "</td>";
                    htmlBody += "</tr>";
                }
                htmlBody += "</table> <br><br>";
            }

            if (disp + forw + newc != 0)
            {
                htmlBody += @" <table class='table-condensed noSideMargin' id='ComprasToneladasTipo'>
                <tbody>
                    <tr class='titulos'>
                        <td id='' class='borde-izquierdo'></td>";

                htmlBody += (disp > 0) ? "<th id='disponible' class='borde-derecho' colspan='" + disp + "'>DISPONIBLE</th>" +
                    "<th id='disponibleTotal' class='borde-derecho-oscuro' rowspan='2' colspan='2'>TOTAL DISPONIBLE</th>" : "";
                htmlBody += (forw > 0) ? "<th id='forward' class='borde-derecho' colspan='" + forw + "'>FORWARD</th>" +
                        "<th id='forwardTotal' class=' borde-derecho-oscuro' rowspan='2' colspan='2'>TOTAL FORWARD</th>" : "";
                htmlBody += (newc > 0) ? "<th id='newCrop' class='borde-derecho' colspan='" + newc + "'>NEW CROP</th>" +
                        "<th id='newCropTotal' class='borde-derecho-oscuro' rowspan='2'>TOTAL NEW CROP</th>" : "";

                htmlBody += "</tr>";
                htmlBody += "<tr class='titulos'>";
                htmlBody += "<th class='producto borde-izquierdo'>PRODUCTO</th>";
                htmlBody += dispAFijar ? "<th class='valores'>A Fijar</th>" : "";
                htmlBody += dispAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += dispFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += dispAgente ? "<th class='valores'>MAT</th>" : "";
                //htmlBody += disp > 0 ? "<td class=''></td>" : "";
                htmlBody += forwAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += forwFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += forwAgente ? "th class='valores'>MAT</th>" : "";
                //htmlBody += forw > 0 ? "<td class=''> </td>" : "";
                htmlBody += newcAFijar ? "<th class='valores'>A Fijar</th>" : "";
                htmlBody += newcAPrecio ? "<th class='valores'>A Precio</th>" : "";
                htmlBody += newcFijacion ? "<th class='valores'>Fijación</th>" : "";
                htmlBody += newcAgente ? "<th class='valores'>MAT</th>" : "";

                htmlBody += "</tr>";
                foreach (var toneladaPrecio in Model.ToneladasGranoTipo)
                {
                    var posicion = Model.PosicionCompras.Where(x => x.Material.ToLower() == toneladaPrecio.Material.ToLower());
                    var totalDisp = posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac + y.DispAFijar + y.DispAPrecio)).Sum() + toneladaPrecio.DispAgente;
                    var totalForw = posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar + y.FrwAPrecio + y.FrwFijac)).Sum() + toneladaPrecio.FrwAgente;
                    var totalNewC = posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar + y.NewAPrecio + y.NewFijac)).Sum() + toneladaPrecio.NewAgente;

                    if (totalDisp != 0 || totalForw != 0 || totalNewC != 0)
                    {
                        htmlBody += @"<tr>";
                        htmlBody += " <th class='borde-izquierdo'>"+toneladaPrecio.Material+"</th>";
                        htmlBody += dispAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += dispFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.DispFijac)).Sum().ToString("N0")) + " </td>" : "";
                        htmlBody += dispAgente ? "<td class=''>" + toneladaPrecio.DispAgente.ToString("N0") + " </td>" : "";
                        htmlBody += disp > 0 ? "<td class='borde-derecho-oscuro'>" + totalDisp.ToString("N0") + " </td>" : "";

                        htmlBody += disp > 0 ? "<td class=''></td>" : "";
                        htmlBody += forwAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.FrwFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += forwAgente ? "<td class=''>" + toneladaPrecio.FrwAgente.ToString("N0") + "</td>" : "";
                        htmlBody += forw > 0 ? "<td class='borde-derecho-oscuro'>" + totalForw.ToString("N0") + "</td>" : "";

                        htmlBody += forw > 0 ? "<td class=' '> </td>" : "";
                        htmlBody += newcAFijar ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcFijacion ? "<td class=''>" + (posicion.Select(x => x.PosicionKilos.Sum(y => y.NewFijac)).Sum().ToString("N0")) + "</td>" : "";
                        htmlBody += newcAgente ? "<td class=''>" + toneladaPrecio.NewAgente.ToString("N0") + " </td>" : "";
                        htmlBody += newc > 0 ? "<td class='borde-derecho-oscuro'>" + totalNewC.ToString("N0") + " </td>" : "";
                        htmlBody += "</tr>";
                    }
                }
                htmlBody += " </tbody>";
                htmlBody += "</table><br><br>";
            }


            foreach (var material in Model.PosicionCompras)
            {

                dispAFijar = material.PosicionKilos.Any(x => x.DispAFijar > 0);
                dispAPrecio = material.PosicionKilos.Any(x => x.DispAPrecio > 0);
                dispFijacion = material.PosicionKilos.Any(x => x.DispFijac > 0);
                forwAFijar = material.PosicionKilos.Any(x => x.FrwAFijar > 0);
                forwAPrecio = material.PosicionKilos.Any(x => x.FrwAPrecio > 0);
                forwFijacion = material.PosicionKilos.Any(x => x.FrwFijac > 0);
                newcAFijar = material.PosicionKilos.Any(x => x.NewAFijar > 0);
                newcAPrecio = material.PosicionKilos.Any(x => x.NewAPrecio > 0);
                newcFijacion = material.PosicionKilos.Any(x => x.NewFijac > 0);
                var totalPesos = material.PosicionKilos.Sum(y => y.KilosPesos);
                var totalDolares = material.PosicionKilos.Sum(y => y.KilosDolares);
                var fix = 3 - (dispFijacion ? 0 : 1) - (forwFijacion ? 0 : 1) - (newcFijacion ? 0 : 1);
                var aFijar = 3 - (dispAFijar ? 0 : 1) - (forwAFijar ? 0 : 1) - (newcAFijar ? 0 : 1);
                var aPrecio = 3 - (dispAPrecio ? 0 : 1) - (forwAPrecio ? 0 : 1) - (newcAPrecio ? 0 : 1);
                var suma = fix + aFijar + aPrecio + (totalPesos != 0 ? 1 : 0) + (totalDolares != 0 ? 1 : 0);
                var pondPesos = material.PosicionKilos.Any(x => x.PrecioPonderadoPesos > 0) ? 1 : 0;
                var pondDolares = material.PosicionKilos.Any(x => x.PrecioPonderadoDolares > 0) ? 1 : 0;
                var pond = pondPesos + pondDolares;
                if (suma > 0)
                {
                    htmlBody += @" <div id='" + material.Material.Replace(" ", "") + "Table' style='width:" + (suma <= 5 ? "50%" : "100%") + "'>" +
                    @"<table id='" + material.Material.Replace(" ", "") + @"' class='table table-condensed'>
                    <tbody>
                        <tr class='titulosPosicion'><th colspan = '" + (suma + pond + 3) + "'> " + material.Material.ToUpper() + @" </ th ></ tr >
                        <tr class='titulosPosicion'>
                            <th rowspan = '2'> Posición </th>";
                    htmlBody += fix > 0 ? "<th colspan='"+fix+"' class='borde-izq-der'>Fix</th>" : "";
                    htmlBody += aFijar > 0 ? "<th colspan = '" + aFijar + "' class='borde-izq-der'>A Fijar</th>" : "";
                    htmlBody += aPrecio > 0 ? "<th colspan = '" + aPrecio + "' class='borde-izq-der'>A Precio</th>" : "";
                    htmlBody += totalPesos != 0 ? "<th rowspan = '2' colspan='2' class=''>Ton. $</th>" : "";
                    htmlBody += pondPesos > 0 ? "<th rowspan = '2'  colspan='2' class=''>Precio $</th>" : "";
                    htmlBody += totalDolares != 0 ? "<th rowspan = '2'  colspan='2' class=''>Ton.USD</th>" : "";
                    htmlBody += pondDolares > 0 ? "<th rowspan = '2'   class=''>Precio USD</th>" : "";
                    
                    htmlBody += "</tr>";
                    htmlBody += "<tr class='titulosPosicion'>";
                    htmlBody += dispFijacion ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwFijacion ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcFijacion ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += dispAFijar ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwAFijar ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcAFijar ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += dispAPrecio ? "<th class='borde-izq-der'>Disponible</th>" : "";
                    htmlBody += forwAPrecio ? "<th class='borde-izq-der'>Forward</th>" : "";
                    htmlBody += newcAPrecio ? "<th class='borde-izq-der'>New Crop</th>" : "";
                    htmlBody += "</tr>";

                    foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                    {
                        int mesActual = (int)((EnumMeses)Enum.Parse(typeof(EnumMeses), mes.Mes.ToString()));
                        htmlBody += @"  <tr>
                        <td> " + mes.Mes + " - " + mes.Anio + "</td>";
                        htmlBody += dispFijacion ? "<td class='borde-izq-der'>" + mes.DispFijac.ToString("N0") + "</td>" : "";
                        htmlBody += forwFijacion ? "<td class='borde-izq-der'>" + mes.FrwFijac.ToString("N0") + "</td>" : "";
                        htmlBody += newcFijacion ? "<td class='borde-izq-der'>" + mes.NewFijac.ToString("N0") + "</td>" : "";
                        htmlBody += dispAFijar ? "<td class='borde-izq-der'>" + mes.DispAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += forwAFijar ? "<td class='borde-izq-der'>" + mes.FrwAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += newcAFijar ? "<td class='borde-izq-der'>" + mes.NewAFijar.ToString("N0") + "</td>" : "";
                        htmlBody += dispAPrecio ? "<td class='borde-izq-der'>" + mes.DispAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += forwAPrecio ? "<td class='borde-izq-der'>" + mes.FrwAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += newcAPrecio ? "<td class='borde-izq-der'>" + mes.NewAPrecio.ToString("N0") + "</td>" : "";
                        htmlBody += totalPesos != 0 ? "<td class='' colspan='2'> " + (mes.KilosPesos.ToString("N0")) + "</td>" : "";
                        htmlBody += pondPesos > 0 ? "<td class='' colspan='2'> " + (mes.PrecioPonderadoPesos.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += totalDolares != 0 ? "<td class='' colspan='2'> " + (mes.KilosDolares.ToString("N0")) + "</td>" : "";
                        htmlBody += pondDolares > 0 ? "<td class='' colspan='2'> " + (mes.PrecioPonderadoDolares.Value.ToString("N0")) + "</td>" : "";
                        htmlBody += "</tr>";
                    }
                    htmlBody += @"   <tr class='titulosPosicion total'>
                        <th>Total</th>";
                    htmlBody += dispFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.DispFijac).ToString("N0") + "</td>" : "";
                    htmlBody += forwFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.FrwFijac).ToString("N0") + "</td>" : "";
                    htmlBody += newcFijacion ? "<td class=''>" + material.PosicionKilos.Sum(y => y.NewFijac).ToString("N0") + "</td>" : "";
                    htmlBody += dispAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.DispAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += forwAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.FrwAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += newcAFijar ? "<td class=''>" + material.PosicionKilos.Sum(y => y.NewAFijar).ToString("N0") + "</td>" : "";
                    htmlBody += dispAPrecio ? "<td class='>" + material.PosicionKilos.Sum(y => y.DispAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += forwAPrecio ? "<td class='>" + material.PosicionKilos.Sum(y => y.FrwAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += newcAPrecio ? "<td class='>" + material.PosicionKilos.Sum(y => y.NewAPrecio).ToString("N0") + "</td>" : "";
                    htmlBody += totalPesos != 0 ? "<th class='' colspan='2'>" + totalPesos.ToString("N0") + "</th>" : "";
                    htmlBody += pondPesos > 0 ? "<td class='' colspan='2'></td>" : "";
                    htmlBody += totalDolares != 0 ? "<th class='' colspan='2'>" + totalDolares.ToString("N0") + "</th>" : "";
                    htmlBody += totalDolares != 0 ? "<td class='' colspan='2'></td>" : "";
                    //htmlBody += "<td></td>  <td></td>";
                    htmlBody += @"</tr>
                    </tbody>
                </table><br><br>
            </div>";


                }
            }

            if (Model.SojaSustentable.Total > 0)
            {
                htmlBody += "<table class='table-condensed ' id='SojaSustentable' style='width:50%;'>";
                htmlBody += "    <tbody>";
                htmlBody += "        <tr>";
                htmlBody += "            <th class='titulos' colspan='3'>SOJA SUSTENTABLE</th>";
                htmlBody += "        </tr>";
                htmlBody += "        <tr>";
                htmlBody += Model.SojaSustentable.Precio > 0 ? "<th class=''>A Precio</th>" : "";
                htmlBody += Model.SojaSustentable.Fijar > 0 ? "<th class=''>A Fijar</th>" : "";
                htmlBody += "            <th>Total</th>";
                htmlBody += "        </tr>";
                htmlBody += "        <tr>";
                htmlBody += Model.SojaSustentable.Precio > 0 ? "<td class=''>" + Model.SojaSustentable.Precio.ToString("N0") + "</td>" : "";
                htmlBody += Model.SojaSustentable.Fijar > 0 ? "<td class=''>" + Model.SojaSustentable.Fijar.ToString("N0") + "</td>  " : "";
                htmlBody += "            <th>" + Model.SojaSustentable.Total.ToString("N0") + "</th>";
                htmlBody += "        </tr>";
                htmlBody += "    </tbody>";
                htmlBody += "</table><br><br>";
            }

            htmlBody += @"<table class='table-condensed noSideMargin' id='MonedaKilo' style='width:50%;'>
                <tbody>";
            foreach (var moneda in Model.PrecioCantidad)
            {
                if (moneda.Cantidad > 0)
                {
                    htmlBody += "<tr>";
                    htmlBody += "<th>" + moneda.Moneda + "</th>";
                    htmlBody += "<td> " + moneda.Cantidad.Value.ToString("N2") + " </td>";
                    htmlBody += "</tr>";
                }

            }
            htmlBody += @"</tbody>
            </table><br><br>";

            htmlBody += "<br></br>";

            htmlBody += "<br></br>";
            htmlBody += "Observaciones: " + (String.IsNullOrEmpty(observaciones) ? "Sin observaciones." : observaciones);

            return htmlBody;
        }


        private ReporteCompraNetModel ObtenerDatosReporte(DateTime fechaDesde, DateTime fechaHasta, string centroId)
        {
            int idCentro = int.Parse(centroId);
            bool filtrarAcopio = idCentro == 0 || idCentro == 1;
            var agentes = filtrarAcopio ? mobjReportesManager.TraerAgenteDeCompra(fechaDesde, fechaHasta) : new List<AgenteCompraDto>();
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

            var objetivos = mobjReportesManager.TraerHedgeObjetivo(fechaDesde, fechaHasta);
            objetivos.PricingCumplido = objetivos.PricingCumplido;
            objetivos.PricingObjetivo = objetivos.PricingObjetivo;
            objetivos.RemitirCumplido = objetivos.RemitirCumplido;
            objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(fechaDesde, fechaHasta, idCentro),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(fechaDesde, fechaHasta, idCentro),
                PricingCampania = mobjReportesManager.TraerPricingCampania(fechaDesde, fechaHasta, idCentro),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(fechaDesde, fechaHasta, idCentro),
                HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(fechaDesde, fechaHasta)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = mobjReportesManager.TraerTcPromedio(fechaDesde, fechaHasta),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
            };
        }
    }

}