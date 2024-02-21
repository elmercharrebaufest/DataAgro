using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.html;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.css;
using Molinos.DataAgro.Entities.Common.Enums;

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

        public BoletoManager(IRepositorio repositorio, ILogger logger, IContratosConfirmadosAgent oContratosConfirmadosAgent
            , IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent, IEnviarBoletoAgent oEnviarBoletoAgent
            , IMailManager mailManager, IHttpContextManager httpContextManager, IServicioClausulas servicioClausula, IStatusContratoAgent status)
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

        public BoletoResult GrabarBoleto(List<int> tipoNegocios, int comercialId, List<string> contratos, bool enviarEmail, List<int> equipo)
        {
            var error = new BoletoResult { boleto = new Boleto() };
            try
            {
                var request = new DataSourceRequest();
                var result = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, false, equipo, new List<int>()));

                var negocios = FiltrarNegocios(result, tipoNegocios);
                var comercial = repositorio.Obtener<Comercial>(comercialId);
                var contratosSinNegocio = contratos.Where(x => negocios.All(n => n.ContratoSAP != x && n.Negocio != x));
                foreach (var itemNegocio in negocios)
                {
                    if (tipoNegocios.Exists(x => x == itemNegocio.TipoNegocioId))
                    {
                        var mensaje = ValidarNegocioEnGeneracionBoleto(itemNegocio, itemNegocio.TipoNegocioId);
                        if (!string.IsNullOrEmpty(mensaje))
                        {
                            error.boletosGenerados.Add(DevolverDto(itemNegocio, false, 0, mensaje));
                            continue;
                        }
                        var consultaBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(itemNegocio.ContratoSAP, itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.Negocio : "");
                        if (consultaBoleto.Generado == "") // probar casos anulados
                        {
                            var tempBoleto = new BoletoGeneradoDto
                            {
                                NegocioId = itemNegocio.Id,
                                Version = Convert.ToInt32(String.IsNullOrEmpty(consultaBoleto.Version) ? "0" : consultaBoleto.Version) + 1,
                                ComercialId = comercialId,
                                FechaGeneracion = DateTime.Now,
                                //TipoNegocioDetalleId = itemNegocio.TipoNegocioId,
                                ContratoSAP = itemNegocio.ContratoSAP,
                                FijacionSAP = itemNegocio.FijacionSAP
                            };
                            logger.Debug("Enviando boleto" + tempBoleto.ToString());
                            var resultado = oEnviarBoletoAgent.Enviar(tempBoleto);
                            if (resultado == "Se actualizan correctamente los datos")
                            {
                                var guardaBoleto = repositorio.Agregar(ConvertirDtoAEntidad(tempBoleto));
                                error.boletos.Add(guardaBoleto);
                                var boletoGenerado = DevolverDto(itemNegocio, true, tempBoleto.Version, "");

                                var pdf = GenerarPDF(itemNegocio, ObtenerClausulas(itemNegocio), boletoGenerado);

                                try
                                {
                                    if (enviarEmail)
                                    {
                                        var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == (itemNegocio.CorredorId != 0 ? itemNegocio.CorredorId : itemNegocio.ProveedorId) && x.Boleto == true);
                                        EnviarMailBoleto(itemNegocio.BoletoDescripcion,
                                            (itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? "Contrato" : itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? "Fijación" : itemNegocio.TipoNegocio,
                                            String.IsNullOrEmpty(itemNegocio.RazonSocialCorredor) ? itemNegocio.RazonSocialProveedor : itemNegocio.RazonSocialCorredor,
                                            itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.Negocio.Substring(itemNegocio.Negocio.Length - 2) : itemNegocio.ContratoSAP.TrimStart('0'),
                                            tempBoleto.Version.ToString(), comercial, emailproveedor, pdf, (itemNegocio.ContratoSAP.TrimStart('0') + "_V" + tempBoleto.Version.ToString().PadLeft(2, '0')));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    error.ListaErrores.Add(new ErrorMessage { Message = " Error al enviar el email: " + ex.Message });
                                }

                                try
                                {
                                    File.WriteAllBytes(ConfigurationManager.AppSettings["PathBoletos"].ToString() + "\\"
                                         + (itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (itemNegocio.ContratoSAP + "_F" + itemNegocio.Negocio.Substring(itemNegocio.Negocio.Length - 3, 2)) : (itemNegocio.ContratoSAP.TrimStart('0') + "_V" + tempBoleto.Version.ToString().PadLeft(2, '0'))) + ".pdf", pdf);
                                }
                                catch (Exception ex)
                                {
                                    boletoGenerado.Mensaje += " Error al grabar el PDF del Boleto: " + ex.Message;
                                }
                                error.boletosGenerados.Add(boletoGenerado);
                            }
                            else
                            {
                                error.boletosGenerados.Add(DevolverDto(itemNegocio, false, 0, resultado));
                            }
                        }
                        else
                        {
                            error.boletosGenerados.Add(DevolverDto(itemNegocio, false, 0, "El boleto ya se encuentra generado en SAP"));
                        }
                    }
                    else
                    {
                        error.boletosGenerados.Add(DevolverDto(itemNegocio, false, 0, "El negocio no corresponde al tipo de negocio indicado"));
                    }
                }
                foreach (string itemContrato in contratos)
                {
                    var neg = negocios.Find(n => n.Negocio.Contains(itemContrato) || n.ContratoSAP.Contains(itemContrato));
                    if (neg == null)
                    {
                        error.boletosGenerados.Add(DevolverDto(new BasicoContrato { TipoNegocioId = (int)EnumTipoNegocio.A_PRECIO, ContratoSAP = itemContrato }, false, 0, "El negocio no esta habilitado para generar boleto"));
                    }
                }

                //repositorio.AgregarTodos(error.boletos);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Errores.Add(new ErrorMessage(400, e.Message));
            }
            return error;
        }

        private static Boleto ConvertirDtoAEntidad(BoletoGeneradoDto tempBoleto)
        {
            return new Boleto
            {
                NegocioId = tempBoleto.NegocioId,
                Version = tempBoleto.Version,
                ComercialId = tempBoleto.ComercialId,
                FechaGeneracion = tempBoleto.FechaGeneracion,
                //TipoNegocioDetalleId = tempBoleto.TipoNegocioDetalleId
            };
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
            catch (Exception ex)
            {
                throw ex;
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

        private static BoletoGeneradoDto DevolverDto(BasicoContrato itemNegocio, bool generado, int version, string mensaje)
        {
            return new BoletoGeneradoDto
            {
                ContratoSAP = Convert.ToInt64(itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.Negocio : itemNegocio.ContratoSAP).ToString(),
                Generado = generado,
                Version = version,
                Mensaje = mensaje
            };
        }

        private List<BasicoContrato> FiltrarNegocios(IQueryable<BasicoContrato> negocios, List<int> tipoNegocios)
        {
            List<TipoNegocioDetalle> tipoNegocioDetalles = repositorio.Listar<TipoNegocioDetalle>();
            List<BasicoContrato> negociosFiltrados = new List<BasicoContrato>();
            foreach (var negocio in negocios)
            {
                foreach (var tipo in tipoNegocioDetalles)
                {
                    if (tipo.Descripcion == negocio.TipoNegocio)
                    {
                        logger.Debug("Tipo Negocio: " + tipo.Descripcion + " " + negocio.TipoNegocio);
                        if ((negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.BoletoContratoId : negocio.BoletoContratoId) == (int)EnumBoletoCompraNet.CONFIRMA && tipo.Confirma)
                        {
                            negociosFiltrados.Add(negocio);
                        }
                        if ((negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.BoletoContratoId : negocio.BoletoContratoId) == (int)EnumBoletoCompraNet.FISICO && tipo.BoletoFisico)
                        {
                            negociosFiltrados.Add(negocio);
                        }
                        if ((negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.BoletoContratoId : negocio.BoletoContratoId) == (int)EnumBoletoCompraNet.CARTA_OFERTA && tipo.CartaOferta)
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

        public void EnviarMailBoleto(string boletoDescripcion, string tipoNegocio, string razonSocial, string contrato, string version, Comercial comercial, List<string> emailproveedor, byte[] pdf, string nombrePDF)
        {
            var lista = new List<string>();
            //var email = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            //if (comercial.IdActiveDirectory != comercial.ToUpper())
            //{
            //    lista.Add(email);
            //    logger.Debug("Enviando mail a Comercial boleto" + email);
            //}

            var comercialRegistrado = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);

            if (!PermisosHelper.Is(PermisosDataAgro.NoRecibirMail))
            {
                lista.Add(comercialRegistrado);
                lista.Add("dataagro@molinosagro.com.ar");
                logger.Debug("Enviando mail Boleto a Comercial Registrado " + comercialRegistrado);
            }
            var subject = boletoDescripcion + " Molinos Agro S.A. – " + tipoNegocio + " - " + razonSocial + " - Contrato Nro. " + contrato;

            mailManager.EnviarMail(comercial, emailproveedor, subject, "", lista, CuerpoMailBoleto(httpContextManager.ObtenerPathLogoMail(), contrato, version), pdf, nombrePDF + ".pdf");
        }

        private AlternateView CuerpoMailBoleto(String filePath, string contrato, string version)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            string htmlBody = "";

            htmlBody += "Se le envía por este medio el boleto de compraventa de granos número " +
                contrato +
                " de Molinos Agro S.A., versión " +
                version +
                ". Por favor imprimir con todas las copias incluidas (doble faz), firmar y subir a la web de" +
                " www.moaoperaciones.com.ar  y luego enviar a nuestras oficinas. <br />";
            htmlBody += "En caso de ser un boleto de Bolsa de Rosario, si no se envía impreso en doble faz se observará debido a que no están autorizando el obleado.<br/>" +
                "En caso de tener alguna consulta ingresar www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales,<br/><br/>" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br/><br/>Molinos Agro S.A. ";
            htmlBody += "<style> table, th, td{ }</style>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private string Split(string str)
        {
            var enumNumero = Enumerable.Range(0, str.Length / 2)
                .Select(i => str.Substring(i * 2, 2)).ToList();
            if (str.Length % 2 == 1)
            {
                enumNumero.Add(str[str.Length - 1].ToString());
            }
            var nuevoString = "";

            for (int i = 0; i < enumNumero.Count(); i++)
            {
                nuevoString += "<span>" + enumNumero[i] + "</span>";
            }
            return nuevoString;
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
                    if ((basico.CorredorId==0) && item.DisplayName.Equals("Clausula Veinte")) continue;//es clausula Veinte y no tiene proveedor SALTAR esta iteracion
                    clausula.Orden = orden++;
                    result.Add(clausula);
                }
            }
            return result.OrderBy(x => x.Orden).ToList();
        }

        public byte[] GenerarPDF(BasicoContrato basico, List<ResultadoClausula> clausulas, BoletoGeneradoDto boleto)
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
                        xHtml = CompletarHtml(basico, clausulas, boleto, xHtml, false);

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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string ObtenerPath(BasicoContrato basico)
        {
            //if(basico.TipoNegocioId == 3 && basico.BoletoContratoId == 2 && basico.BolsaContratoId == 2)
            //{
            //    return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/BoletoFisico.html");
            //}
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
                return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/CartaOferta.html");
            }

            return Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "Templates/BoletoFisico.html");
        }

        private string CompletarHtml(BasicoContrato basico, List<ResultadoClausula> clausulas, BoletoGeneradoDto boleto, string xHtml, bool esCartaOferta)
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
            font-size: 11.1px;
            color: rgb(0,0,0);
            font-weight: normal;
            font-style: normal;
            text-decoration: none;
        }

        .cls_008 {
            font-family: Arial,serif;
            font-size: 10.0px;
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
            font-size: 6px;
            text-align: justify;
        }
        
    </style>";

            if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.ROSARIO)
            {
                var precio = "";
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    precio = basico.Moneda + " " + basico.PrecioNeto.ToString();
                }
                else
                {
                    precio = "A Fijar";
                }
                var titulo = "";
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    titulo = "Bolsa de Comercio de Rosario Boleto de compra venta para cereales y oleaginosos";
                }
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && basico.Canje != true)
                {
                    titulo = "Bolsa de Comercio de Rosario Boleto de compra venta de granos a fijar precio";
                }
                if (basico.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && basico.Canje == true)
                {
                    titulo = "Bolsa de Comercio de Rosario Boleto de compra venta con pago en especie";
                }
                var numeroSio = status.ValidarEstado(basico.ContratoSAP).NumeroSio;

                xHtml = string.Format(xHtml,
                     stylesHtml, basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3), boleto.Version, basico.RazonSocialProveedor, basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3), (basico.CorredorId > 0 ? basico.RazonSocialCorredor : ""), basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3),
                     basico.RazonSocialProveedor, (basico.CorredorId > 0 ? basico.RazonSocialCorredor : ""), basico.Material, basico.Campania, basico.Cantidad,
                     precio, ($"{basico.Localidad}, {basico.Provincia}"), ($"{basico.DestinoLocalidad}, {basico.DestinoProvincia}"), "5", basico.Cuit, (basico.CorredorId > 0 ? basico.CUITCorredor : ""),
                     titulo, clausulashtml, basico.FechaOperacion.GetValueOrDefault().ToString("dd'/'MM'/'yyyy"),"5", (basico.CorredorId > 0 ? "__________________" : ""), (basico.CorredorId > 0 ? "P. el Corredor" : ""), (basico.CorredorId > 0 ? "Aclaración: _____________" : ""),
                     (basico.CorredorId > 0 ? "DNI Nro:&nbsp; _______________" : ""), (basico.CorredorId > 0 ? "CUIT Nro.: _____________" : ""),numeroSio);
            }
            else if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.FISICO && basico.BolsaContratoId == (int)EnumBolsaCompraNet.BS_AS)
            {
                var corredor = basico.CorredorId > 0 ? ($"<tr><td><b> Contrato N°: {basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3)}</b> <br />" +
                $"<b>Comprador: {basico.RazonSocialCorredor}</b><br /><b> CUIT: {basico.CUITCorredor} </b> </td></tr>") : "";
                xHtml = string.Format(xHtml,
                stylesHtml, basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3), boleto.Version, basico.ContratoSAP.Substring(3, basico.ContratoSAP.Length - 3),
                basico.RazonSocialProveedor, basico.Cuit, corredor, clausulashtml, (basico.CorredorId > 0 ? "__________________" : ""), (basico.CorredorId > 0 ? "P. el Corredor" : ""), (basico.CorredorId > 0 ? "Aclaración: _____________" : ""),
                     (basico.CorredorId > 0 ? "DNI Nro:&nbsp; _______________" : ""), (basico.CorredorId > 0 ? "CUIT Nro.: _____________" : ""));
            }
            else if (basico.BoletoContratoId == (int)EnumBoletoCompraNet.CARTA_OFERTA)
            {
                //var localidad = reposi
                string clausulasNumeradas = "";
                for (int i = 1; i <= clausulas.Count; i++)
                {
                    clausulasNumeradas += "<li>" + clausulas.Where(x => x.Orden == i).First().Texto + "</li>";
                }
                xHtml = String.Format(xHtml, stylesHtml, DateTime.Now.ToString("dd/MM/yyyy"), basico.ContratoSAP, basico.Proveedor, basico.Cuit, basico.ProveedorDireccion, basico.ProveedorProvincia, basico.ProveedorCP
                    , basico.Corredor, basico.CUITCorredor, clausulasNumeradas);
                return xHtml;
            }
            return xHtml;
        }

        private string ValidarNegocioEnGeneracionBoleto(BasicoContrato negocio, int tipoNegocio)
        {
            var mensaje = "";
            var kilosDisponibles = 10000;
            if (tipoNegocio == (int)EnumTipoNegocio.FIJACION)
            {
                var contrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP == negocio.ContratoSAP);
                if (contrato != null)
                {
                    if (contrato.Cantidad < (kilosDisponibles))
                    {
                        mensaje = "No se pudo generar el boleto para la fijación seleccionada";
                        logger.Debug("No se pudo generar el boleto para la fijacion seleccionada por cantidad no disponible " + negocio.FijacionSAP);
                    }
                    if (contrato.Canje == true)
                    {
                        mensaje = "No se pudo generar el boleto para la fijación seleccionada";
                        logger.Debug("No se pudo generar el boleto para la fijacion seleccionada por tener Canje " + negocio.FijacionSAP);
                    }
                }
                else
                {
                    mensaje = "No se encontró el contrato para la fijación seleccionada";
                }
            }
            else
            {
                var res = status.ValidarEstado(negocio.ContratoSAP);
                if (!string.IsNullOrEmpty(res.Status) && res.Status != "X")
                {
                    string motivoStatus = StatusNegocioEnGeneracionBoleto(res);
                    mensaje = $"No se pudo generar el boleto para el contrato seleccionado. Estado contrato: {motivoStatus}";
                    logger.Debug($"No se pudo generar el boleto por el status: {res.Status} ({motivoStatus}) - ContratoSAP: {negocio.ContratoSAP}");
                }
            }
            return mensaje;
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
    }

}