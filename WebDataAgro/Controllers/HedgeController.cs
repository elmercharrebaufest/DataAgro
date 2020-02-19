using Molinos.DataAgro.Entities.Common.Enums;
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

        public HedgeController(IHedgeManager oHedgeManager, IReportesManager mobjReportesManager, IDiferencialManager diferencialManager)
        {
            this.oHedgeManager = oHedgeManager;
            this.mobjReportesManager = mobjReportesManager;
            this.diferencialManager = diferencialManager;
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
        public ActionResult CerrarDia(bool mail)
        {
            mail = true;
            var mailEnviar = mail ? ExcelReporteCompleto.GenerarExcel(this.ObtenerDatosReporte(), mobjReportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true) : new byte[1];

            var diferencial = diferencialManager.TraerDiferencial();
            oHedgeManager.CerrarDia(GlobalVariables.ComercialId,
                                    mailEnviar,
                                    GlobalVariables.IdActiveDirectory,
                                    mail,
                                    GenerarCuerpoMail(),
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
            var agentes = mobjReportesManager.TraerAgenteDeCompra(DateTime.Now, DateTime.Now);
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            return new ReporteCompraNetModel
            {
                ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(DateTime.Now, DateTime.Now),
                SojaSustentable = mobjReportesManager.TraerToneladasSojaSust(DateTime.Now, DateTime.Now),
                PosicionCompras = mobjReportesManager.TraerPosicionCompras(DateTime.Now, DateTime.Now),
                PrecioCantidad = mobjReportesManager.TraerMonedaCantidad(DateTime.Now, DateTime.Now),
                HedgeMaterial = TransformarAModel(mobjReportesManager.TraerTodosHedgeMaterial(DateTime.Now, DateTime.Now)),
                HedgeObjetivo = mobjReportesManager.TraerHedgeObjetivo(DateTime.Now, DateTime.Now),
                TCPromedioDto = mobjReportesManager.TraerTcPromedio(DateTime.Now, DateTime.Now),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
            };
        }

        private string GenerarCuerpoMail()
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
            var hedgeObj = model.HedgeObjetivo.RemitirObjetivo != 0 && model.HedgeObjetivo.PricingObjetivo != 0;

            var titulo = " border: 1px solid black; background: #017940; color: white; ";
            var datoIzquierda = " font-weight:bold; border: 1px solid black; text-align:left; ";
            var datoCentro = " border: 1px solid black; text-align:center; ";
            var colorHedge = " background: #008b8b; ";
            var colorObjetivo = " background: #dda0dd; ";
            var colorPosicion = "";
            var colorAgente = " background: #FFD700; ";

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



            if (hedgeObj)
            {
                htmlBody += "  <table  style=\" width: 50%; border-collapse:unset;\">";
                htmlBody += "     <tr>";
                htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \"></th>";
                htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Dia</th>";
                htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Objetivo</th>";
                htmlBody += "     </tr>";
                htmlBody += "     <tr>";
                htmlBody += "         <th style=\"  " + titulo + colorObjetivo + "  \">Pricing</th>";
                htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.PricingCumplido.ToString("n0") + "</td>";
                htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.PricingObjetivo.ToString("n0") + "</td>";
                htmlBody += "     </tr>";
                htmlBody += "     <tr>";
                htmlBody += "         <th  style=\"  " + titulo + colorObjetivo + "  \">A remitir</th>";
                htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.RemitirCumplido.ToString("n0") + "</td>";
                htmlBody += "         <td style=\" " + datoCentro + " \" >" + model.HedgeObjetivo.RemitirObjetivo.ToString("n0") + "</td>";
                htmlBody += "     </tr>";
                htmlBody += " </table>";

                htmlBody += "<br></br>";

                htmlBody += " <table style=\" width: 25%; border-collapse:unset;\">";
                htmlBody += "     <tr>";
                htmlBody += "         <td style=\" " + datoCentro + " \" >Hedge TC: $" + model.TCPromedioDto.PromedioTC.ToString("N0") + "</td>";
                htmlBody += "         <td style=\" " + datoCentro + " \">$" + model.TCPromedioDto.TotalTC.ToString("N0") + "</td>";
                htmlBody += "     </tr>";
                htmlBody += "</table>";

            }

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

            if (model.AgenteCompras.ListaAgenteCompras.Count > 0)
            {
                htmlBody += "<table  style=\"width:70%; border-collapse:unset;\" > ";
                htmlBody += "<tr>";
                htmlBody += "<th style=\" " + titulo + colorAgente + "\" colspan = \" " + (model.AgenteCompras.ListaOperadores.Count + 4) + " \">AGENTE DE COMPRAS</th>";
                htmlBody += "</tr>";
                htmlBody += "<tr>";
                htmlBody += "<th style=\" " + titulo + colorAgente + "\">Agente de Compra</th>";
                htmlBody += "<th style=\" " + titulo + colorAgente + "\">Producto</th>";
                htmlBody += "<th style=\" " + titulo + colorAgente + "\">Posición</th>";
                foreach (var op in model.AgenteCompras.ListaOperadores)
                {
                    htmlBody += "<th style=\" " + titulo + colorAgente + "\">" + op.OperadorDesc + "</th>";
                }
                htmlBody += "<th style=\" " + titulo + colorAgente + "\">Total</th>";
                htmlBody += "</tr>";
                foreach (var agente in model.AgenteCompras.ListaAgenteCompras)
                {
                    var pos = agente.Posicion.Split('.');
                    htmlBody += "<tr>";

                    htmlBody += "<td  style=\" " + datoCentro + " \" >" + agente.TipoAgenteDesc + "</td>";

                    htmlBody += "<td  style=\" " + datoCentro + " \" >" + agente.MaterialDesc + "</td>";

                    htmlBody += "<td  style=\" " + datoCentro + " \" >" + (EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1] + "</td>";
                    foreach (var op in model.AgenteCompras.ListaOperadores)
                    {
                        var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                        htmlBody += "<td style=\" " + datoCentro + " \" >" + (cantidad != null ? cantidad : "0") + "</td>";
                    }
                    htmlBody += "<td style=\" " + datoCentro + " \" >" + agente.Operador.Sum(x => x.Cantidad).ToString("N0") + "</td>";
                    htmlBody += "</tr>";
                }
                htmlBody += "</table>";
            }

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


            return htmlBody;
        }
    }
}