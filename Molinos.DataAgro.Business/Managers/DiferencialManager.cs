using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Molinos.DataAgro.Business
{

    public class DiferencialManager : IDiferencialManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private IMailManager mailManager;
        private IReportesManager mobjReportesManager;
        private IHedgeManager hedgeManager;
    

        public DiferencialManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, 
            IReportesManager mobjReportesManager, IHedgeManager hedgeManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.mobjReportesManager = mobjReportesManager;
            this.hedgeManager = hedgeManager;
        }

        public DiferencialDto TraerDiferencial()
        {
            var diferencial = repositorio.ObtenerMayor<Diferencial, int, DiferencialDto>(x => true, x => x.Id, x =>
                                          new DiferencialDto
                                          {
                                              Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                                              DiferencialDefault = x.DiferencialDefault,
                                              Id = x.Id,
                                              Fecha = x.Fecha,
                                              TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                                              TipoNegocioId = (int)x.TipoNegocioId
                                          });
            if (diferencial != null)
            {
                diferencial.historialDiferencial = TraerHistorial();
            }          
            
            return diferencial;
        }

        public DiferencialDto TraerDiferencial(int tipoNegocioId)
        {
            var diferencial = repositorio.ObtenerMayor<Diferencial, int, DiferencialDto>(x => x.TipoNegocioId == tipoNegocioId, x => x.Id, x =>
                                          new DiferencialDto
                                          {
                                              Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                                              DiferencialDefault = x.DiferencialDefault,
                                              Id = x.Id,
                                              Fecha = x.Fecha,
                                              TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                                              TipoNegocioId = (int)x.TipoNegocioId
                                          });
            if (diferencial != null)
            {
                diferencial.historialDiferencial = TraerHistorial();
            }

            return diferencial;
        }

        public Resultado GrabarDiferencial(Diferencial diferencial)
        {
            var resultado = new Resultado();
            if (diferencial.DiferencialDefault == 0)
            {
                resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser cero"));
                return resultado;
            }
            var anterior = repositorio.ObtenerMayor<Diferencial, int>(x => x.TipoNegocioId == diferencial.TipoNegocioId, x => x.Id);
            if (anterior != null)
            {
                if (diferencial.DiferencialDefault == anterior.DiferencialDefault)
                {
                    resultado.Errores.Add(new ErrorMessage(400, "El diferencial no puede ser igual al activo"));
                    return resultado;
                }
            }
            repositorio.Agregar<Diferencial>(diferencial);
            repositorio.GuardarCambios();
            return resultado;
        }

        public Resultado EliminarDiferencial(int id)
        {
            var resultado = new Resultado();
            var diferencial = repositorio.Obtener<Diferencial>(id);
            repositorio.Remover(diferencial);
            repositorio.GuardarCambios();
            return resultado;
        }

        private List<DiferencialDto> TraerHistorial(int ultimosN = 20)
        {
            return repositorio.Listar<Diferencial, DiferencialDto>(x => new DiferencialDto
            {
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                DiferencialDefault = x.DiferencialDefault,
                Id = x.Id,
                Fecha = x.Fecha,
                TipoNegocioDescripcion = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            }, null, ultimosN, "Fecha", Entities.Helpers.DirOrden.Desc);
        }

        public Resultado ValidarComprasDiferencial(int comercialId)
        {
            try
            {
                if (hedgeManager.Dia() != null)
                {
                    logger.Debug("hedgeManager.Dia() "+ hedgeManager.Dia().ToJson());
                    var cantidadAFijar = repositorio.Listar<Contrato, double>(x => x.Cantidad,
                        x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(DateTime.Now)
                        && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.TipoNegocioId == 1
                        && x.FinDelDia == null).Sum();

                    var cantidadAPrecio = repositorio.Listar<Contrato, double>(x => x.Cantidad,
                        x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(DateTime.Now)
                        && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.TipoNegocioId == 2
                        && x.FinDelDia == null).Sum();

                    logger.Debug("cantidadAFijar "+ cantidadAFijar);
                    logger.Debug("cantidadAPrecio " + cantidadAPrecio);
                    if (cantidadAFijar > (this.TraerDiferencial(1).DiferencialDefault * 1000))
                    {
                        logger.Debug("envia mail a fijar" );
                        var cuerpo = hedgeManager.GenerarCuerpoMail("");
                        logger.Debug("GenerarCuerpoMail ");

                        mailManager.ReenviarMailCierreDia("Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            "Actualización Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            cuerpo);
                        logger.Debug("ya envio ");
                    }
                    else if (cantidadAPrecio > (this.TraerDiferencial(2).DiferencialDefault * 1000))
                    {
                        logger.Debug("envia mail a precio");
                        var cuerpo = hedgeManager.GenerarCuerpoMail("");
                        logger.Debug("GenerarCuerpoMail ");

                        mailManager.ReenviarMailCierreDia("Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            "Actualización Cierre del dia " + DateTime.Now.Day + "/" + DateTime.Now.Month,
                                                            cuerpo);
                        logger.Debug("ya envio ");
                    }
                }
                return new Resultado();

            }
            catch (Exception e)
            {
                var resultado = new Resultado();
                resultado.Errores.Add(new ErrorMessage(400, "Error en validar compras diferencial"));
                return resultado;

            }

        }

        //private string GenerarCuerpoMailCierreDiaExcedidoCantidad()
        //{
        //    var model = this.ObtenerDatosReporte();

        //    var dispAFijar = model.ToneladasGranoTipo.Any(x => x.DispAFijar > 0);
        //    var dispAPrecio = model.ToneladasGranoTipo.Any(x => x.DispAPrecio > 0);
        //    var dispFijacion = model.ToneladasGranoTipo.Any(x => x.DispFijac > 0);
        //    var dispFason = model.ToneladasGranoTipo.Any(x => x.DispFason > 0);
        //    var disp = 4 - (dispAFijar ? 0 : 1) - (dispAPrecio ? 0 : 1) - (dispFijacion ? 0 : 1) - (dispFason ? 0 : 1);
        //    var forwAFijar = model.ToneladasGranoTipo.Any(x => x.FrwAFijar > 0);
        //    var forwAPrecio = model.ToneladasGranoTipo.Any(x => x.FrwAPrecio > 0);
        //    var forwFijacion = model.ToneladasGranoTipo.Any(x => x.FrwFijac > 0);
        //    var forwFason = model.ToneladasGranoTipo.Any(x => x.FrwFason > 0);
        //    var dispFwd = dispAPrecio || dispFijacion || forwAPrecio || forwFijacion;
        //    var forw = 4 - (forwAFijar ? 0 : 1) - (forwAPrecio ? 0 : 1) - (forwFijacion ? 0 : 1) - (forwFason ? 0 : 1);
        //    var newcAFijar = model.ToneladasGranoTipo.Any(x => x.NewAFijar > 0);
        //    var newcAPrecio = model.ToneladasGranoTipo.Any(x => x.NewAPrecio > 0);
        //    var newcFijacion = model.ToneladasGranoTipo.Any(x => x.NewFijac > 0);
        //    var newcFason = model.ToneladasGranoTipo.Any(x => x.NewFason > 0);
        //    var newc = 4 - (newcAFijar ? 0 : 1) - (newcAPrecio ? 0 : 1) - (newcFijacion ? 0 : 1) - (newcFason ? 0 : 1);

        //    var titulo = " border: 1px solid black; background: #017940; color: white; ";
        //    var datoIzquierda = " font-weight:bold; border: 1px solid black; text-align:left; ";
        //    var datoCentro = " border: 1px solid black; text-align:center; ";


        //    var htmlBody = "";

        //    htmlBody += "Estimados,";
        //    htmlBody += "<br></br>";
        //    htmlBody += "A continuación, se detallan los diferenciales del día.";
        //    htmlBody += "<br></br>";
        //    htmlBody += "<br></br>";

        //    htmlBody += "<table style=\"width:100%; border-collapse:unset;\">";
        //    htmlBody += "<tbody>";
        //    htmlBody += "<tr>";
        //    htmlBody += "<td style=\"  " + titulo + " \"></td>";

        //    htmlBody += "<th  style=\"  " + titulo + " \" colspan=\" " + 3 + "\">DISPONIBLE</th>";
        //    htmlBody += "<th  style=\"  " + titulo + " \" rowspan=\" " + 2 + "\">TOTAL DISP</th>";

        //    htmlBody += "<th  style=\" " + titulo + "\" colspan=\" " + 3 + "\">FORWARD</th>";
        //    htmlBody += "<th  style=\" " + titulo + "\" rowspan=\" " + 2 + "\">TOTAL FORWARD</th>";

        //    htmlBody += "<th  style=\" " + titulo + "\" colspan=\" " + 3 + "\">NEW CROP</th>";
        //    htmlBody += "<th  style=\" " + titulo + "\" rowspan=\" " + 2 + "\">TOTAL NEW CROP</th>";

        //    htmlBody += "</tr>";
        //    htmlBody += "<tr>";
        //    htmlBody += "<th  style=\" " + titulo + "\">PRODUCTO</th>";

        //    htmlBody += "<th  style=\" " + titulo + "\" >A Fijar</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";



        //    htmlBody += "<th style=\" " + titulo + "\">A Fijar</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";



        //    htmlBody += "<th style=\" " + titulo + "\">A Fijar</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">A Precio</th>";

        //    htmlBody += "<th style=\" " + titulo + "\">Fijación</th>";




        //    htmlBody += "</tr>";

        //    foreach (var toneladaPrecio in model.ToneladasGranoTipo)
        //    {
        //        if (toneladaPrecio.Total > 0)
        //        {
        //            htmlBody += "<tr>";
        //            htmlBody += " <th  style=\" " + datoIzquierda + " \" >" + toneladaPrecio.Material + "</th>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.DispAFijar.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.DispAPrecio.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.DispFijac + toneladaPrecio.DispFason).ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.DispAFijar + toneladaPrecio.DispAPrecio + toneladaPrecio.DispFijac + toneladaPrecio.DispFason).ToString("N0") + "</td>";

        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.FrwAFijar.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.FrwAPrecio.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.FrwFijac + toneladaPrecio.FrwFason).ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.FrwAFijar + toneladaPrecio.FrwAPrecio + toneladaPrecio.FrwFijac + toneladaPrecio.FrwFason).ToString("N0") + "</td>";

        //            htmlBody += "<td style=\" " + datoCentro + " \" > " + toneladaPrecio.NewAFijar.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + toneladaPrecio.NewAPrecio.ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.NewFijac + toneladaPrecio.NewFason).ToString("N0") + "</td>";
        //            htmlBody += "<td style=\" " + datoCentro + " \" >" + (toneladaPrecio.NewAFijar + toneladaPrecio.NewAPrecio + toneladaPrecio.NewFijac + toneladaPrecio.NewFason).ToString("N0") + "</td>";

        //            htmlBody += "</tr>";
        //        }
        //    }
        //    htmlBody += "</tbody>";
        //    htmlBody += "</table>";


        //    htmlBody += "<br></br>";

        //    return htmlBody;
        //}

        //private ReporteCompraNetDto ObtenerDatosReporte()
        //{
        //    return new ReporteCompraNetDto
        //    {
        //        ToneladasGranoTipo = mobjReportesManager.TraerToneladasGranoTipo(DateTime.Now, DateTime.Now,null)
        //    };
        //}

    }
}

