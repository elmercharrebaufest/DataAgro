using Autofac.Extras.NLog;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Molinos.DataAgro.Business.Managers
{
    public class BoletoManager : IBoletoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IContratosConfirmadosAgent oContratosConfirmadosAgent;
        private readonly IEnviarBoletoAgent oEnviarBoletoAgent;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;
        private readonly IMailManager mailManager;
        private readonly IHttpContextManager httpContextManager;
        private readonly IServicioClausulas servicioClausula;
        private readonly IStatusContratoAgent status;

        public BoletoManager(IRepositorio repositorio, ILogger logger, IContratosConfirmadosAgent oContratosConfirmadosAgent, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent,
            IEnviarBoletoAgent oEnviarBoletoAgent, IMailManager mailManager, IHttpContextManager httpContextManager, IServicioClausulas servicioClausula, IStatusContratoAgent status)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.oContratosConfirmadosAgent = oContratosConfirmadosAgent;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
            this.oEnviarBoletoAgent = oEnviarBoletoAgent;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
            this.servicioClausula = servicioClausula;
            this.status = status;
        }

        public BoletoResult GrabarBoleto(List<string> contratos, List<int> tipoNegocios, BoletoDto boletoContrato, List<int> equipo)
        {
            var boletoResult = new BoletoResult();
            try
            {
                var basicoContratos = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, equipo));
                var negociosHabilitados = FiltrarNegociosHabilitados(basicoContratos);

                foreach (string itemContrato in contratos)
                {
                    var habilitado = negociosHabilitados.Exists(n => n.Negocio.Contains(itemContrato) || n.ContratoSAP.Contains(itemContrato));
                    if (!habilitado)
                    {
                        boletoResult.BoletosDto.Add(DevolverDto(new BasicoContrato { ContratoSAP = itemContrato }, false, 0, "El negocio no está habilitado para generar boleto."));
                    }
                }
                foreach (var negocio in negociosHabilitados)
                {
                    if (!tipoNegocios.Exists(x => x == negocio.TipoNegocioId))
                    {
                        boletoResult.BoletosDto.Add(DevolverDto(negocio, false, 0, "El negocio no corresponde al tipo de negocio indicado."));
                    }
                    else
                    {
                        var boletoDto = ValidarNegocioParaGenerarBoleto(negocio);
                        boletoDto.ComercialId = boletoContrato.ComercialId;

                        if (!string.IsNullOrEmpty(boletoDto.Mensaje))
                        {
                            boletoDto.Generado = false;
                            boletoResult.BoletosDto.Add(boletoDto);
                            continue;
                        }

                        logger.Debug("Enviando boleto " + boletoDto.ToString());
                        var resultado = oEnviarBoletoAgent.EnviarBoleto(boletoDto);
                        if (resultado == "Se actualizan correctamente los datos")
                        {
                            boletoDto.Generado = true;
                            var guardarBoleto = repositorio.Agregar(ConvertirBoletoDtoAEntidad(boletoDto));
                            boletoResult.BoletosGenerados.Add(guardarBoleto);
                            List<ResultadoClausula> clausulas = new List<ResultadoClausula>();
                            if (boletoContrato.Clausulas.Any())
                            {
                                int orden = 1;
                                clausulas = boletoContrato.Clausulas.Select(x => new ResultadoClausula { Texto = x, Orden = orden++ }).ToList();
                            }
                            else
                            {
                                clausulas = ObtenerClausulas(negocio);
                            }
                            var pdf = GenerarPDF(negocio, clausulas, boletoDto);
                            if (boletoContrato.Mail)
                            {
                                try
                                {
                                    var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == (negocio.CorredorId != 0 ? negocio.CorredorId : negocio.ProveedorId) && x.Boleto == true);
                                    var comercial = repositorio.Obtener<Comercial>(boletoContrato.ComercialId);
                                    EnviarMailBoleto(negocio.BoletoDescripcion, String.IsNullOrEmpty(negocio.RazonSocialCorredor) ? negocio.RazonSocialProveedor : negocio.RazonSocialCorredor,
                                        negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.Negocio.Substring(negocio.Negocio.Length - 2) : negocio.ContratoSAP.TrimStart('0'),
                                        boletoDto.Version.ToString(), comercial, emailproveedor, pdf, negocio.ContratoSAP.TrimStart('0') + "_V" + boletoDto.Version.ToString().PadLeft(2, '0'));
                                }
                                catch (Exception ex)
                                {
                                    boletoDto.Mensaje += "Error al enviar el email del boleto: " + ex.Message;
                                    logger.Error(ex);
                                }
                            }
                            try
                            {
                                File.WriteAllBytes(ConfigurationManager.AppSettings["PathBoletos"].ToString() + "\\"
                                     + (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (negocio.ContratoSAP + "_F" + negocio.Negocio.Substring(negocio.Negocio.Length - 3, 2)) : (negocio.ContratoSAP.TrimStart('0') + "_V" + boletoDto.Version.ToString().PadLeft(2, '0'))) + ".pdf", pdf);
                            }
                            catch (Exception ex)
                            {
                                boletoDto.Mensaje += "Error al guardar el PDF del boleto: " + ex.Message;
                                logger.Error(ex);
                            }
                            boletoDto.Mensaje = "El boleto se generó correctamente.";
                            boletoResult.BoletosDto.Add(boletoDto);
                        }
                        else
                        {
                            boletoDto.Generado = false;
                            boletoDto.Mensaje = resultado;
                            boletoResult.BoletosDto.Add(boletoDto);
                        }
                    }
                }

                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                boletoResult.Error("Error en GrabarBoleto", e.Message);
            }

            return boletoResult;
        }

        private static Boleto ConvertirBoletoDtoAEntidad(BoletoDto tempBoleto)
        {
            return new Boleto
            {
                NegocioId = tempBoleto.NegocioId,
                Version = tempBoleto.Version,
                FechaGeneracion = tempBoleto.FechaGeneracion,
                ComercialId = tempBoleto.ComercialId
            };
        }

        public string ValidarNegocio(string negocioSAP, List<int> equipo)
        {
            var contratos = new List<string> { negocioSAP };
            var basicoContratos = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, equipo));
            var contrato = FiltrarNegociosHabilitados(basicoContratos).FirstOrDefault();
            var mensaje = contrato is null ? "No encontrado" : ValidarNegocioParaGenerarBoleto(contrato).Mensaje;
            return mensaje;
        }

        public byte[] BoletoEnByte(string archivoUrl)
        {
            try
            {
                FileStream stream = File.OpenRead(archivoUrl);
                byte[] fileBytes = new byte[stream.Length];

                stream.Read(fileBytes, 0, fileBytes.Length);
                stream.Close();
                //return System.Text.Encoding.UTF8.GetString(fileBytes);
                return fileBytes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ObtenerIdentDescarga()
        {
            DateTime oNow = DateTime.Now;
            string strFechaHora = oNow.ToString("yyyyMMddHHmmss");
            string strTicks = oNow.Ticks.ToString();
            string strIdent = strFechaHora + strTicks;
            return strIdent;
        }

        private static BoletoDto DevolverDto(BasicoContrato itemNegocio, bool generado, int version, string mensaje)
        {
            return new BoletoDto
            {
                ContratoSAP = Convert.ToInt64(itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.Negocio : itemNegocio.ContratoSAP).ToString(),
                Generado = generado,
                Version = version,
                Mensaje = mensaje
            };
        }

        private List<BasicoContrato> FiltrarNegociosHabilitados(IQueryable<BasicoContrato> negocios)
        {
            List<TipoNegocioDetalle> tipoNegocioDetalles = repositorio.Listar<TipoNegocioDetalle>();
            List<BasicoContrato> negociosFiltrados = new List<BasicoContrato>();

            foreach (var negocio in negocios)
            {
                foreach (var tipo in tipoNegocioDetalles)
                {
                    if (tipo.Descripcion == negocio.TipoNegocio)
                    {
                        if (negocio.BoletoContratoId == (int)EnumBoletoCompraNet.CONFIRMA && tipo.Confirma)
                        {
                            negociosFiltrados.Add(negocio);
                        }
                        if (negocio.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && tipo.BoletoFisico)
                        {
                            negociosFiltrados.Add(negocio);
                        }
                        if (negocio.BoletoContratoId == (int)EnumBoletoCompraNet.CARTA_OFERTA && tipo.CartaOferta)
                        {
                            negociosFiltrados.Add(negocio);
                        }
                    }
                }
            }
            return negociosFiltrados;
        }

        public DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null)
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.tiponegocio.Add(new TipoNegocioQry { TipoNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.tiponegocio.Add(new TipoNegocioQry { TipoNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        private void EnviarMailBoleto(string boletoDescripcion, string razonSocial, string contrato, string version, Comercial comercial, List<string> emailproveedor, byte[] pdf, string nombrePDF)
        {
            var lista = new List<string>();

            var comercialRegistrado = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);

            if (!PermisosHelper.Is(PermisosDataAgro.NoRecibirMail))
            {
                lista.Add(comercialRegistrado);
                lista.Add("dataagro@molinosagro.com.ar");
                logger.Debug("Enviando mail Boleto a " + comercialRegistrado);
            }
            var subject = boletoDescripcion == "Físico" ? "Boleto Físico" : boletoDescripcion;
            subject += " Molinos Agro S.A. – " + razonSocial + " - Contrato Nro. " + contrato;

            mailManager.EnviarMail(comercial, emailproveedor, subject, "", lista, CuerpoMailBoleto(httpContextManager.ObtenerPathLogoMail(), contrato, version), pdf, nombrePDF + ".pdf");
        }

        private AlternateView CuerpoMailBoleto(String filePath, string contrato, string version)
        {
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            string htmlBody = "Se le envía por este medio el boleto de compraventa de granos número " + contrato + " de Molinos Agro S.A., versión " + version +
                ". Por favor imprimir con todas las copias incluidas (doble faz), firmar y subir a la web de www.moaoperaciones.com.ar y luego enviar a nuestras oficinas. <br />";
            htmlBody += "En caso de ser un boleto de Bolsa de Rosario, si no se envía impreso en doble faz se observará debido a que no están autorizando el obleado.<br/>" +
                "<br/>En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales," +
                "<br/><br/>Molinos Agro S.A.<br/><br/>" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br/>www.molinosagro.com.ar";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);

            return alternateView;
        }

        private void Inicializar(BasicoContrato basico)
        {
            var generalPorFuera = new DescuentoBonificacionDto()
            {
                Importe = 0,
                Porcentaje = 0,
                TipoPeriodoDBId = (int)EnumTipoPeriodoDB.GENERALES,
                TipoDBId = (int)EnumTipoDB.POR_FUERA_DEL_PRECIO
            };
            var generalSobre = new DescuentoBonificacionDto()
            {
                Importe = 0,
                Porcentaje = 0,
                TipoPeriodoDBId = (int)EnumTipoPeriodoDB.GENERALES,
                TipoDBId = (int)EnumTipoDB.SOBRE_EL_PRECIO
            };
            if (basico.Descuentos == null)
            {
                basico.Descuentos = new List<DescuentoBonificacionDto> { generalPorFuera, generalSobre };
            }
            else
            {
                var descuentoGeneralFueraPrecio = basico.Descuentos.Where(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && x.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO).FirstOrDefault();
                if (descuentoGeneralFueraPrecio == null)
                {
                    basico.Descuentos.Add(generalPorFuera);
                }
                var descuentoGeneralSobrePrecio = basico.Descuentos.Where(x => x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && x.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO).FirstOrDefault();
                if (descuentoGeneralSobrePrecio == null)
                {
                    basico.Descuentos.Add(generalSobre);
                }
            }
        }

        private List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico)
        {
            var clausulas = repositorio.Listar<Clausula>();
            var result = new List<ResultadoClausula>();
            var orden = 1;
            Inicializar(basico);
            foreach (var item in clausulas)
            {
                item.Basico = basico;
                var clausula = servicioClausula.DevolverClausulas(item);
                if (clausula != null && !string.IsNullOrEmpty(clausula.Texto))
                {
                    if ((basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) && item.DisplayName.Equals("Clausula Diez")) continue;//es clausula Diez y es Precio Establecido(No es precio a Fijar) SALTAR esta iteracion
                    //if (item.DisplayName.Equals("Clausula Veinte") && basico.CorredorId > 0) continue;//es clausula Veinte y tiene corredor(No es operacion directa) SALTAR esta clausula
                    clausula.Orden = orden++;
                    result.Add(clausula);
                }
            }
            return result.OrderBy(x => x.Orden).ToList();
        }

        private byte[] GenerarPDF(BasicoContrato basico, List<ResultadoClausula> clausulas, BoletoDto boleto)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    using (var document = new Document(PageSize.A4, 10f, 10f, 10f, 100f))
                    {
                        string templateFilePath = ObtenerPath(basico);

                        var templateString = System.IO.File.ReadAllText(templateFilePath);

                        var xHtml = templateString;
                        xHtml = CompletarHtml(basico, clausulas, boleto, xHtml);

                        var PdfWriter = iTextSharp.text.pdf.PdfWriter.GetInstance(document, stream);
                        document.Open();

                        // Our custom Header and Footer is done using Event Handler
                        //if (basico.BoletoId == 2 && basico.BolsaId == 2)
                        //{
                        BoletoHeaderFooter PageEventHandler = new BoletoHeaderFooter(basico);
                        PdfWriter.PageEvent = PageEventHandler;
                        //}

                        var tagProcessors = (DefaultTagProcessorFactory)Tags.GetHtmlTagProcessorFactory();
                        tagProcessors.RemoveProcessor(HTML.Tag.IMG); // remove the default processor
                        tagProcessors.AddProcessor(HTML.Tag.IMG, new Molinos.DataAgro.Entities.Helpers.CustomImageTagProcessor()); // use our new processor

                        CssFilesImpl cssFiles = new CssFilesImpl();
                        cssFiles.Add(XMLWorkerHelper.GetInstance().GetDefaultCSS());
                        var cssResolver = new StyleAttrCSSResolver(cssFiles);
                        cssResolver.AddCss(@"code { padding: 2px 4px; }", "utf-8", true);
                        var charset = Encoding.UTF8;
                        var hpc = new HtmlPipelineContext(new CssAppliersImpl(new XMLWorkerFontProvider()));
                        hpc.SetAcceptUnknown(true).AutoBookmark(true).SetTagFactory(tagProcessors); // inject the tagProcessors
                        var htmlPipeline = new HtmlPipeline(hpc, new PdfWriterPipeline(document, PdfWriter));
                        var pipeline = new CssResolverPipeline(cssResolver, htmlPipeline);
                        var worker = new XMLWorker(pipeline, true);
                        var xmlParser = new XMLParser(true, worker, charset);
                        xmlParser.Parse(new StringReader(xHtml));
                        document.Close();
                        byte[] bytes = stream.ToArray();
                        stream.Close();
                        return bytes;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al generar PDF de boleto. ", e);
                throw;
            }
        }

        private static string ObtenerPath(BasicoContrato basico)
        {
            if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.ROSARIO)
            {
                return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/BoletoFisico.html");
            }
            if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.BS_AS)
            {
                return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/BoletoFisicoBuenosAires.html");
            }
            if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                return basico.CorredorId == 0 ?
                    Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/CartaOfertaSinCorredor.html") :
                    Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/CartaOferta.html");
            }

            return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/BoletoFisico.html");
        }

        private string CompletarHtml(BasicoContrato basico, List<ResultadoClausula> clausulas, BoletoDto boleto, string xHtml)
        {
            string clausulashtml = String.Join("", clausulas.OrderBy(a => a.Orden).Select(a => "<br />" + a.Orden + " . " + a.Texto).ToList());
            var stylesHtml = @"<style type='text/css'>
        .cls_003 {
            font-family: Arial,serif;
            font-size: 12.1px;
            color: rgb(255,255,255);
            font-weight: bold;
            font-style: normal;
            text-decoration: none;
            background-color: black;
            text-align: center;
            top: -59px;
            position: relative;
            left: -1px;
            width: 102%;
        }

        .cls_002 {
            font-family: Arial,serif;
            font-size: 14.1px;
            color: rgb(0,0,0);
            font-weight: bold;
            font-style: italic;
            text-decoration: none
        }

        .border, td {
            border-collapse: collapse;
            border: 1px solid black;
        }

        .noborder {
            border-collapse: collapse;
            border: 1px solid white;
        }

        .cls_005 {
            font-family: Arial,serif;
            font-size: 11.1px;
            color: rgb(0,0,0);
            font-weight: bold;
            font-style: normal;
            text-decoration: none;
        }
        .cls_005_top {
            vertical-align: top;
        }

        .cls_006 {
            font-family: Arial,serif;
            font-size: 10px;
            color: rgb(0,0,0);
            font-weight: normal;
            font-style: normal;
            text-decoration: none;
        }

        .cls_008 {
            font-family: Arial,serif;
            font-size: 12px;
            color: rgb(0,0,0);
            font-weight: normal;
            font-style: normal;
            text-decoration: none;
        }

        .cls_009 {
            font-family: Arial,serif;
            font-size: 11.1px;
            color: rgb(0,0,0);
            font-weight: bold;
            font-style: normal;
            text-decoration: none;
            text-align: center;
        }

        .cls_011 {
            font-family: Courier New,serif;
            font-size: 10.1px;
            color: rgb(0,0,0);
            font-weight: normal;
            font-style: normal;
            text-decoration: none
        }

        .cls_clausulas {
             font-family: Arial,serif;
             color: rgb(0,0,0);
             font-size: 10px;
             text-align: justify;
             line-height: 1.15;
             margin-top: 0px;
        }
        .cls_clausulas.cls_clausulas_co {
            font-size: 8px !important;
        }
        .cls_footer_co {
            height:4em !important;
        }
        .espacio {
            height: 10px;
            display: block;
        }

        .w33 {
            width: 30%;
            display: inline-block;
        }
        .cls_012 {
            font-family: Arial,serif;
            font-size: 12px;
            color: rgb(0,0,0);
            text-align: justify;
        }
        .cls_pdt_1 {
            height: 15px !important;
        }

    </style>";

            if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.ROSARIO)
            {
                var precio = "";
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    var simboloMoneda = basico.Moneda == "ARP" ? "ARS" : basico.Moneda;
                    precio = simboloMoneda + " " + basico.PrecioNeto?.ToString("N", new CultureInfo("es-AR"));
                }
                else
                {
                    precio = "A Fijar";
                }
                var titulo = "";
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    titulo = "Boleto de compra venta para cereales y oleaginosos";
                }
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && basico.Canje != true)
                {
                    titulo = "Boleto de compra venta de granos a fijar precio";
                }
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && basico.Canje == true)
                {
                    titulo = "Boleto de compra venta con pago en especie";
                }
                var numeroSio = status.ValidarEstado(basico.ContratoSAP).NumeroSio;
                string seccionSio = basico.CorredorId > 0 ?
                    String.Empty :
                    "<div class=\"espacio\"></div><div style=\"\" class=\"\"><span class=\"cls_006\">NÚMERO DE SIO GRANOS " + numeroSio + "</span></div><div class=\"espacio\"></div><div class=\"espacio\"></div>";

                xHtml = string.Format(xHtml,
                     stylesHtml, basico.ContratoSAP.TrimStart('0'), boleto.Version.ToString().PadLeft(2, '0'), basico.RazonSocialProveedor, basico.ContratoSAP.TrimStart('0'), basico.CorredorId > 0 ? basico.RazonSocialCorredor : "", basico.ContratoSAP.TrimStart('0'),
                     basico.RazonSocialProveedor, (basico.CorredorId > 0 ? basico.RazonSocialCorredor : ""), basico.Material, basico.Campania, basico.Cantidad.ToString("N", new CultureInfo("es-AR")),
                     precio, ($"{basico.Localidad}, {basico.Provincia}"), ($"{basico.DestinoLocalidad}, {basico.DestinoProvincia}"), "5", FormatoCuit(basico.Cuit), basico.CorredorId > 0 ? FormatoCuit(basico.CUITCorredor) : "",
                     titulo, clausulashtml, basico.FechaOperacion.GetValueOrDefault().ToString("dd'/'MM'/'yyyy"), "5", (basico.CorredorId > 0 ? "__________________" : ""), (basico.CorredorId > 0 ? "P. Corredor" : ""), (basico.CorredorId > 0 ? "Aclaración: _____________" : ""),
                     (basico.CorredorId > 0 ? "DNI Nro:&nbsp; _______________" : ""), (basico.CorredorId > 0 ? "CUIT Nro.: _____________" : ""), seccionSio);
            }
            else if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.BS_AS)
            {
                var corredor = basico.CorredorId > 0 ? $"<tr><td><b>Corredor: {basico.RazonSocialCorredor}</b><br /><b>CUIT: {FormatoCuit(basico.CUITCorredor)} </b> </td></tr>" : "";
                xHtml = string.Format(xHtml,
                stylesHtml, basico.ContratoSAP.TrimStart('0'), boleto.Version.ToString().PadLeft(2, '0'), basico.ContratoSAP.TrimStart('0'),
                basico.RazonSocialProveedor, FormatoCuit(basico.Cuit), corredor, clausulashtml, (basico.CorredorId > 0 ? "__________________" : ""), (basico.CorredorId > 0 ? "P. Corredor" : ""), (basico.CorredorId > 0 ? "Aclaración: _______________" : ""),
                     (basico.CorredorId > 0 ? "DNI Nro:&nbsp; _________________" : ""), (basico.CorredorId > 0 ? "Cargo:&nbsp;&nbsp; __________________" : ""), basico.FechaOperacion.GetValueOrDefault().ToString("dd'/'MM'/'yyyy"));
            }
            else if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                string clausulasNumeradas = "";
                for (int i = 1; i <= clausulas.Count; i++)
                {
                    clausulasNumeradas += "<li>" + clausulas.Where(x => x.Orden == i).First().Texto + "</li>";
                }
                xHtml = String.Format(xHtml, stylesHtml, basico.FechaOperacion?.ToString("dd.MM.yyyy"), basico.ContratoSAP.TrimStart('0'), basico.Proveedor, FormatoCuit(basico.Cuit), basico.ProveedorDireccion, basico.ProveedorProvincia, basico.ProveedorCP
                    , basico.Corredor, FormatoCuit(basico.CUITCorredor), clausulasNumeradas);
                return xHtml;
            }
            return xHtml;
        }

        public BoletoDto ValidarNegocioParaGenerarBoleto(BasicoContrato negocio)
        {
            var estadoBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(negocio.ContratoSAP, negocio.FijacionSAP);
            var boletoDto = new BoletoDto
            {
                NegocioId = negocio.Id,
                FechaGeneracion = DateTime.Now,
                ContratoSAP = negocio.ContratoSAP,
                FijacionSAP = negocio.FijacionSAP,
                TipoBoletoId = negocio.BoletoId.Value,
                Version = Convert.ToInt32(String.IsNullOrEmpty(estadoBoleto.Version) ? "0" : estadoBoleto.Version) + 1
            };

            if (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                boletoDto.Mensaje = $"No se pudo generar el boleto porque el negocio tiene tilde de Confirma.";
            else if (!string.IsNullOrEmpty(estadoBoleto.Generado) && string.IsNullOrEmpty(estadoBoleto.Anulado))
                boletoDto.Mensaje = $"El negocio ya tiene un boleto generado en SAP.";
            else if (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
            {
                if (negocio.BoletoId != (int)EnumBoletoCompraNet.FISICO && negocio.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
                {
                    boletoDto.Mensaje = $"No se pudo generar el boleto porque la fijación no tiene tilde de boleto físico o carta oferta.";
                }
                else if (negocio.Cantidad < 10000)
                {
                    boletoDto.Mensaje = $"No se pudo generar el boleto porque la fijación tiene cantidad menor a 10 toneladas.";
                }
                else if (negocio.Canje != true)
                {
                    boletoDto.Mensaje = $"No se pudo generar el boleto porque la fijación no es de Canje.";
                }
            }
            else
            {
                var res = status.ValidarEstado(negocio.ContratoSAP); //en fijaciones no se valida el estado
                if (string.IsNullOrEmpty(res.Status))
                {
                    boletoDto.Mensaje = $"No se pudo generar el boleto porque el contrato está en slip.";
                }
                else if (res.Status != "X")
                {
                    string motivoStatus = StatusNegocioEnGeneracionBoleto(res);
                    boletoDto.Mensaje = $"No se pudo generar el boleto por el estado del contrato: {res.Status} - {motivoStatus}";
                }
            }

            return boletoDto;
        }

        private string StatusNegocioEnGeneracionBoleto(EstadoSAPDto statusNegocio)
        {
            string msje = "";
            switch (statusNegocio.Status)
            {
                case "A":
                    msje = "Con Anulación Automática";
                    break;

                case "X":
                    msje = "Confirmado";
                    break;

                case "F":
                    msje = "Liquidación Finalizada";
                    break;

                case "C":
                    msje = "Cumplido";
                    break;

                case "M":
                    msje = "Con Anulación Parcial";
                    break;

                case "B":
                    msje = "Contrato Anulado Totalmente";
                    break;

                case "K":
                    msje = "Cumplido en Camiones(no se usa)";
                    break;

                case "T":
                    msje = "Contrato de Canje Cerrado(no se usa)";
                    break;

                case "J":
                    msje = "Prefijación Cerrada(no se usa)";
                    break;

                default:
                    break;
            }
            return msje;
        }

        public void ReenviarBoletos(List<string> listaContratos, List<string> archivos, string pathArchivos)
        {
            foreach (string numeroNegocio in listaContratos)
            {
                var itemNegocio = repositorio.Obtener<Negocio>(n => n.ContratoSAP == ("000" + numeroNegocio));

                if (itemNegocio != null)
                {
                    var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == (itemNegocio.CorredorId != 0 ? itemNegocio.CorredorId : itemNegocio.ProveedorId) && x.Boleto == true);

                    string _negocio = itemNegocio is ContratoAcuerdo ? itemNegocio.Id.ToString() : (itemNegocio is FijacionDePrecioContrato && (itemNegocio.EstadoId == (int)EnumEstadoContrato.Finalizado || itemNegocio.EstadoId == (int)EnumEstadoContrato.Eliminado)) ? (itemNegocio as FijacionDePrecioContrato).FijacionSAP : itemNegocio.ContratoSAP != "0" ? itemNegocio.ContratoSAP : "";

                    string contratoSapPdf = archivos.Find(sap => sap.Contains(numeroNegocio));
                    Byte[] fileBytes = BoletoEnByte(pathArchivos + "\\" + contratoSapPdf);

                    var consultaBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(itemNegocio.ContratoSAP, itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? _negocio : "");

                    var lista = new List<string>();
                    string contrato = itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? _negocio.Substring(_negocio.Length - 2) : itemNegocio.ContratoSAP.TrimStart('0');
                    string razonSocial = (itemNegocio.Corredor != null) ? itemNegocio.Corredor.RazonSocial : itemNegocio.Proveedor.RazonSocial;
                    string version = (Convert.ToInt32(String.IsNullOrEmpty(consultaBoleto.Version) ? "0" : consultaBoleto.Version) + 1).ToString();

                    var comercialRegistrado = mailManager.GetEmailUserActiveDirectory(itemNegocio.Comercial.IdActiveDirectory);

                    if (!PermisosHelper.Is(PermisosDataAgro.NoRecibirMail))
                    {
                        lista.Add(comercialRegistrado);
                        lista.Add("dataagro@molinosagro.com.ar");
                        logger.Debug("Reenviando mail Boleto a Comercial Registrado " + comercialRegistrado);
                    }
                    var subject = itemNegocio.Boleto.Descripcion == "Físico" ? "Boleto Físico" : itemNegocio.Boleto.Descripcion;
                    subject += " Molinos Agro S.A. – " + razonSocial + " - Contrato Nro. " + numeroNegocio;

                    mailManager.EnviarMail(itemNegocio.Comercial, emailproveedor, subject, "", lista, CuerpoMailBoleto(httpContextManager.ObtenerPathLogoMail(), contrato, version), fileBytes, contratoSapPdf);
                }
            }
        }

        private string FormatoCuit(string cuit)
        {
            if (cuit.Length == 11)
            {
                cuit = $"{cuit.Substring(0, 2)}-{cuit.Substring(2, 8)}-{cuit.Substring(10, 1)}";
            }

            return cuit;
        }

        public List<string> ObtenerClausulasPorNegocio(string contratoSap, List<int> equipo)
        {
            contratoSap = contratoSap.PadLeft(10, '0');
            List<string> clausulas = new List<string>();
            var contratos = new List<string> { contratoSap };
            var basicoContrato = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, equipo)).FirstOrDefault();
            clausulas = ObtenerClausulas(basicoContrato).Select(x => x.Texto).ToList();
            return clausulas;
        }

        public DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro)
        {
            var filter = CorregirFiltro(filtro);
            var result = repositorio.ObtenerConsultaEscalar(new TraerBoletosConFiltro(filter)) ?? throw new InvalidOperationException("El resultado de la consulta es nulo.");
            var data = result.Data as IEnumerable<BasicoBoleto>;

            // Iterar sobre los datos y modificar atributos
            foreach (var boleto in data)
            {
                boleto.Version_Proxima = boleto.Version_Proxima is null ? 1 : boleto.Version_Proxima + 1;
            }
            return result;
        }

        private DataSourceRequest CorregirFiltro(DataSourceRequest request)
        {
            if (request.Filter != null)
            {
                // Modificar el filtro principal
                request.Filter = ModificarFiltro(request.Filter);
            }

            return request;
        }

        private string CompletarEstadoBoleto(string ContratoSAP, string FijacionSAP)
        {
            var consultaBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(ContratoSAP, FijacionSAP);
            var mensaje = string.Empty;
            if (consultaBoleto.Generado == "X" && consultaBoleto.Anulado == "X")
            {
                mensaje = "Anulado";
            }
            else if (consultaBoleto.Generado == "X" && consultaBoleto.Anulado == "")
            {
                mensaje = "Generado";
            }
            else
            {
                mensaje = "No Generado";
            }
            return mensaje;
        }

        private Filter ModificarFiltro(Filter filtro)
        {
            int claseNegocio = 0;
            if (filtro == null)
            {
                return null;
            }

            // Lista para acumular los filtros modificados
            var modifiedFilters = new List<Filter>();

            // Manejo de filtros hijos
            if (filtro.Filters != null)
            {
                foreach (var childFilter in filtro.Filters)
                {
                    // Manejar filtros 'ClaseNegocio'
                    if (childFilter.Field == "ClaseNegocio")
                    {
                        childFilter.Field = "TipoNegocioId";

                        if (childFilter.Value.ToString() == "1")
                        {
                            claseNegocio = 1;
                            // Crear un nuevo filtro con lógica 'or' para TipoNegocio = 1 o TipoNegocio = 2
                            modifiedFilters.Add(new Filter
                            {
                                Logic = "or",
                                Filters = new List<Filter>
                        {
                            new Filter { Field = "TipoNegocioId", Operator = "eq", Value = 1 },
                            new Filter { Field = "TipoNegocioId", Operator = "eq", Value = 2 }
                        }
                            });
                        }
                        else if (childFilter.Value.ToString() == "2")
                        {
                            claseNegocio = 2;
                            childFilter.Value = 3;
                            childFilter.Operator = "eq";
                            modifiedFilters.Add(childFilter);
                        }
                    }
                    // Manejar filtros 'ContratoSAP' con operador 'gte' y si solo hay uno
                    else if ((childFilter.Field == "ContratoSAP" && childFilter.Operator == "gte" && filtro.Filters.Count(f => f.Field == "ContratoSAP") == 1) ||
                     (childFilter.Field == "FijacionSAP" && childFilter.Operator == "gte" && filtro.Filters.Count(f => f.Field == "FijacionSAP") == 1))
                    {
                        // Interpretar el valor como una lista de contratos y crear filtros eq
                        var contratos = childFilter.Value.ToString().Split(';');
                        var eqFilters = contratos.Select(c => new Filter
                        {
                            Field = ObtenerTextoNegocio(claseNegocio),
                            Operator = "eq",
                            Value = CompletarNegocioSAP(c)
                        }).ToList();

                        // Crear un nuevo filtro con lógica 'or' para ContratoSAP = [lista de contratos]
                        var contratoSapLogicFilter = new Filter
                        {
                            Logic = "or",
                            Filters = eqFilters
                        };

                        // Añadir el nuevo filtro y continuar con los demás filtros
                        modifiedFilters.Add(contratoSapLogicFilter);
                    }
                    else if (childFilter.Field == "ContratoSAP" || childFilter.Field == "FijacionSAP")
                    {
                        childFilter.Value = CompletarNegocioSAP(childFilter.Value.ToString());
                    }
                    // Convertir valores a DateTime solo si el filtro es de tipo FechaConfirmacion
                    else if (childFilter.Field == "FechaConfirmacion" && childFilter.Value is string strValue)
                    {
                        if (DateTime.TryParse(strValue, out DateTime dateValue))
                        {
                            childFilter.Value = dateValue;
                        }
                        modifiedFilters.Add(childFilter);
                    }
                    // Añadir otros filtros tal cual
                    else
                    {
                        modifiedFilters.Add(childFilter);
                    }
                }
            }

            // Asignar la lista de filtros modificados al filtro principal
            filtro.Filters = modifiedFilters;

            return filtro;
        }

        private string ObtenerTextoNegocio(int claseNegocio) => claseNegocio==1 ? "ContratoSAP" : "FijacionSAP";

        private string CompletarNegocioSAP(string negocioSAP) => int.Parse(negocioSAP).ToString("D10");
    }
}