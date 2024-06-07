using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Molinos.DataAgro.Entities.Common.Enums;
using System.Globalization;
using System.Xml;
using Molinos.DataAgro.Interfaces.Clausulas;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfirmaManager : IConfirmaManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IStatusContratoAgent status;
        private readonly IMailManager mailManager;
        private readonly IHttpContextManager httpContextManager;
        private readonly IEnviarBoletoAgent oEnviarBoletoAgent;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;
        private readonly IServicioClausulas servicioClausula;

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status,IEnviarBoletoAgent oEnviarBoletoAgent, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent, IMailManager mailManager, IHttpContextManager httpContextManager, IServicioClausulas servicioClausula)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
            this.servicioClausula = servicioClausula;
            this.oEnviarBoletoAgent = oEnviarBoletoAgent;
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public ConfirmaResult GrabarConfirmas(int claseNegocio,int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo)
        {
            var codigos = AgregarCeros(codigosSap);
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, false, equipo, new List<int>()));
            var contratos = FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
            var resultado = new ConfirmaResult();
            try
            {
                foreach (var contrato in contratos)
                {
                    
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
                            IsWebService = usarWebServiceConfirma,
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
            var codigoSAPcompleto = codigoSAP.PadLeft(10, '0');
            var tiposNegocios = ConvertirClaseNegocioATiposNegocios(claseNegocio);
            var mensaje = "";
            var kilosMinimos = 10000;
            var negocio = repositorio.Obtener<Negocio>(x => x.ContratoSAP == codigoSAPcompleto);

            if (negocio != null)
            {
                //Validar que corresponda la clase de negocio
                if (!tiposNegocios.Contains(negocio.TipoNegocioId))
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP}. Ha seleccionado el tipo incorrecto.";
                    logger.Debug($"No se puede generar el confirma el confirma {codigoSAP}. Ha seleccionado el tipo incorrecto.");
                    return mensaje;
                }

                if (negocio.TipoNegocioId==(int)EnumTipoNegocio.FIJACION) //FIJACION
                {
                    if (negocio.Cantidad < kilosMinimos)
                    {
                        mensaje = $"No se puede generar el confirma {codigoSAP} por su cantidad menor a 10 toneladas.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por cantidad menor a 10 toneladas.");
                        return mensaje;
                    }
                    if (negocio.Canje != true)
                    {
                        mensaje = $"No se puede generar el confirma {codigoSAP} por no ser de canje.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no ser de canje.");
                        return mensaje;
                    }

                }else{ //CONTRATO
                    //Validar estado del contrato
                    var res = status.ValidarEstado(codigoSAP);
                    if (!string.IsNullOrEmpty(res.Status) && res.Status != "X")
                    {
                        string motivoStatus = StatusNegocioConfirma(res);
                        mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por su estado: {motivoStatus}";
                        logger.Debug($"No se puede generar el confirma por el status: {res.Status} ({motivoStatus}) - ContratoSAP: {codigoSAP}");
                        return mensaje;
                    }
                    else if (string.IsNullOrEmpty(res.Status))
                    {
                        mensaje = $"No se puede generar el confirma con negocio {codigoSAP} para el contrato por estar en slip.";
                        logger.Debug($"No se puede generar el confirma por tener status vacío (slip) - ContratoSAP: {codigoSAP}");
                        return mensaje;
                    }
                }

                //Validar que tenga tilde CONFIRMA
                if (negocio.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
                {
                    mensaje = $"No se puede generar el confirma {codigoSAP} por no tener tilde de confirma.";
                    logger.Debug($"No se puede generar el confirma para la fijacion {codigoSAP} por no tener tilde de confirma.");
                    return mensaje;
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
            var confirma = repositorio.Obtener<Confirma>(x => x.Negocio.ContratoSAP == codigoSAP);
            var tempCodigo = confirma.Negocio.ContratoSAP;
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { tempCodigo }, false, new List<int>(), new List<int>()));
            var contrato = consulta.First();
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
            //Calcular datos para el XML
            var clausulas = ObtenerClausulas(contrato);
            var datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(codigoSAP, string.Empty);
            var condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
            //Inicia formateo del XML
            xmlWriter.WriteStartElement("Lote"); // NODO Lote
            xmlWriter.WriteStartElement("Documento"); //Abre Nodo Documento
            xmlWriter.WriteAttributeString("xmlns", "Documento");
            #region CabeceraDocumento
            xmlWriter.WriteStartElement("CabeceraDocumento");
                xmlWriter.WriteElementString("Bolsa",string.Empty); //Bolsa
                    xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.BolsaId.ToString()); //CodLista "Identificador de la Bolsa (valores tabulados) 
                xmlWriter.WriteElementString("TipoDocumento", string.Empty); //TipoDocumento
                    xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.Canje == true ? "17" : (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "3" : ""))); //TipoDocumento
                xmlWriter.WriteElementString("Formulario", string.Empty); //Formulario version
                    xmlWriter.WriteAttributeString("formversion", "1.04"); 
            xmlWriter.WriteEndElement();
            #endregion CabeceraDocumento
            #region Workflow
            xmlWriter.WriteStartElement("UploadInfo");
                xmlWriter.WriteElementString("Workflow", confirma.Negocio.CorredorId > 0 ? "4" : "7"); //Workflow depende de las partes
            xmlWriter.WriteEndElement();
            #endregion Workflow
            xmlWriter.WriteStartElement("DetalleDocumento");
            #region Partes
            xmlWriter.WriteStartElement("Partes"); //Inicio Partes
                // parte Vendedor
                xmlWriter.WriteStartElement("Parte");
                xmlWriter.WriteAttributeString("CodLista", "1"); //CodLista = 1 para Vendedor
                    xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoVendedor);
                    xmlWriter.WriteElementString("CUIT", confirma.Negocio.Proveedor.CUIT);
                    xmlWriter.WriteElementString("Sucursal", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista", string.Empty);
                xmlWriter.WriteEndElement();
            //fin parte Vendedor
            // parte Corredor si corresponde
            if (confirma.Negocio.CorredorId > 0)
            {
                xmlWriter.WriteStartElement("Parte");
                xmlWriter.WriteAttributeString("CodLista", "2");
                xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoCorredor);
                xmlWriter.WriteElementString("CUIT", confirma.Negocio.Corredor.CUIT);
                xmlWriter.WriteElementString("Sucursal", string.Empty);
                xmlWriter.WriteAttributeString("CodLista", string.Empty);
                xmlWriter.WriteEndElement();
            }
            //fin parte Corredor si corresponde
            // parte Comprador
            xmlWriter.WriteStartElement("Parte");
            xmlWriter.WriteAttributeString("CodLista", "3");
            xmlWriter.WriteElementString("NroContratoInterno", confirma.Negocio.ContratoSAP);
            xmlWriter.WriteElementString("CUIT", "30715118773");
            xmlWriter.WriteElementString("Sucursal", string.Empty);
            xmlWriter.WriteAttributeString("CodLista", string.Empty);
            xmlWriter.WriteEndElement();
            //fin parte Comprador
            xmlWriter.WriteEndElement(); //Fin Partes
            #endregion Partes
            #region DetalleContrato
            // Inicio DetalleContrato
            xmlWriter.WriteStartElement("DetalleContrato");

            xmlWriter.WriteElementString("Producto",string.Empty);
            xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.MaterialId == (int)EnumMateriales.TRIGO ? "1" : confirma.Negocio.MaterialId == (int)EnumMateriales.MAIZ ? "2" : confirma.Negocio.MaterialId == (int)EnumMateriales.SORGO ? "3" : confirma.Negocio.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : confirma.Negocio.MaterialId == (int)EnumMateriales.GIRASOL ? "21" : "");
            xmlWriter.WriteElementString("DescAdicional", confirma.Negocio.Canje == true ? "INSUMO" : "");
            xmlWriter.WriteElementString("FechaConcertacion", confirma.Negocio.FechaOperacion.ToString("dd/MM/yyyy"));
            xmlWriter.WriteElementString("Cosecha",string.Empty);
            xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.Campana.Descripcion);
            xmlWriter.WriteElementString("UnidadMedida",string.Empty);
            xmlWriter.WriteAttributeString("CodLista","K");
            xmlWriter.WriteElementString("CantidadDesde", confirma.Negocio.KgMinimo.ToString());
            xmlWriter.WriteElementString("CantidadHasta", confirma.Negocio.KgMaximo.ToString());
            xmlWriter.WriteElementString("Ajuste", string.Empty);//Consultar siempre vacia?
            xmlWriter.WriteAttributeString("CodLista", string.Empty);
            xmlWriter.WriteElementString("CantCamiones", confirma.Negocio.CantidadCamiones.GetValueOrDefault().ToString());
            xmlWriter.WriteElementString("Moneda", string.Empty);
            xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.Moneda.Descripcion=="ARP"?"1":"2");
            xmlWriter.WriteElementString("Precio", confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? confirma.Negocio.Precio.ToString() : "");
            xmlWriter.WriteElementString("UnidadMedidaPrecio", string.Empty);
            xmlWriter.WriteAttributeString("CodLista", "T");
            xmlWriter.WriteElementString("PorcComisionComprador", confirma.Negocio.PorcentajeComision.ToString());
            #region Calidad
            xmlWriter.WriteStartElement("Calidad");
                xmlWriter.WriteElementString("CondicionesCalidad",string.Empty);
                xmlWriter.WriteAttributeString("CodLista", (confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.CAMARA || confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.FABRICA ? "4" : ""));
                xmlWriter.WriteElementString("OtrasCondicionesCalidad", string.Empty);
            xmlWriter.WriteEndElement();
            #endregion Calidad
            xmlWriter.WriteElementString("MedioTransporte", string.Empty);
            xmlWriter.WriteAttributeString("CodLista", "C");
            #region Entregas
            xmlWriter.WriteStartElement("Entregas");
                xmlWriter.WriteElementString("EntregaDesde", confirma.Negocio.FechaDesde.ToString("dd/MM/yyyy"));
                xmlWriter.WriteElementString("EntregaHasta", confirma.Negocio.FechaHasta.ToString("dd/MM/yyyy"));
            xmlWriter.WriteEndElement();
            #endregion Entregas
            #region Origen
            xmlWriter.WriteStartElement("Origen");
                xmlWriter.WriteElementString("LocalidadOrigen", confirma.Negocio.Localidad.CodLocalidad);
                xmlWriter.WriteElementString("ProvinciaOrigen", string.Empty);
                xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.Provincia.CodigoConfirma);
            xmlWriter.WriteEndElement();
            #endregion Origen
            xmlWriter.WriteElementString("Destino",string.Empty);
            xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.Destino.CodigoConfirma.ToString());
            xmlWriter.WriteAttributeString("CodPrv", "0000");
            xmlWriter.WriteElementString("ProvinciaInstrumentacion", "0001");
            xmlWriter.WriteAttributeString("CodLista", "B");
            #region Pagos
            if (confirma.Negocio.Canje != true)
            {
                xmlWriter.WriteStartElement("Pagos");
                xmlWriter.WriteElementString("ProvinciaPago", string.Empty);
                xmlWriter.WriteAttributeString("CodLista", "B");
                xmlWriter.WriteElementString("FechaCondicionPago", confirma.Negocio.Canje == true ? "" : (esFijacionContratoConvenio ? "4 días hábiles de fecha de fijación" : (!(confirma.Negocio.PagoDiferido == true) ? "Días de diferimiento contra mercadería entregada" : (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (confirma.Negocio.Warrant == true ? "Pago contra warrant" : (confirma.Negocio.CD == true ? "Pago contra CD" : "72 hrs contra mercadería entregada")) : ""))));
                xmlWriter.WriteElementString("LugarPago", "BUENOS AIRES");
                xmlWriter.WriteElementString("PagoAOrdenDe", string.Empty);
                xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.CorredorId > 0 ? (confirma.Negocio.PagoDirectoVendedor == true ? "1" : "2") : "");
                xmlWriter.WriteElementString("PorcPago", confirma.Negocio.PorcentajeDePago.ToString());
                xmlWriter.WriteEndElement();
            }
            #endregion Pagos
            #region FIJACION
            if (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
            {
                xmlWriter.WriteStartElement("Fijacion");
                    xmlWriter.WriteElementString("FijMinima",condiciones.CantidadMinima.ToString());
                    xmlWriter.WriteElementString("FijMaxima",condiciones.CantidadMaxima.ToString());
                    xmlWriter.WriteElementString("UnidadMedidaFijacion",string.Empty);
                    xmlWriter.WriteAttributeString("Caption", "K");
                    xmlWriter.WriteAttributeString("CodLista", "K");
                    xmlWriter.WriteElementString("FijPeriodo", "1");
                    xmlWriter.WriteElementString("FijFecDesde", CorregirFormatoFecha(condiciones.FechaDesde));
                    xmlWriter.WriteElementString("FijFecHasta", CorregirFormatoFecha(condiciones.FechaHasta));
                    xmlWriter.WriteElementString("PorcMultaIncumplimiento", "010");
                    xmlWriter.WriteElementString("ComunicacionFijacion", confirma.Negocio.PagoDirectoVendedor == true ? "2" : "1");
                xmlWriter.WriteEndElement();
            }
            #endregion FIJACION
            xmlWriter.WriteElementString("ProduccionVendedor", string.Empty);
            xmlWriter.WriteAttributeString("CodLista", confirma.Negocio.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (confirma.Negocio.PagoDirectoVendedor == true ? "1" : "4") : (confirma.Negocio.Consignatario == true ? "5" : "2"));
            #region APRECIO
            if (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                xmlWriter.WriteElementString("APrecio", string.Empty);
                xmlWriter.WriteAttributeString("CodLista", "1");
            }
            #endregion APRECIO
            xmlWriter.WriteElementString("TipoOperacion", "1");
            #region SioGranos
            xmlWriter.WriteStartElement("SioGranos");
                xmlWriter.WriteElementString("NumeroDeclaracion", estadoSAP.NumeroSio.ToString());
                xmlWriter.WriteStartElement("DetalleDeclaracion");
                    xmlWriter.WriteElementString("ModalidadOperacion", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista",string.Empty);
                    xmlWriter.WriteElementString("EsCompradorFinal", string.Empty);
                    xmlWriter.WriteElementString("ProvinciaDestino", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista",string.Empty);
                    xmlWriter.WriteElementString("LocalidadDestino", string.Empty);
                    xmlWriter.WriteElementString("LugarEntregaSIO", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista",string.Empty);
                    xmlWriter.WriteElementString("CondicionPago", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista",string.Empty);
                    xmlWriter.WriteElementString("OpcionFijacion", string.Empty);
                    xmlWriter.WriteAttributeString("CodLista",string.Empty);
                    xmlWriter.WriteElementString("Observaciones", string.Empty);
                xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            #endregion SioGranos

            xmlWriter.WriteEndElement(); //Fin DetalleContrato
            #endregion DetalleContrato
            #region ExtendedData
            xmlWriter.WriteStartElement("ExtendedData");
            xmlWriter.WriteElementString("ExtendedDataItem",string.Empty);
            xmlWriter.WriteAttributeString("Caption", string.Empty);
            xmlWriter.WriteAttributeString("DataName", string.Empty);
            xmlWriter.WriteEndElement();
            #endregion ExtendedData
            #region Clausulas
            xmlWriter.WriteStartElement("Clausulas");
            foreach (var clausula in clausulas)
            {
                xmlWriter.WriteStartElement("Clausula");
                xmlWriter.WriteAttributeString("Orden", clausula.Orden.ToString());
                    xmlWriter.WriteElementString("TextoClausula",clausula.Texto);
                    xmlWriter.WriteElementString("TextoAdicionalClausula", string.Empty);
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            #endregion Clausulas
            xmlWriter.WriteEndElement(); //Fin DetalleDocumento
            xmlWriter.WriteEndElement(); //Fin Nodo Documento
            xmlWriter.WriteEndElement(); //Fin NODO Lote   
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

        // CORREOS
        public string EnviarMailConfirmas()
        {
            List<string> correos = new List<string>() {  "dataagro@baufest.com" };
            EnviarMailBoleto("molinos agro S.A.", correos);

            return "ok";
        }

        private void EnviarMailBoleto(string razonSocial, List<string> emailproveedor)
        {
            var listaContratos = new List<string>();
            var listaProvedoores = new List<string>();
            var listaProvedooresContactos = new List<string>();
            List<Confirma> confirmasRecientes = repositorio.Listar<Confirma>().Where(c => c.IsWebService && c.FechaGeneracion.ToShortDateString().Equals(DateTime.Now.ToShortDateString())).ToList();

            if (confirmasRecientes.Count > 0)
            {
                if (!PermisosHelper.Is(PermisosDataAgro.NoRecibirMail))
                {
                    listaProvedooresContactos.Add("dataagro@molinosagro.com.ar");
                    logger.Debug("Enviando mail Confirma ");
                }

                string subject = "Confirmas Molinos Agro S.A.";

                foreach (Confirma item in confirmasRecientes)
                {

                    if (!listaProvedoores.Contains(item.Negocio.Proveedor.CUIT))
                        listaProvedoores.Add(item.Negocio.Proveedor.CUIT);

                    var correoproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == ((item.Negocio.CorredorId != null && item.Negocio.CorredorId > 0 ) ? item.Negocio.CorredorId : item.Negocio.ProveedorId) && x.Boleto == true);

                    listaProvedooresContactos.AddRange(correoproveedor);

                    listaContratos.Add(item.Negocio.ContratoSAP);
                }
                mailManager.EnviarMail(emailproveedor, subject, "", listaProvedooresContactos, CuerpoMailBoleto(httpContextManager.ObtenerPathLogoMail(), confirmasRecientes, listaProvedoores)); 
            }
        }

        private AlternateView CuerpoMailBoleto(String filePath, List<Confirma> contratos, List<string> proveedores)
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

            htmlBody += "Estimado, le informamos que ya se encuentran subidos al sistema confirma los siguientes contratos: <br/><br/>";
            foreach (string cuitProveedor in proveedores)
            {
                var contratosFiltradoProveedor = contratos.Where(c => c.Negocio.Proveedor.CUIT.Equals(cuitProveedor));

                htmlBody += "Proveedor:  " + cuitProveedor + ".<br/>";


                foreach (Confirma confirma in contratosFiltradoProveedor)
                {
                        htmlBody += "&emsp;Bolsa  " + confirma.Negocio.Bolsa.Descripcion + ".<br/>";
                        htmlBody += "&emsp;&emsp;" + confirma.Negocio.ContratoSAP + " de Molinos Agro S.A.<br/>";
                }
            }
            htmlBody += "En caso de tener alguna consulta ingresar www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales,<br/><br/>" +
                "www.molinosagro.com.ar <br/>" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br/><br/>Molinos Agro S.A.<br/><br/><br/><br/>";
            htmlBody += "<style> table, th, td{ }</style>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
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

        private string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private List<string> AgregarCeros(List<string> lista)
        {
            var result = new List<string>();
            foreach (var item in lista)
            {
                result.Add(item.PadLeft(10, '0'));
            }
            return result;
        }
    }
}