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

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.status = status;
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public ConfirmaResult GrabarConfirmas(int claseNegocio,int ComercialId, List<string> contratos, bool usarWebConfirma)
        {
            var result = new ConfirmaResult();
            try
            {
                foreach(var contrato in contratos)
                {
                    var negocio = repositorio.ObtenerPrimero<Negocio>(x=>x.ContratoSAP==contrato);
                    //var esValido = ValidarNegocio(contrato, negocio.TipoNegocioId)==""?true:false;  //IMPLEMENTAR VALIDACION
                    var esValido = true;
                    if (esValido && negocio!=null)
                    {
                        var tempConfirma = new ConfirmaGeneradoDto();
                        tempConfirma.FechaGeneracion=DateTime.Now;
                        tempConfirma.NegocioId=negocio.Id;
                        tempConfirma.ComercialId=ComercialId;
                        tempConfirma.IsWebService=usarWebConfirma;
                        tempConfirma.ContratoSAP=contrato;
                        tempConfirma.Generado = true;
                        var nuevoConfirma = repositorio.Agregar(ConvertirDtoAEntidad(tempConfirma));
                        //Agregar a Servicio Confirma
                        //result.confirmas.Add(nuevoConfirma);
                        result.confirmasGenerados.Add(tempConfirma);
                    }
                    
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                result.Errores.Add(new ErrorMessage(400, e.Message));
            }
            return result;
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

        public List<string> ValidarNegocios(List<string> codigosSAP, int tipoNegocio)
        {
            List<string> rechazados = new List<string>();
            foreach (var itemNegocio in codigosSAP)
            {
                var mensaje = ValidarNegocio(itemNegocio, tipoNegocio);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    rechazados.Add(mensaje);
                }
            }
            return rechazados;
        }

        public List<string> FiltrarNegociosPorFecha(string desde, string hasta, int tipoNegocio)
        {
            var fechaDesde = DateTime.ParseExact(desde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = hasta == "" ? DateTime.Now : DateTime.ParseExact(hasta, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);
            var listaNegocios = new List<Negocio>();
            if (tipoNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta);
            }
            else
            {
                listaNegocios = repositorio.Listar<Negocio>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta && x.Cantidad >= 10000);
            }
            var result = listaNegocios.Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            return result.FindAll(x => ValidarNegocio(x, tipoNegocio) == "");
        }

        public string ValidarNegocio(string codigoSAP, int tipoNegocio)
        {
            var mensaje = "";
            var kilosDisponibles = 10000;
            var contrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP == codigoSAP);
            if (tipoNegocio == (int)EnumTipoNegocio.FIJACION)
            {
                if (contrato != null)
                {
                    if (contrato.Cantidad < kilosDisponibles)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por su cantidad menor a 10 toneladas.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por cantidad menor a 10 toneladas.");
                    }
                    if (contrato.Canje != true)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por no ser de canje.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no ser de canje.");
                    }
                    if (contrato.BoletoId != (int)EnumBoletoCompraNet.FISICO && contrato.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
                    {
                        mensaje = $"No se puede generar el confirma para la fijación {codigoSAP} por no tener tilde de boleto físico o carta oferta.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no tener tilde de boleto físico o carta oferta.");
                    }
                }
                else
                {
                    mensaje = $"No se encontró el contrato para la fijación {codigoSAP} seleccionada.";
                }
            }
            else
            {
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
            //Agregar Validacion ANTI BOLETO Y CARTA OFERTA
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
                                        xmlWriter.WriteElementString("CondicionesCalidad", (confirma.Negocio.StandardDeCalidad.Id==(int)EnumStandarCalidad.CAMARA || confirma.Negocio.StandardDeCalidad.Id == (int)EnumStandarCalidad.ESPECIAL) ?"1":(confirma.Negocio.StandardDeCalidad.Id==(int)EnumStandarCalidad.FABRICA?"4":""));
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
    }
}