using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ConfirmaQALoteBorradorService;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Xml;
using Molinos.DataAgro.Entities.Common.Enums;
using System.Globalization;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaLoteBorradorAgent : IConfirmaLoteBorradorAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IStatusContratoAgent status;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;
        string userConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        string passConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        string ambientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];
        string ambienteLocal = ConfigurationManager.AppSettings["AmbienteLocal"];
        string cuitMOA = ConfigurationManager.AppSettings["Cuit"];
        // CUITs de prueba recomendado por Confirma para Staging
        string cuit1 = "23555555555"; // VIOLETA
        string cuit2 = "23888888888"; // CELESTE

        public ConfirmaLoteBorradorAgent(ILogger logger, IRepositorio repositorio, IStatusContratoAgent status, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
        }

        public ConfirmaAltaLoteBorradorResultDto ConfirmaLoteBorrador(List<ResultadoClausula> clausulas, List<int> equipo, BasicoContrato contrato, EstadosConfirmaDto estadosConfirmaDto)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
            {
                ConfirmaAltaLoteBorradorResultDto confirmaAltaLoteResult = new ConfirmaAltaLoteBorradorResultDto()
                {
                    altaIdLote = "18443050",
                    altaEstado = 1,
                    altaEstadoSpecified = true,
                    altaEstadoLote = 4,
                    altaEstadoLoteSpecified = true,
                    altaItem = new List<altaItemBorradorDto>(){
                        new altaItemBorradorDto()
                        {
                            altaIdLote = "",
                            altaIdBolsa = "",
                            altaEstadoDocumento = 6,
                            altaErrores = new List<string>(){ "Documento existente" },
                            altaIdDocumentoExistente = "18390504",
                            altaIdDocumentoExistenteLote = "25007245",
                            codigo = "",
                            confirmaAltaEstadoDocumento = new ConfirmaAltaEstadoDocumentoDto(){ Id = 6, Descripcion = "Documento existente", CodigoConfirmaAltaEstadoDocumento = 6 }
                        }
                    },
                    confirmaAltaEstado = new ConfirmaAltaEstadoDto() { Id = 1, Descripcion = "Correcta", CodigoConfirmaAltaEstado = 1 },
                    confirmaAltaEstadoLote = new ConfirmaAltaEstadoLoteDto() { Id = 4, Descripcion = "Procesado", CodigoConfirmaAltaEstadoLote = 4 }
                };

                return confirmaAltaLoteResult;
            }
            else
            {
                try
                {
                    // Deshabilita temporalmente la validación del certificado SSL. Aplicar sólo para UAT/Staging. No para PRD.
                    if (ambientePruebas == "1") ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                    LoteBorradorServiceClient agent = new LoteBorradorServiceClient();
                    agent.ClientCredentials.UserName.UserName = userConfirma;
                    agent.ClientCredentials.UserName.Password = passConfirma;

                    string numeroSAP = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP;

                    EstadoSAPDto estadoSAP = status.ValidarEstado(contrato.ContratoSAP);
                    if (estadoSAP is null) throw new ArgumentNullException("EstadoSAP", $"WS Confirma - Error al consultar el estadoSAP asociado al contrato: {numeroSAP}");
                    else logger.Info($"WS Confirma - Se Consulta el status del contrato SAP {numeroSAP} resultando STATUS: {estadoSAP.Status} y Mensaje: {estadoSAP.Mensaje}");

                    DatosEstadoBoletoDto datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(contrato.ContratoSAP, contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : "");
                    CondicionFijacionEstadoBoletoDto condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
                    if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
                    {
                        if (datosConfirma is null || (condiciones is null && contrato.EsFason != true && contrato.PrestamoDevolucion != true)) throw new ArgumentNullException("Error CondicionFijacion", $"WS Confirma - Se consultó el Estado del Boleto SAP del contrato {numeroSAP} y no tiene condiciones de fijacion asociadas.");
                        else if (datosConfirma != null && condiciones != null) logger.Info($"WS Confirma - Se consultó el Estado del Boleto SAP del contrato {numeroSAP}. Resultando las condiciones fijacion: CantidadMaxima: {condiciones.CantidadMaxima} y CantidadMinima: {condiciones.CantidadMinima}.");
                    }

                    bool esConvenio = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && contrato.Madre == true;
                    bool esCanje = contrato.Canje == true;
                    string tipoDocumento = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : esCanje ? "17" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "";
                    string nroContratoInterno = numeroSAP.TrimStart('0');

                    List<ConfirmaParteDto> Partes = new List<ConfirmaParteDto> {
                        new ConfirmaParteDto
                        {
                            CodLista = "1",
                            NroContratoInterno = nroContratoInterno, 
                            CUIT = ambienteLocal == "1" || ambientePruebas == "1" ? cuit1 : contrato.Cuit,
                            Sucursal = string.Empty
                        },
                        new ConfirmaParteDto
                        {
                            CodLista = "3",
                            NroContratoInterno = nroContratoInterno + "V01",
                            CUIT = cuitMOA,
                            Sucursal = string.Empty
                        }
                    };
                    if (contrato.CorredorId > 0)
                        Partes.Add(new ConfirmaParteDto
                        {
                            CodLista = "2",
                            NroContratoInterno = nroContratoInterno,
                            CUIT = ambienteLocal == "1" || ambientePruebas == "1" ? cuit2 : contrato.CUITCorredor,
                            Sucursal = string.Empty
                        });

                    logger.Info($"Datos Precalculados del Negocio de Confirma; Codigo:{numeroSAP}, esCanje:{esCanje}, esConvenio:{esConvenio}, Partes: {string.Join(" - ", Partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");
                    if (clausulas is null || clausulas.Count == 0) throw new ArgumentNullException("Clausulas", $"WS Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {numeroSAP}.");

                    Lote lote = new Lote();
                    #region Lote
                    lote.EmpresaPresentante = cuitMOA;
                    List<LoteDocumento> documentos = new List<LoteDocumento>();

                    #region documentos[]

                    XmlDocument xmlDoc = new XmlDocument(); // Crear un nuevo XmlDocument

                    // Crear la lista de nodos que serán asignados al array 'Any'
                    List<XmlNode> nodeList = new List<XmlNode>();

                    #region Partes
                    // Crear el elemento "Partes"
                    XmlElement partesElement = xmlDoc.CreateElement("Partes");

                    // Para cada objeto en 'Partes' (asumiendo que 'Partes' es una colección)
                    foreach (var parte in Partes)
                    {
                        // Crear el elemento "Parte"
                        XmlElement parteElement = xmlDoc.CreateElement("Parte");
                        parteElement.SetAttribute("CodLista", parte.CodLista);

                        // Crear y agregar el subelemento "NroContratoInterno"
                        XmlElement nroContratoElement = xmlDoc.CreateElement("NroContratoInterno");
                        nroContratoElement.InnerText = parte.NroContratoInterno;
                        parteElement.AppendChild(nroContratoElement);

                        // Crear y agregar el subelemento "CUIT"
                        XmlElement cuitElement = xmlDoc.CreateElement("CUIT");
                        cuitElement.InnerText = parte.CUIT;
                        parteElement.AppendChild(cuitElement);

                        // Crear y agregar el subelemento "Sucursal" con el atributo "CodLista"
                        XmlElement sucursalElement = xmlDoc.CreateElement("Sucursal");
                        sucursalElement.SetAttribute("CodLista", string.Empty);
                        parteElement.AppendChild(sucursalElement);

                        partesElement.AppendChild(parteElement);
                    }

                    // Agregar el nodo "Partes" a la lista de nodos
                    nodeList.Add(partesElement);
                    #endregion Partes

                    #region DetalleContrato
                    // Crear el elemento "DetalleContrato"
                    XmlElement detalleContratoElement = xmlDoc.CreateElement("DetalleContrato");

                    // Crear el subelemento "Producto" con el atributo "CodLista"
                    XmlElement productoElement = xmlDoc.CreateElement("Producto");
                    productoElement.SetAttribute("CodLista",
                        contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" :
                        contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" :
                        contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" :
                        contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" :
                        contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty);
                    detalleContratoElement.AppendChild(productoElement);

                    // Otros subelementos de "DetalleContrato"
                    XmlElement descAdicionalElement = xmlDoc.CreateElement("DescAdicional");
                    descAdicionalElement.InnerText = esCanje ? "INSUMO" : null;
                    detalleContratoElement.AppendChild(descAdicionalElement);

                    XmlElement fechaConcertacionElement = xmlDoc.CreateElement("FechaConcertacion");
                    fechaConcertacionElement.InnerText = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null;
                    detalleContratoElement.AppendChild(fechaConcertacionElement);

                    XmlElement cosechaElement = xmlDoc.CreateElement("Cosecha");
                    cosechaElement.SetAttribute("CodLista", contrato.CampanaConfirma);
                    detalleContratoElement.AppendChild(cosechaElement);

                    XmlElement unidadMedidaElement = xmlDoc.CreateElement("UnidadMedida");
                    unidadMedidaElement.SetAttribute("CodLista", "K");
                    detalleContratoElement.AppendChild(unidadMedidaElement);

                    // Otros subelementos según la lógica condicional
                    XmlElement cantidadDesdeElement = xmlDoc.CreateElement("CantidadDesde");
                    cantidadDesdeElement.InnerText = (contrato.KgMinimo > 0 ? contrato.KgMinimo : (int)contrato.Cantidad).ToString();
                    detalleContratoElement.AppendChild(cantidadDesdeElement);

                    XmlElement cantidadHastaElement = xmlDoc.CreateElement("CantidadHasta");
                    cantidadHastaElement.InnerText = (contrato.KgMaximo > 0 ? contrato.KgMaximo : (int)contrato.Cantidad).ToString();
                    detalleContratoElement.AppendChild(cantidadHastaElement);

                    XmlElement ajusteElement = xmlDoc.CreateElement("Ajuste");
                    ajusteElement.SetAttribute("CodLista", string.Empty);
                    detalleContratoElement.AppendChild(ajusteElement);

                    XmlElement cantCamionesElement = xmlDoc.CreateElement("CantCamiones");
                    cantCamionesElement.InnerText = contrato.CantidadCamiones.ToString();
                    detalleContratoElement.AppendChild(cantCamionesElement);

                    if (esCanje || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                    {
                        XmlElement montoImponibleElement = xmlDoc.CreateElement("MontoImponible");
                        detalleContratoElement.AppendChild(montoImponibleElement);
                    }

                    XmlElement monedaElement = xmlDoc.CreateElement("Moneda");
                    monedaElement.SetAttribute("CodLista", contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty);
                    detalleContratoElement.AppendChild(monedaElement);

                    if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                    {
                        XmlElement precioElement = xmlDoc.CreateElement("Precio");
                        precioElement.InnerText = contrato.Precio.ToString();
                        detalleContratoElement.AppendChild(precioElement);

                        XmlElement unidadMedidaPrecioElement = xmlDoc.CreateElement("UnidadMedidaPrecio");
                        unidadMedidaPrecioElement.SetAttribute("CodLista", "T");
                        detalleContratoElement.AppendChild(unidadMedidaPrecioElement);
                    }

                    if(tipoDocumento != "17"){
                        XmlElement porcComisionCompradorElement = xmlDoc.CreateElement("PorcComisionComprador");
                        porcComisionCompradorElement.InnerText = contrato.PorcentajeComision.HasValue ? contrato.PorcentajeDePago.Value.ToString("F2", CultureInfo.InvariantCulture) : string.Empty;
                        detalleContratoElement.AppendChild(porcComisionCompradorElement);
                    }

                    nodeList.Add(detalleContratoElement);
                    #endregion DetalleContrato

                    #region Calidad
                    // Crear el elemento "Calidad"
                    XmlElement calidadElement = xmlDoc.CreateElement("Calidad");

                    // Crear el subelemento "CondicionesCalidad" con el atributo "CodLista"
                    XmlElement condicionesCalidadElement = xmlDoc.CreateElement("CondicionesCalidad");
                    condicionesCalidadElement.SetAttribute("CodLista",
                        (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA ||
                         contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" :
                        (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : string.Empty));
                    calidadElement.AppendChild(condicionesCalidadElement);

                    // Crear el subelemento "OtrasCondicionesCalidad"
                    XmlElement otrasCondicionesCalidadElement = xmlDoc.CreateElement("OtrasCondicionesCalidad");
                    calidadElement.AppendChild(otrasCondicionesCalidadElement);

                    // Agregar "Calidad" al mismo nivel que "Partes" dentro de "DetalleDocumento"
                    detalleContratoElement.AppendChild(calidadElement);
                    #endregion Calidad

                    #region MedioTransporte
                    XmlElement medioTransporteElement = xmlDoc.CreateElement("MedioTransporte");
                    medioTransporteElement.SetAttribute("CodLista", "C");
                    detalleContratoElement.AppendChild(medioTransporteElement);
                    #endregion MedioTransporte

                    #region Entregas
                    // Crear el elemento "Entregas"
                    XmlElement entregasElement = xmlDoc.CreateElement("Entregas");

                    // Crear y agregar el subelemento "EntregaDesde"
                    XmlElement entregaDesdeElement = xmlDoc.CreateElement("EntregaDesde");
                    entregaDesdeElement.InnerText = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null;
                    entregasElement.AppendChild(entregaDesdeElement);

                    // Crear y agregar el subelemento "EntregaHasta"
                    XmlElement entregaHastaElement = xmlDoc.CreateElement("EntregaHasta");
                    entregaHastaElement.InnerText = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null;
                    entregasElement.AppendChild(entregaHastaElement);

                    detalleContratoElement.AppendChild(entregasElement);
                    #endregion Entregas

                    #region Origen
                    // Crear el elemento "Origen"
                    XmlElement origenElement = xmlDoc.CreateElement("Origen");

                    // Crear y agregar el subelemento "LocalidadOrigen"
                    XmlElement localidadOrigenElement = xmlDoc.CreateElement("LocalidadOrigen");
                    localidadOrigenElement.InnerText = contrato.LocalidadConfirma;
                    origenElement.AppendChild(localidadOrigenElement);

                    // Crear y agregar el subelemento "ProvinciaOrigen" con el atributo "CodLista"
                    XmlElement provinciaOrigenElement = xmlDoc.CreateElement("ProvinciaOrigen");
                    XmlAttribute codListaAttr = xmlDoc.CreateAttribute("CodLista");
                    codListaAttr.Value = contrato.ProvinciaConfirma;
                    provinciaOrigenElement.Attributes.Append(codListaAttr);
                    origenElement.AppendChild(provinciaOrigenElement);

                    detalleContratoElement.AppendChild(origenElement);
                    #endregion Origen

                    #region Destino
                    // Crear el elemento "Destino"
                    XmlElement destinoElement = xmlDoc.CreateElement("Destino");

                    // Crear y agregar el atributo "CodLista"
                    XmlAttribute codListaAttr1 = xmlDoc.CreateAttribute("CodLista");
                    codListaAttr1.Value = contrato.DestinoConfirma;
                    destinoElement.Attributes.Append(codListaAttr1);

                    // Crear y agregar el atributo "CodPrv"
                    XmlAttribute codPrvAttr = xmlDoc.CreateAttribute("CodPrv");
                    codPrvAttr.Value = "0000"; // Valor fijo según tu ejemplo
                    destinoElement.Attributes.Append(codPrvAttr);

                    detalleContratoElement.AppendChild(destinoElement);
                    #endregion Destino

                    #region DecisionDeclara
                    if (esCanje)
                    {
                        // Crear el elemento "DecisionDeclaraPrecioUnit"
                        XmlElement decisionDeclaraPrecioUnitElement = xmlDoc.CreateElement("DecisionDeclaraPrecioUnit");
                        // Crear y agregar el atributo "CodLista"
                        XmlAttribute codListaAttrPrecioUnit = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrPrecioUnit.Value = "0";
                        decisionDeclaraPrecioUnitElement.Attributes.Append(codListaAttrPrecioUnit);

                        detalleContratoElement.AppendChild(decisionDeclaraPrecioUnitElement);

                        // Crear el elemento "DecisionDeclaraCantidad"
                        XmlElement decisionDeclaraCantidadElement = xmlDoc.CreateElement("DecisionDeclaraCantidad");
                        // Crear y agregar el atributo "CodLista"
                        XmlAttribute codListaAttrCantidad = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrCantidad.Value = "0";
                        decisionDeclaraCantidadElement.Attributes.Append(codListaAttrCantidad);

                        detalleContratoElement.AppendChild(decisionDeclaraCantidadElement);
                    }
                    #endregion DecisionDeclara

                    #region ProvinciaInstrumentacion
                    // Crear el elemento "ProvinciaInstrumentacion"
                    XmlElement provinciaInstrumentacionElement = xmlDoc.CreateElement("ProvinciaInstrumentacion");
                    // Crear y agregar el atributo "CodLista"
                    XmlAttribute codListaAttrProvincia = xmlDoc.CreateAttribute("CodLista");
                    codListaAttrProvincia.Value = "B";
                    provinciaInstrumentacionElement.Attributes.Append(codListaAttrProvincia);

                    detalleContratoElement.AppendChild(provinciaInstrumentacionElement);
                    #endregion ProvinciaInstrumentacion

                    #region Pagos
                    // Crear el nodo "Pagos" si esCanje es diferente de true
                    if (esCanje != true)
                    {
                        // Crear el elemento "Pagos"
                        XmlElement pagosElement = xmlDoc.CreateElement("Pagos");

                        // Crear el elemento "ProvinciaPago" con el atributo "CodLista"
                        XmlElement provinciaPagoElement = xmlDoc.CreateElement("ProvinciaPago");
                        XmlAttribute codListaAttrProvinciaPago = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrProvinciaPago.Value = "B";
                        provinciaPagoElement.Attributes.Append(codListaAttrProvinciaPago);

                        // Agregar el elemento "ProvinciaPago" al elemento "Pagos"
                        pagosElement.AppendChild(provinciaPagoElement);

                        // Crear el elemento "FechaCondicionPago"
                        XmlElement fechaCondicionPagoElement = xmlDoc.CreateElement("FechaCondicionPago");
                        string fechaCondicionPagoValue = null;

                        if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio)
                        {
                            fechaCondicionPagoValue = "4 días hábiles de fecha de fijación";
                        }
                        else if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                        {
                            if (contrato.CD == true)
                            {
                                fechaCondicionPagoValue = "Pago Anticipado";
                            }
                            else if (contrato.Warrant == true)
                            {
                                fechaCondicionPagoValue = "Pago contra Warrant";
                            }
                            else if (contrato.PagoDiferido == true)
                            {
                                fechaCondicionPagoValue = "Días de diferimiento contra mercadería entregada";
                            }
                            else
                            {
                                fechaCondicionPagoValue = "72 hs contra mercadería descargada.";
                            }
                        }

                        if (fechaCondicionPagoValue != null)
                        {
                            fechaCondicionPagoElement.InnerText = fechaCondicionPagoValue;
                            pagosElement.AppendChild(fechaCondicionPagoElement);
                        }

                        // Crear el elemento "LugarPago"
                        XmlElement lugarPagoElement = xmlDoc.CreateElement("LugarPago");
                        lugarPagoElement.InnerText = "BUENOS AIRES";
                        pagosElement.AppendChild(lugarPagoElement);

                        // Crear el elemento "PagoAOrdenDe" con el atributo "CodLista"
                        XmlElement pagoAOrdenDeElement = xmlDoc.CreateElement("PagoAOrdenDe");
                        XmlAttribute codListaAttrPagoAOrdenDe = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrPagoAOrdenDe.Value = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1";
                        pagoAOrdenDeElement.Attributes.Append(codListaAttrPagoAOrdenDe);
                        pagosElement.AppendChild(pagoAOrdenDeElement);

                        // Crear el elemento "PorcPago"
                        XmlElement porcPagoElement = xmlDoc.CreateElement("PorcPago");
                        porcPagoElement.InnerText = contrato.PorcentajeDePago.HasValue ? contrato.PorcentajeDePago.Value.ToString("F2", CultureInfo.InvariantCulture) : string.Empty;
                        pagosElement.AppendChild(porcPagoElement);

                        detalleContratoElement.AppendChild(pagosElement);
                    }
                    #endregion Pagos

                    #region Insumos
                    if (esCanje)
                    {
                        // Crear el elemento "Insumos"
                        XmlElement insumosElement = xmlDoc.CreateElement("Insumos");

                        // Crear el elemento "Productos"
                        XmlElement productosElement = xmlDoc.CreateElement("Productos");

                        // Crear el elemento "Insumo"
                        XmlElement insumoElement = xmlDoc.CreateElement("Insumo");

                        // Agregar los elementos dentro de "Insumo"
                        XmlElement productoElement1 = xmlDoc.CreateElement("Producto");
                        XmlAttribute codListaAttrProducto = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrProducto.Value = "1";
                        productoElement1.Attributes.Append(codListaAttrProducto);
                        insumoElement.AppendChild(productoElement1);

                        // Agregar el elemento "DescAdicional"
                        XmlElement descAdicionalElement1 = xmlDoc.CreateElement("DescAdicional");
                        descAdicionalElement1.InnerText = "insumos";
                        insumoElement.AppendChild(descAdicionalElement1);

                        // Agregar los elementos "Cantidad", "Precio", "UnidadMedida", y "UnidadMedidaPrecio"
                        insumoElement.AppendChild(xmlDoc.CreateElement("Cantidad"));
                        insumoElement.AppendChild(xmlDoc.CreateElement("Precio"));

                        XmlElement unidadMedidaElement1 = xmlDoc.CreateElement("UnidadMedida");
                        XmlAttribute codListaAttrUnidadMedida = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrUnidadMedida.Value = string.Empty;
                        unidadMedidaElement1.Attributes.Append(codListaAttrUnidadMedida);
                        insumoElement.AppendChild(unidadMedidaElement1);

                        XmlElement unidadMedidaPrecioElement = xmlDoc.CreateElement("UnidadMedidaPrecio");
                        XmlAttribute codListaAttrUnidadMedidaPrecio = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrUnidadMedidaPrecio.Value = string.Empty;
                        unidadMedidaPrecioElement.Attributes.Append(codListaAttrUnidadMedidaPrecio);
                        insumoElement.AppendChild(unidadMedidaPrecioElement);

                        // Agregar "Insumo" a "Productos"
                        productosElement.AppendChild(insumoElement);

                        // Agregar "Productos" a "Insumos"
                        insumosElement.AppendChild(productosElement);

                        // Agregar otros elementos a "Insumos"
                        XmlElement monedaElement1 = xmlDoc.CreateElement("Moneda");
                        XmlAttribute codListaAttrMoneda = xmlDoc.CreateAttribute("CodLista");
                        codListaAttrMoneda.Value = contrato.Monto.HasValue ? (contrato.MonedaCanjeId.Trim() == "ARP" ? "1" :"2"):string.Empty;
                        monedaElement1.Attributes.Append(codListaAttrMoneda);
                        insumosElement.AppendChild(monedaElement1);

                        // Agregar "PrecioTotal"
                        XmlElement precioTotalElement = xmlDoc.CreateElement("PrecioTotal");
                        if(contrato.Monto.HasValue) precioTotalElement.InnerText = contrato.Monto.Value.ToString("F2", CultureInfo.InvariantCulture);
                        insumosElement.AppendChild(precioTotalElement);

                        // Agregar otros elementos vacíos
                        insumosElement.AppendChild(xmlDoc.CreateElement("Factura"));
                        insumosElement.AppendChild(xmlDoc.CreateElement("PorcentajeGastos"));
                        insumosElement.AppendChild(xmlDoc.CreateElement("TipoCambioPesos"));
                        insumosElement.AppendChild(xmlDoc.CreateElement("LugarEntrega"));

                        // Agregar "ProvinciaEntrega" con el valor correspondiente
                        XmlElement provinciaEntregaElement = xmlDoc.CreateElement("ProvinciaEntrega");
                        provinciaEntregaElement.InnerText = contrato.ProvinciaConfirma;
                        insumosElement.AppendChild(provinciaEntregaElement);

                        detalleContratoElement.AppendChild(insumosElement);
                    }
                    #endregion Insumos

                    #region Fijacion
                    // Crear el nodo "Fijacion" si el tipo de negocio es "FIJACION" o "A_FIJAR"
                    if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                    {
                        // Crear el elemento "Fijacion"
                        XmlElement fijacionElement = xmlDoc.CreateElement("Fijacion");

                        // Agregar "FijMinima"
                        XmlElement fijMinimaElement = xmlDoc.CreateElement("FijMinima");
                        if (condiciones != null)
                        {
                            fijMinimaElement.InnerText = Convert.ToInt32(condiciones.CantidadMinima).ToString();
                        }
                        else if (contrato.KgMinimo > 0)
                        {
                            fijMinimaElement.InnerText = contrato.KgMinimo.ToString();
                        }
                        fijacionElement.AppendChild(fijMinimaElement);

                        // Agregar "FijMaxima"
                        XmlElement fijMaximaElement = xmlDoc.CreateElement("FijMaxima");
                        if (condiciones != null)
                        {
                            fijMaximaElement.InnerText = Convert.ToInt32(condiciones.CantidadMaxima).ToString();
                        }
                        else if (contrato.KgMaximo > 0)
                        {
                            fijMaximaElement.InnerText = contrato.KgMaximo.ToString();
                        }
                        fijacionElement.AppendChild(fijMaximaElement);

                        // Agregar "UnidadMedidaFijacion"
                        XmlElement unidadMedidaFijacionElement = xmlDoc.CreateElement("UnidadMedidaFijacion");
                        XmlAttribute captionAttr = xmlDoc.CreateAttribute("Caption");
                        captionAttr.Value = "K";
                        unidadMedidaFijacionElement.Attributes.Append(captionAttr);
                        XmlAttribute codListaAttr5 = xmlDoc.CreateAttribute("CodLista");
                        codListaAttr5.Value = "K";
                        unidadMedidaFijacionElement.Attributes.Append(codListaAttr5);
                        fijacionElement.AppendChild(unidadMedidaFijacionElement);

                        // Agregar "FijPeriodo"
                        XmlElement fijPeriodoElement = xmlDoc.CreateElement("FijPeriodo");
                        fijPeriodoElement.InnerText = "1";
                        fijacionElement.AppendChild(fijPeriodoElement);

                        // Agregar "FijFecDesde"
                        XmlElement fijFecDesdeElement = xmlDoc.CreateElement("FijFecDesde");
                        if (condiciones != null)
                        {
                            fijFecDesdeElement.InnerText = CorregirFormatoFecha(condiciones.FechaDesde);
                        }
                        else if (contrato.FechaDesde.HasValue)
                        {
                            fijFecDesdeElement.InnerText = contrato.FechaDesde.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        fijacionElement.AppendChild(fijFecDesdeElement);

                        // Agregar "FijFecHasta"
                        XmlElement fijFecHastaElement = xmlDoc.CreateElement("FijFecHasta");
                        if (condiciones != null)
                        {
                            fijFecHastaElement.InnerText = CorregirFormatoFecha(condiciones.FechaHasta);
                        }
                        else if (contrato.FechaHasta.HasValue)
                        {
                            fijFecHastaElement.InnerText = contrato.FechaHasta.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        fijacionElement.AppendChild(fijFecHastaElement);

                        // Agregar "PorcMultaIncumplimiento"
                        XmlElement porcMultaElement = xmlDoc.CreateElement("PorcMultaIncumplimiento");
                        porcMultaElement.InnerText = "010";
                        fijacionElement.AppendChild(porcMultaElement);

                        // Agregar "ComunicacionFijacion"
                        XmlElement comunicacionFijacionElement = xmlDoc.CreateElement("ComunicacionFijacion");
                        XmlAttribute codListaComunicacionAttr = xmlDoc.CreateAttribute("CodLista");
                        codListaComunicacionAttr.Value = contrato.PagoDirectoVendedor == true ? "2" : "1";
                        comunicacionFijacionElement.Attributes.Append(codListaComunicacionAttr);
                        fijacionElement.AppendChild(comunicacionFijacionElement);

                        // Agregar "PizarraFijacion" si corresponde
                        if (contrato.Pizarra == true)
                        {
                            XmlElement pizarraFijacionElement = xmlDoc.CreateElement("PizarraFijacion");
                            XmlAttribute codListaPizarraAttr = xmlDoc.CreateAttribute("CodLista");
                            codListaPizarraAttr.Value = "1";
                            pizarraFijacionElement.Attributes.Append(codListaPizarraAttr);
                            fijacionElement.AppendChild(pizarraFijacionElement);
                        }

                        detalleContratoElement.AppendChild(fijacionElement);
                    }
                    #endregion Fijacion

                    #region ProduccionVendedor
                    XmlElement produccionVendedorElement = xmlDoc.CreateElement("ProduccionVendedor");

                    // Determinar el valor de "CodLista" según las condiciones del contrato
                    string codListaValue;
                    if (contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor)
                    {
                        codListaValue = contrato.CorredorId > 0 ? "4" : "1";
                    }
                    else
                    {
                        codListaValue = contrato.Consignatario == true ? "5" : "2";
                    }

                    // Agregar el atributo "CodLista"
                    XmlAttribute codListaAttr2 = xmlDoc.CreateAttribute("CodLista");
                    codListaAttr2.Value = codListaValue;
                    produccionVendedorElement.Attributes.Append(codListaAttr2);

                    detalleContratoElement.AppendChild(produccionVendedorElement);
                    #endregion ProduccionVendedor

                    #region APRECIO
                    if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                    {
                        // Crear el elemento "APrecio"
                        XmlElement aPrecioElement = xmlDoc.CreateElement("APrecio");

                        // Agregar el atributo "CodLista"
                        XmlAttribute codListaAttr3 = xmlDoc.CreateAttribute("CodLista");
                        codListaAttr3.Value = "1"; // Asignar el valor "1"
                        aPrecioElement.Attributes.Append(codListaAttr3);

                        detalleContratoElement.AppendChild(aPrecioElement);
                    }
                    #endregion APRECIO

                    #region TipoOperacion
                    // Crear el elemento "TipoOperacion"
                    XmlElement tipoOperacionElement = xmlDoc.CreateElement("TipoOperacion");

                    // Crear el atributo "CodLista" y asignar el valor "1"
                    XmlAttribute codListaAttr4 = xmlDoc.CreateAttribute("CodLista");
                    codListaAttr4.Value = "1";

                    // Agregar el atributo al elemento "TipoOperacion"
                    tipoOperacionElement.Attributes.Append(codListaAttr4);

                    // Finalmente, agregar "TipoOperacion" a "DetalleDocumento"
                    detalleContratoElement.AppendChild(tipoOperacionElement);
                    #endregion TipoOperacion

                    #region SioGranos
                    // Crear el elemento "SioGranos"
                    XmlElement sioGranosElement = xmlDoc.CreateElement("SioGranos");

                    // Agregar el elemento "NumeroDeclaracion"
                    XmlElement numeroDeclaracionElement = xmlDoc.CreateElement("NumeroDeclaracion");
                    if(estadoSAP.NumeroSio > 0) numeroDeclaracionElement.InnerText = estadoSAP.NumeroSio.ToString();
                    sioGranosElement.AppendChild(numeroDeclaracionElement);

                    // Crear el elemento "DetalleDeclaracion"
                    XmlElement detalleDeclaracionElement = xmlDoc.CreateElement("DetalleDeclaracion");

                    // Agregar "ModalidadOperacion"
                    XmlElement modalidadOperacionElement = xmlDoc.CreateElement("ModalidadOperacion");
                    XmlAttribute modalidadCodListaAttr = xmlDoc.CreateAttribute("CodLista");
                    modalidadCodListaAttr.Value = esCanje ? "2" : "1";
                    modalidadOperacionElement.Attributes.Append(modalidadCodListaAttr);
                    detalleDeclaracionElement.AppendChild(modalidadOperacionElement);

                    // Agregar "EsCompradorFinal"
                    detalleDeclaracionElement.AppendChild(xmlDoc.CreateElement("EsCompradorFinal"));

                    // Agregar "ProvinciaDestino"
                    XmlElement provinciaDestinoElement = xmlDoc.CreateElement("ProvinciaDestino");
                    XmlAttribute provinciaCodListaAttr = xmlDoc.CreateAttribute("CodLista");
                    provinciaCodListaAttr.Value = string.Empty; // Valor vacío
                    provinciaDestinoElement.Attributes.Append(provinciaCodListaAttr);
                    detalleDeclaracionElement.AppendChild(provinciaDestinoElement);

                    // Agregar "LocalidadDestino"
                    detalleDeclaracionElement.AppendChild(xmlDoc.CreateElement("LocalidadDestino"));

                    // Agregar "LugarEntregaSIO"
                    XmlElement lugarEntregaSIOElement = xmlDoc.CreateElement("LugarEntregaSIO");
                    XmlAttribute lugarEntregaCodListaAttr = xmlDoc.CreateAttribute("CodLista");
                    lugarEntregaCodListaAttr.Value = string.Empty; // Valor vacío
                    lugarEntregaSIOElement.Attributes.Append(lugarEntregaCodListaAttr);
                    detalleDeclaracionElement.AppendChild(lugarEntregaSIOElement);

                    // Agregar "CondicionPago"
                    XmlElement condicionPagoElement = xmlDoc.CreateElement("CondicionPago");
                    XmlAttribute condicionPagoCodListaAttr = xmlDoc.CreateAttribute("CodLista");
                    condicionPagoCodListaAttr.Value = string.Empty; // Valor vacío
                    condicionPagoElement.Attributes.Append(condicionPagoCodListaAttr);
                    detalleDeclaracionElement.AppendChild(condicionPagoElement);

                    // Agregar "OpcionFijacion"
                    XmlElement opcionFijacionElement = xmlDoc.CreateElement("OpcionFijacion");
                    XmlAttribute opcionFijacionCodListaAttr = xmlDoc.CreateAttribute("CodLista");
                    opcionFijacionCodListaAttr.Value = string.Empty; // Valor vacío
                    opcionFijacionElement.Attributes.Append(opcionFijacionCodListaAttr);
                    detalleDeclaracionElement.AppendChild(opcionFijacionElement);

                    // Agregar "Observaciones"
                    detalleDeclaracionElement.AppendChild(xmlDoc.CreateElement("Observaciones"));

                    // Agregar "DetalleDeclaracion" a "SioGranos"
                    sioGranosElement.AppendChild(detalleDeclaracionElement);

                    detalleContratoElement.AppendChild(sioGranosElement);
                    #endregion SioGranos

                    #region ExtendedData
                    // Crear el elemento "ExtendedData"
                    XmlElement extendedDataElement = xmlDoc.CreateElement("ExtendedData");

                    // Crear el elemento "ExtendedDataItem"
                    XmlElement extendedDataItemElement = xmlDoc.CreateElement("ExtendedDataItem");

                    // Crear los atributos "Caption" y "DataName" y asignarles valores vacíos
                    XmlAttribute captionAttr1 = xmlDoc.CreateAttribute("Caption");
                    captionAttr1.Value = string.Empty;

                    XmlAttribute dataNameAttr = xmlDoc.CreateAttribute("DataName");
                    dataNameAttr.Value = string.Empty;

                    // Agregar los atributos al elemento "ExtendedDataItem"
                    extendedDataItemElement.Attributes.Append(captionAttr1);
                    extendedDataItemElement.Attributes.Append(dataNameAttr);

                    // Agregar "ExtendedDataItem" dentro de "ExtendedData"
                    extendedDataElement.AppendChild(extendedDataItemElement);

                    nodeList.Add(extendedDataElement);
                    #endregion ExtendedData

                    #region Clausulas
                    // Crear el elemento "Clausulas"
                    XmlElement clausulasElement = xmlDoc.CreateElement("Clausulas");

                    // Iterar sobre la colección de clausulasConfirma para crear cada "Clausula"
                    foreach (var x in clausulas)
                    {
                        // Crear el elemento "Clausula"
                        XmlElement clausulaElement = xmlDoc.CreateElement("Clausula");

                        // Crear el atributo "Orden" con valor vacío
                        XmlAttribute ordenAttr = xmlDoc.CreateAttribute("Orden");
                        ordenAttr.Value = string.Empty;

                        // Asignar el atributo al elemento "Clausula"
                        clausulaElement.Attributes.Append(ordenAttr);

                        // Crear el elemento "TextoClausula" con el texto de la clausula
                        XmlElement textoClausulaElement = xmlDoc.CreateElement("TextoClausula");
                        textoClausulaElement.InnerText = x.Texto;

                        // Crear el elemento "TextoAdicionalClausula" con valor vacío
                        XmlElement textoAdicionalClausulaElement = xmlDoc.CreateElement("TextoAdicionalClausula");
                        textoAdicionalClausulaElement.InnerText = string.Empty;

                        // Agregar "TextoClausula" y "TextoAdicionalClausula" dentro de "Clausula"
                        clausulaElement.AppendChild(textoClausulaElement);
                        clausulaElement.AppendChild(textoAdicionalClausulaElement);

                        // Agregar la "Clausula" al elemento "Clausulas"
                        clausulasElement.AppendChild(clausulaElement);
                    }

                    nodeList.Add(clausulasElement);
                    #endregion Clausulas


                    // Asignar la lista de nodos al array 'arrayXmlNode'
                    XmlNode[] arrayXmlNode = nodeList.ToArray();

                    documentos.Add(new LoteDocumento
                    {
                        CabeceraDocumento = new LoteDocumentoCabeceraDocumento()
                        {
                            Bolsa = new LoteDocumentoCabeceraDocumentoBolsa()
                            {
                                CodLista = contrato.BolsaConfirma,
                                Text = new List<string>().ToArray()
                            },
                            TipoDocumento = new LoteDocumentoCabeceraDocumentoTipoDocumento()
                            {
                                CodLista = tipoDocumento,
                                Text = new List<string>().ToArray()
                            },
                            Formulario = new LoteDocumentoCabeceraDocumentoFormulario()
                            {
                                formversion = "1.04",
                                Text = new List<string>().ToArray()
                            },
                        },
                        UploadInfo = new LoteDocumentoUploadInfo()
                        {
                            Any = new XmlElement[]
                            {
                                CreateXmlElement("Workflow", contrato.CorredorId > 0 ? "4" : "7")
                            }
                        },
                        DetalleDocumento = new LoteDocumentoDetalleDocumento()
                        {
                            Any = arrayXmlNode
                        },
                    });
                    #endregion documentos[]

                    lote.Documento = documentos.ToArray();
                    #endregion Lote

                    logger.Debug(lote.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = lote.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    altaLoteResult devolucion = agent.AltaBorrador(lote);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    ConfirmaAltaLoteBorradorResultDto confirmaAltaLoteResult = ResultadoAltaDefinitiva(devolucion, estadosConfirmaDto);

                    return confirmaAltaLoteResult;
                }
                catch (Exception e)
                {
                    logger.Error("Error: No se pudo procesar XML en WS Confirma", e);
                    throw;
                }
            }
        }

        public ConfirmaAltaLoteBorradorResultDto ResultadoAltaDefinitiva(altaLoteResult devolucion, EstadosConfirmaDto estadosConfirmaDto)
        {
            ConfirmaAltaLoteBorradorResultDto confirmaAltaLoteResult = new ConfirmaAltaLoteBorradorResultDto();

            confirmaAltaLoteResult.altaIdLote = devolucion.altaIdLote;
            confirmaAltaLoteResult.altaEstado = int.Parse(new string(devolucion.altaEstado.ToString().Where(char.IsDigit).ToArray()));
            confirmaAltaLoteResult.confirmaAltaEstado = estadosConfirmaDto.ConfirmaAltaEstadoDto.Find(x => x.CodigoConfirmaAltaEstado == confirmaAltaLoteResult.altaEstado);
            confirmaAltaLoteResult.altaEstadoSpecified = devolucion.altaEstadoSpecified;
            confirmaAltaLoteResult.altaEstadoDetalleError = devolucion.altaEstadoDetalleError;
            confirmaAltaLoteResult.altaEstadoLote = int.Parse(new string(devolucion.altaEstadoLote.ToString().Where(char.IsDigit).ToArray()));
            confirmaAltaLoteResult.confirmaAltaEstadoLote = estadosConfirmaDto.ConfirmaAltaEstadoLoteDto.Find(x => x.CodigoConfirmaAltaEstadoLote == confirmaAltaLoteResult.altaEstadoLote);
            confirmaAltaLoteResult.altaEstadoLoteSpecified = devolucion.altaEstadoLoteSpecified;
            confirmaAltaLoteResult.altaItem = new List<altaItemBorradorDto>();

            if (devolucion.altaItem != null)
            {
                foreach (var item in devolucion.altaItem)
                {
                    altaItemBorradorDto altaItem = new altaItemBorradorDto();
                    altaItem.altaIdLote = item.altaIdLote;
                    altaItem.altaIdBolsa = item.altaIdBolsa;
                    altaItem.altaEstadoDocumento = int.Parse(new string(item.altaEstadoDocumento.ToString().Where(char.IsDigit).ToArray()));
                    altaItem.confirmaAltaEstadoDocumento = estadosConfirmaDto.ConfirmaAltaEstadoDocumentoDto.Find(x => x.CodigoConfirmaAltaEstadoDocumento == altaItem.altaEstadoDocumento);
                    altaItem.altaErrores = new List<string>();

                    if (item.altaErrores != null)
                    {
                        foreach (var item2 in item.altaErrores)
                        {
                            altaItem.altaErrores.Add(item2);
                        }
                    }

                    altaItem.altaIdDocumentoExistenteLote = item.altaIdDocumentoExistenteLote;
                    altaItem.altaIdDocumentoExistente = item.altaIdDocumentoExistente;
                    altaItem.codigo = item.codigo;

                    confirmaAltaLoteResult.altaItem.Add(altaItem);
                }
            }

            return confirmaAltaLoteResult;
        }

        private static XmlElement CreateXmlElement(string elementName, string innerText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement xmlElement = xmlDoc.CreateElement(elementName);
            xmlElement.InnerText = innerText;
            return xmlElement;
        }

        public string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
