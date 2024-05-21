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
using System.Globalization;
using System.ServiceModel.Channels;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Agent.ScatoRepositorio;
using System.Xml.Linq;
using System.Xml;
using System.Web.Mvc;
using Molinos.DataAgro.Entities.Helpers;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Security.Policy;
using Org.BouncyCastle.Utilities;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfirmaManager : IConfirmaManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IStatusContratoAgent status;
        private readonly IEnviarBoletoAgent oEnviarBoletoAgent;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status,IEnviarBoletoAgent oEnviarBoletoAgent, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public ConfirmaResult GrabarConfirmas(int claseNegocio,int ComercialId, List<string> codigos, bool usarWebConfirma, List<int>equipo)
        {
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, false, equipo, new List<int>()));
            var contratos = FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
            var resultado = new ConfirmaResult();
            try
            {
                foreach (var contrato in contratos)
                {

                    //VALIDA NEGOCIO
                    var mensaje = ValidarNegocio(contrato.ContratoSAP, claseNegocio);
                    var esValido = mensaje == "" ? true : false;
                    if (!esValido)
                    {
                        resultado.confirmasGenerados.Add(DevolverDto(contrato, false, mensaje));
                        continue;
                    }

                    //CONSULTAR EXISTE CONFIRMA/BOLETO A RFC
                    var consultaConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(contrato.ContratoSAP, contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.Negocio : "");
                    if (consultaConfirma.Generado == "" || consultaConfirma.Anulado.Equals("X")) // probar casos anulados
                    { //Consulta: El Boleto/Confirma no existe o no esta generado en SAP???
                        //PRECARGAR CONFIRMA GENERADO DTO
                        var tempConfirma = new ConfirmaGeneradoDto() {
                            NegocioId = contrato.Id,
                            Version = "0",
                            ComercialId = ComercialId,
                            FechaGeneracion = DateTime.Now,
                            ContratoSAP = contrato.ContratoSAP,
                            FijacionSAP = contrato.FijacionSAP,
                            TipoBoletoId = contrato.BoletoId.GetValueOrDefault(),
                            IsWebService = usarWebConfirma,
                            Mensaje = string.Empty,
                            Generado = true
                         };
                        //Enviando Confirma a RFC como BoletoGeneradoDto
                        logger.Debug("Enviando confirma" + tempConfirma.ToString());
                        var res = oEnviarBoletoAgent.Enviar(ConfirmaABoletoDto(tempConfirma));
                        if (res == "Se actualizan correctamente los datos")
                        { //Generado exitosamente en RFC
                            //Se Almacena en DB el nuevo Confirma
                            var nuevoConfirma = repositorio.Agregar(ConvertirDtoAEntidad(tempConfirma));
                            resultado.confirmasGenerados.Add(tempConfirma);
                        }
                        else
                        {
                            resultado.confirmasGenerados.Add(DevolverDto(contrato, false, res));
                        }
                    }
                    else
                    {
                        logger.Info($"Confirma.Generado = {consultaConfirma.Generado} -- contrato SAP {contrato.ContratoSAP}");
                        resultado.confirmasGenerados.Add(DevolverDto(contrato, false, "El boleto ya se encuentra generado en SAP."));
                    }
                }
                //Se impactan los cambios en DB
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                resultado.Errores.Add(new ErrorMessage(400, e.Message));
            }
            return resultado;
        }

        private BoletoGeneradoDto ConvertirConfirmaBoleto(BasicoContrato negocio)
        {
            throw new NotImplementedException();
        }

        public List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int tipoNegocio)
        {
            var listaNegocios = new List<Negocio>();
            if (tipoNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true);
            }
            else
            {
                listaNegocios = repositorio.Listar<Negocio>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.Cantidad >= 10000);
            }

            List<string> codigos = listaNegocios.Where(x => int.Parse(x.ContratoSAP) >= negocioDesde && int.Parse(x.ContratoSAP) <= negocioHasta).Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            codigos.Sort();
            return codigos.FindAll(x => ValidarNegocio(x, tipoNegocio) == ""); ;
        }

        public List<string> ValidarNegocios(List<string> codigosSAP, int claseNegocio)
        {
            List<string> rechazados = new List<string>();
            foreach (var itemNegocio in codigosSAP)
            {
                var mensaje = ValidarNegocio(itemNegocio, claseNegocio);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    rechazados.Add(mensaje);
                }
            }
            return rechazados;
        }

        public List<string> FiltrarNegociosPorFecha(string desde, string hasta, int claseNegocio)
        {
            var fechaDesde = DateTime.ParseExact(desde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = hasta == "" ? DateTime.Now : DateTime.ParseExact(hasta, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);
            var listaNegocios = new List<Negocio>();
            if (claseNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta);
            }
            else
            {
                listaNegocios = repositorio.Listar<Negocio>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta && x.Cantidad >= 10000);
            }
            var result = listaNegocios.Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            return result.FindAll(x => ValidarNegocio(x, claseNegocio) == "");
        }

        public string ValidarNegocio(string codigoSAP, int claseNegocio)
        {
            var tiposNegocios = ConvertirClaseNegocioATiposNegocios(claseNegocio);
            var mensaje = "";
            var kilosDisponibles = 10000;
            var negocio = repositorio.Obtener<Negocio>(x => x.ContratoSAP == codigoSAP);
            
            if (negocio != null)
            {
                if (negocio.Cantidad < kilosDisponibles)
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP} por su cantidad menor a 10 toneladas.";
                    logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por cantidad menor a 10 toneladas.");
                }
                if (negocio.Canje != true)
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP} por no ser de canje.";
                    logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no ser de canje.");
                }
                //Validar que corresponda la clase de negocio
                if (tiposNegocios.Contains(negocio.TipoNegocioId))
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP}. Ha seleccionado el tipo incorrecto.";
                    logger.Debug($"No se puede generar el confirma el confirma {codigoSAP}. Ha seleccionado el tipo incorrecto.");
                }
                //Validar que tenga tilde CONFIRMA
                if (negocio.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP} por no tener tilde de confirma.";
                    logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no tener tilde de boleto físico o carta oferta.");
                }
                //Validar estado del contrato
                var res = status.ValidarEstado(codigoSAP);
                if (!string.IsNullOrEmpty(res.Status) && res.Status != "X")
                {
                    string motivoStatus = StatusNegocioConfirma(res);
                    mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por su estado: {motivoStatus}";
                    logger.Debug($"No se puede generar el confirma por el status: {res.Status} ({motivoStatus}) - ContratoSAP: {codigoSAP}");
                }
                else if (string.IsNullOrEmpty(res.Status))
                {
                    mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por estar en slip.";
                    logger.Debug($"No se puede generar el confirma por tener status vacío (slip) - ContratoSAP: {codigoSAP}");
                }
            }
            else
            {
                mensaje = $"No se encontró el negocio {codigoSAP} seleccionado.";
            }
            
            return mensaje;
        }

        private string StatusNegocioConfirma(EstadoSAPDto statusNegocio)
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

        public byte[] ConfirmaEnByte(string codigoSAP)
        {
            //Constantes
            const string B = "0001";
            var confirma = repositorio.Obtener<Confirma>(x => x.Negocio.ContratoSAP == codigoSAP);
            //Parte temporal, no queda en la version final
            confirma.FechaGeneracion = DateTime.Now;

            var estadoSAP = status.ValidarEstado(confirma.Negocio.ContratoSAP);
            var esFijacionContratoConvenio = confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && confirma.Negocio.Madre == true;
            //cargarle por seters los datos que necesites mostrar

            //Fin de carga de Setters
            XmlDocument doc = new XmlDocument(); //Documento XML
            MemoryStream ms = new MemoryStream(); //Memory Stream
            var xmlString = new StringBuilder(); //String Builder
            var xmlWriter = XmlWriter.Create(xmlString, new XmlWriterSettings { Indent = true }); //Incializa Writer

            //Inicia formateo del XML
            xmlWriter.WriteStartElement("LoteDocumentos"); //Abre LoteDocumentos
                xmlWriter.WriteStartElement("Lote"); // NODO lote
                    xmlWriter.WriteElementString("EmpresaPresentante", "30715118773");

                    /// ITEMS - codigo Identificador del ítem para el cliente
                    #region ITEMS
                    xmlWriter.WriteStartElement("Items");

                        ///ItemInfo
                        xmlWriter.WriteStartElement("ItemInfo");

                            xmlWriter.WriteElementString("Workflow", confirma.Negocio.CorredorId>0?"4":"7");

                        xmlWriter.WriteEndElement(); //Fin ItemInfo

                        /// ContratoFijaPrecio
                        #region ContratoFijaPrecio
                        xmlWriter.WriteStartElement("ContratoFijarPrecio");

                            // cabeceraDocumento
                            xmlWriter.WriteStartElement("CabeceraDocumento");

                                //Bolsa - CodLista "Identificador de la Bolsa (valores tabulados) 
                                xmlWriter.WriteElementString("Bolsa", confirma.Negocio.BolsaId.ToString());
                                //TipoDocumento - CodLista = 3 
                                xmlWriter.WriteElementString("TipoDocumento", confirma.Negocio.Canje==true?"17":(confirma.Negocio.TipoNegocioId==(int)EnumTipoNegocio.A_PRECIO?"1":(confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "3" : "")));

                            xmlWriter.WriteEndElement();
                            //Fin cabeceraDocumento

                            /// DetalleDocumento
                            xmlWriter.WriteStartElement("DetalleDocumento");


                                /// Partes - Hay un nodo <Parte> por cada empresa que forma parte del contrato -
                                xmlWriter.WriteStartElement("Partes");
                                    // parte Vendedor
                                    xmlWriter.WriteStartElement("Parte");
                                        xmlWriter.WriteElementString("CodLista", "1");
                                        xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoSAP);
                                        xmlWriter.WriteElementString("CUIT", confirma.Negocio.Proveedor.CUIT);
                                    xmlWriter.WriteEndElement();
                                    //fin parte Vendedor
                                    // parte Comprador
                                    xmlWriter.WriteStartElement("Parte");
                                        xmlWriter.WriteElementString("CodLista", "3");
                                        xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoSAP);
                                        xmlWriter.WriteElementString("CUIT", "30715118773");
                                    xmlWriter.WriteEndElement();
                                    //fin parte Comprador
                                    if(confirma.Negocio.CorredorId>0){
                                    // parte Corredor si corresponde
                                    xmlWriter.WriteStartElement("Parte");
                                        xmlWriter.WriteElementString("CodLista", "2");
                                        xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoSAP);
                                        xmlWriter.WriteElementString("CUIT", confirma.Negocio.Corredor.CUIT);
                                    xmlWriter.WriteEndElement();
                                    //fin parte Corredor si corresponde
                                    }
                                xmlWriter.WriteEndElement();
                                //Fin Partes
                                // DetalleContrato
                                xmlWriter.WriteStartElement("DetalleContrato");
                                    //  Comisión del corredor a cargo del comprador (en contratos con intervención del corredor)
                                    xmlWriter.WriteElementString("ComisionPorComprador", confirma.Negocio.PorcentajeComision.ToString());
                                    // CodLista - dentificador del Producto (valores tabulados) 
                                    xmlWriter.WriteElementString("Producto", confirma.Negocio.MaterialId==(int)EnumMateriales.TRIGO? "1" : (confirma.Negocio.MaterialId == (int)EnumMateriales.MAIZ ? "2" : (confirma.Negocio.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : (confirma.Negocio.MaterialId == (int)EnumMateriales.GIRASOL ? "21" : ""))));
                                    xmlWriter.WriteElementString("DescAdicional", confirma.Negocio.Canje==true?"INSUMO":"");//PREGUNTAR
                                    xmlWriter.WriteElementString("FechaConcertacion", confirma.Negocio.FechaOperacion.ToString());
                                    //CodLista  Identificador de la Cosecha del producto (valores tabulados)
                                    xmlWriter.WriteElementString("Cosecha", confirma.Negocio.Campana.Descripcion);
                                    //CodLista  Identificador de la Unidad de medida en que se expresan las cantidades del contrato
                                    xmlWriter.WriteElementString("UnidadMedida", "K");
                                    xmlWriter.WriteElementString("CantidadDesde", string.Empty);
                                    xmlWriter.WriteElementString("CantidadHasta", string.Empty);
                                    // CodLista="Identificador del método de ajuste de la cantidad si en el contrato se expresa cantidad desde=hasta
                                    xmlWriter.WriteElementString("Ajuste", string.Empty);
                                    xmlWriter.WriteElementString("CantCamiones", string.Empty);
                                    //  CodLista="Identificador de la moneda del contrato (valores tabulados)
                                    xmlWriter.WriteElementString("Moneda", confirma.Negocio.Moneda.Descripcion);
                                    xmlWriter.WriteElementString("MontoImponible", confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ?confirma.Negocio.Monto.ToString():"" );

                                    //Calidad
                                    xmlWriter.WriteStartElement("DetalleContrato");
                                        // CodLista="Identificador de la calidad del producto (valores tabulados) "
                                        xmlWriter.WriteElementString("CondicionesCalidad", (confirma.Negocio.StandardDeCalidad?.Id==(int)EnumStandarCalidad.CAMARA || confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.ESPECIAL) ?"1":(confirma.Negocio.StandardDeCalidad?.Id==(int)EnumStandarCalidad.FABRICA?"4":""));
                                        xmlWriter.WriteElementString("OtrasCondicionesCalidad", string.Empty);
                                    xmlWriter.WriteEndElement();
                                    //Fin Calidad
                                    // CodLista="Identificador del medio de transporte en que se translada el producto (valores tabulados) "
                                    xmlWriter.WriteElementString("MedioTransporte", "C");

                                    // Entregas
                                    xmlWriter.WriteStartElement("Entregas");
                                        xmlWriter.WriteElementString("EntregaDesde", confirma.Negocio.FechaEntrega.ToString());
                                        xmlWriter.WriteElementString("EntregaHasta", string.Empty);
                                    xmlWriter.WriteEndElement();
                                    //Fin Entregas
            
                                    // Origen
                                    xmlWriter.WriteStartElement("Origen");
                                        xmlWriter.WriteElementString("LocalidadOrigen", confirma.Negocio.Localidad.CodLocalidad);
                                        xmlWriter.WriteElementString("ProvinciaOrigen", confirma.Negocio.Provincia.Orden.ToString());
                                    xmlWriter.WriteEndElement();
                                    //Fin Origen

                                    // CodLista="Identificador del puerto destino del producto (valores tabulados) "
                                    xmlWriter.WriteElementString("Destino", confirma.Negocio.Destino.CodigoConfirma.ToString());

                                    // Pagos
                                    xmlWriter.WriteStartElement("Origen");
                                        xmlWriter.WriteElementString("FechaCondicionPago", confirma.Negocio.Canje == true ?"": (esFijacionContratoConvenio ? "4 días hábiles de fecha de fijación" : (!(confirma.Negocio.PagoDiferido == true) ?"Días de diferimiento contra mercadería entregada": (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (confirma.Negocio.Warrant==true ? "Pago contra warrant" : (confirma.Negocio.CD == true ? "Pago contra CD" : "72 hrs contra mercadería entregada")) : ""))));
                                        xmlWriter.WriteElementString("LugarPago", "Buenos Aires");
                                        xmlWriter.WriteElementString("PagoAOrdenDe", confirma.Negocio.CorredorId>0?(confirma.Negocio.PagoDirectoVendedor == true ?"1":"2") :"");
                                        xmlWriter.WriteElementString("PorcPago", confirma.Negocio.PorcentajeDePago.ToString());
                                    xmlWriter.WriteEndElement();
                                    //Fin Pagos

                                    // Fijacion
                                    xmlWriter.WriteStartElement("Origen");
                                        xmlWriter.WriteElementString("FijMinima", string.Empty);//Preguntar
                                        xmlWriter.WriteElementString("UnidadMedidaFijacion", "K");
                                        xmlWriter.WriteElementString("FijPeriodo", "1");
                                        xmlWriter.WriteElementString("FijFecDesde", confirma.Negocio.DesdeFijacion.ToString());
                                        xmlWriter.WriteElementString("FijFecHasta", confirma.Negocio.HastaFijacion.ToString());
                                        xmlWriter.WriteElementString("PorcMultaIncumplimiento", "010");
                                        xmlWriter.WriteElementString("ComunicacionFijacion", confirma.Negocio.PagoDirectoVendedor == true ?"2":"1");
                                    xmlWriter.WriteEndElement();
                                    //Fin Fijacion

                                    xmlWriter.WriteElementString("ProduccionVendedor", confirma.Negocio.ClasificacionId==(int)EnumClasificacionCompraNet.Productor?(confirma.Negocio.PagoDirectoVendedor == true ?"1":"4"):(confirma.Negocio.Consignatario==true?"5": "2"));
                                    xmlWriter.WriteElementString("DecisionPagoVoluntario", string.Empty);
                                    xmlWriter.WriteElementString("APrecio", confirma.Negocio.TipoNegocioId==(int)EnumTipoNegocio.A_PRECIO?"1":"");
                                    xmlWriter.WriteElementString("TipoOperacion", "1");
                                    xmlWriter.WriteElementString("DecisionDeclaraPrecioUnit", confirma.Negocio.Canje == true ?"0":"");
                                    xmlWriter.WriteElementString("DecisionDeclaraCantidad", confirma.Negocio.Canje == true ?"0":"");
                                   // xmlWriter.WriteElementString("OperacionExentaImpSantaFe", string.Empty);

                                    // SioGranos
                                    xmlWriter.WriteStartElement("Origen");
                                        xmlWriter.WriteElementString("NumeroDeclaracion", estadoSAP.NumeroSio.ToString());
                                    xmlWriter.WriteEndElement();
                                    //Fin SioGranos


                                xmlWriter.WriteEndElement();
                                //Fin DetalleContrato

                                // Clausulas
                                xmlWriter.WriteStartElement("Clausulas");
                                    // Orden="orden de la cláusula dentro de la lista " TipoClausula="3 (valor fijo) ">
                                    xmlWriter.WriteElementString("Clausula", string.Empty);
                                xmlWriter.WriteEndElement();
                                //Fin Clausulas

                            xmlWriter.WriteEndElement(); 
                            //Fin DetalleDocuemento

                        xmlWriter.WriteEndElement(); 
                        //Fin ContratoFijaPrecio
                        #endregion

                    xmlWriter.WriteEndElement(); 
                    //Fin Items
                    #endregion
                xmlWriter.WriteEndElement(); //Fin NODO lote
            xmlWriter.WriteEndElement(); //Fin LoteDocumentos
            //Fin Formateo del XML
            xmlWriter.Flush(); //Limpia memoria
            doc.LoadXml(xmlString.ToString()); //Carga en el documento lo escrito en el String
            doc.Save(ms); //Guarda el Documento en el Stream
            byte[] bytes = ms.ToArray(); //Devuelve el documento
            return bytes;
        }

        private static Confirma ConvertirDtoAEntidad(ConfirmaGeneradoDto tempConfirma)
        {
            return new Confirma
            {
                NegocioId = tempConfirma.NegocioId,
                ComercialId = tempConfirma.ComercialId,
                FechaGeneracion = tempConfirma.FechaGeneracion,
                IsWebService = tempConfirma.IsWebService,
            };
        }

        private List<int> ConvertirClaseNegocioATiposNegocios(int claseNegocio)
        {
            var tiposNegocios = new List<int>();
            if (claseNegocio == 1) // Contrato
            {
                tiposNegocios.Add((int)EnumTipoNegocio.A_FIJAR);
                tiposNegocios.Add((int)EnumTipoNegocio.A_PRECIO);
            }
            else // Fijación
            {
                tiposNegocios.Add((int)EnumTipoNegocio.FIJACION);
            }
            return tiposNegocios;
        }

        public string GenerarNombreArchivoConfirma(string codigoSAP)
        {
            var confirma = repositorio.Obtener<Confirma>(x => x.Negocio.ContratoSAP == codigoSAP);
            var nombreArchivo = "confirma" + confirma.FechaGeneracion.Year.ToString() + confirma.FechaGeneracion.Month.ToString() + confirma.FechaGeneracion.Day.ToString() + "_000" + codigoSAP + ".xml";
            return nombreArchivo;
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

        private static ConfirmaGeneradoDto DevolverDto(BasicoContrato itemNegocio, bool generado, string mensaje)
        {
            return new ConfirmaGeneradoDto
            {
                ContratoSAP = Convert.ToInt64(itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.Negocio : itemNegocio.ContratoSAP).ToString(),
                Generado = generado,
                Mensaje = mensaje,
                FechaGeneracion = default(DateTime),
                IsWebService = false,
            };
        }

        private static BoletoGeneradoDto ConfirmaABoletoDto(ConfirmaGeneradoDto tempConfirma)
        {
            return new BoletoGeneradoDto
            {
                NegocioId = tempConfirma.NegocioId,
                Version = 0,
                ComercialId = tempConfirma.ComercialId,
                FechaGeneracion = tempConfirma.FechaGeneracion,
                ContratoSAP = tempConfirma.ContratoSAP,
                FijacionSAP = tempConfirma.FijacionSAP,
                TipoBoletoId = tempConfirma.TipoBoletoId
            };
        }
        public List<ConfirmaArchivoDto> ListarConfirmas()
        {
            return repositorio.Listar<Confirma, ConfirmaArchivoDto>
                (a => new ConfirmaArchivoDto { 
                    Id = a.Id, 
                    NegocioId = a.NegocioId,
                    ComercialId = a.ComercialId,
                    Nombre = ("confirma" + a.FechaGeneracion.Year.ToString() + a.FechaGeneracion.Month.ToString() + a.FechaGeneracion.Year.ToString() + a.FechaGeneracion.Day.ToString() + "_000" +  a.Negocio.ContratoSAP + ".xml"), 
                    FechaGeneracion = a.FechaGeneracion.Day +"/"+ a.FechaGeneracion.Month + "/" + a.FechaGeneracion.Year,
                    IsWebService = a.IsWebService,
                    ContratoSAP = a.Negocio.ContratoSAP,
                }, null, 0, null, Entities.Helpers.DirOrden.Asc)
                .Where(c => !c.IsWebService).ToList();
        }
    }
}