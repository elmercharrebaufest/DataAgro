using NLog;
using Molinos.DataAgro.Agent.ConfirmaQALoteBorradorService;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaLoteBorradorAgent : IConfirmaLoteBorradorAgent
    {
        // ── Dependencias ─────────────────────────────────────────────────────────
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IStatusContratoAgent status;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;

        // ── Configuración ────────────────────────────────────────────────────────
        private readonly string userConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string passConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string ambientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];
        private readonly string ambienteLocal = ConfigurationManager.AppSettings["AmbienteLocal"];
        private readonly string cuitMOA = ConfigurationManager.AppSettings["Cuit"];

        // CUITs de prueba recomendados por Confirma para Staging
        private readonly string cuit1 = "23555555555"; // VIOLETA
        private readonly string cuit2 = "23888888888"; // CELESTE

        // ── Constructor ──────────────────────────────────────────────────────────
        public ConfirmaLoteBorradorAgent(
            ILogger logger,
            IRepositorio repositorio,
            IStatusContratoAgent status,
            IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
        }

        // ════════════════════════════════════════════════════════════════════════
        // PUNTO DE ENTRADA
        // ════════════════════════════════════════════════════════════════════════

        public ConfirmaAltaLoteBorradorResultDto ConfirmaLoteBorrador(
            List<ResultadoClausula> clausulas,
            List<int> equipo,
            BasicoContrato contrato,
            EstadosConfirmaDto estadosConfirmaDto)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
                return DevolverResultadoPrueba();

            try
            {
                HabilitarSSLSiAmbientePruebas();

                string numeroSAP = ObtenerNumeroSAP(contrato);

                EstadoSAPDto estadoSAP = ValidarEstadoSAP(contrato, numeroSAP);

                DatosEstadoBoletoDto datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(
                    contrato.ContratoSAP,
                    contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : "");

                CondicionFijacionEstadoBoletoDto condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
                ValidarCondicionesFijacion(contrato, datosConfirma, condiciones, numeroSAP);

                bool esConvenio = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && contrato.Madre == true;
                bool esCanje = contrato.Canje == true;
                string tipoDocumento = ResolverTipoDocumento(contrato, esCanje, esConvenio);
                string nroContInterno = numeroSAP.TrimStart('0');
                string nroContratoInternoVendedor = contrato.ContratoVendedor != null ? contrato.ContratoVendedor : nroContInterno;
                string nroContratoInternoCorredor = contrato.ContratoCorredor;
                bool contratoTieneCorredor = contrato.CorredorId > 0;
                List<ConfirmaParteDto> partes = ArmarPartes(contrato, nroContInterno, nroContratoInternoVendedor, nroContratoInternoCorredor);

                logger.Info(
                    $"Datos Precalculados del Negocio de Confirma; Codigo:{numeroSAP}, " +
                    $"esCanje:{esCanje}, esConvenio:{esConvenio}, " +
                    $"Partes: {string.Join(" - ", partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");

                if (clausulas == null || clausulas.Count == 0)
                    throw new ArgumentNullException("Clausulas",
                        $"WS Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {numeroSAP}.");

                Lote lote = ConstruirLote(
                    contrato, clausulas, partes, estadoSAP,
                    condiciones, tipoDocumento, nroContInterno,
                    esCanje, esConvenio);

                logger.Debug(lote.ToXml());

                int logId = GuardarLogXml(lote.ToXml());

                altaLoteResult devolucion = LlamarServicioConfirma(lote);

                logger.Debug(devolucion.ToXml());

                ActualizarLogXml(logId, devolucion.ToXml());

                return ResultadoAltaDefinitiva(devolucion, estadosConfirmaDto);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: No se pudo procesar el XML en WS Confirma");
                throw;
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // VALIDACIONES Y RESOLUCIONES
        // ════════════════════════════════════════════════════════════════════════

        private void HabilitarSSLSiAmbientePruebas()
        {
            if (ambientePruebas == "1")
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, certificate, chain, sslPolicyErrors) => true;
        }

        private string ObtenerNumeroSAP(BasicoContrato contrato)
        {
            return contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                ? contrato.FijacionSAP
                : contrato.ContratoSAP;
        }

        private EstadoSAPDto ValidarEstadoSAP(BasicoContrato contrato, string numeroSAP)
        {
            EstadoSAPDto estadoSAP = status.ValidarEstado(contrato.ContratoSAP);

            if (estadoSAP is null)
                throw new ArgumentNullException("EstadoSAP",
                    $"WS Confirma - Error al consultar el estadoSAP asociado al contrato: {numeroSAP}");

            logger.Info(
                $"WS Confirma - Se Consulta el status del contrato SAP {numeroSAP} " +
                $"resultando STATUS: {estadoSAP.Status} y Mensaje: {estadoSAP.Mensaje}");

            return estadoSAP;
        }

        private void ValidarCondicionesFijacion(
            BasicoContrato contrato,
            DatosEstadoBoletoDto datosConfirma,
            CondicionFijacionEstadoBoletoDto condiciones,
            string numeroSAP)
        {
            bool esAFijarOFijacion = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR
                                  || contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION;
            if (!esAFijarOFijacion) return;

            bool sinCondiciones = datosConfirma is null
                               || (condiciones is null
                                   && contrato.EsFason != true
                                   && contrato.PrestamoDevolucion != true);

            if (sinCondiciones)
                throw new ArgumentNullException("Error CondicionFijacion",
                    $"WS Confirma - Se consultó el Estado del Boleto SAP del contrato {numeroSAP} " +
                    $"y no tiene condiciones de fijacion asociadas.");

            if (datosConfirma != null && condiciones != null)
                logger.Info(
                    $"WS Confirma - Se consultó el Estado del Boleto SAP del contrato {numeroSAP}. " +
                    $"Resultando las condiciones fijacion: " +
                    $"CantidadMaxima: {condiciones.CantidadMaxima} y CantidadMinima: {condiciones.CantidadMinima}.");
        }

        private string ResolverTipoDocumento(BasicoContrato contrato, bool esCanje, bool esConvenio)
        {
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) return "1";
            if (esCanje) return "17";
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR
                || esConvenio) return "3";
            return string.Empty;
        }

        private string ResolverCondicionPago(BasicoContrato contrato, bool esConvenio)
        {
            // A FIJAR o convenio
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio)
                return "4 días hábiles de fecha de fijación";

            // A PRECIO
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                // Fecha cierta sin CD
                if (contrato.CD != true && contrato.FechaCierta.HasValue)
                    return contrato.FechaCierta.Value.ToString("dd/MM/yyyy");

                // Fecha cierta con CD
                if (contrato.CD == true && contrato.FechaCierta.HasValue)
                    return contrato.FechaCierta.Value.ToString("dd/MM/yyyy") + " Pago Anticipado";

                // CD sin fecha cierta
                if (contrato.CD == true && !contrato.FechaCierta.HasValue)
                    return "Pago Anticipado";

                // Warrant
                if (contrato.Warrant == true)
                    return "Pago contra Warrant";

                // Pago diferido
                if (contrato.PagoDiferido == true)
                    return $"{contrato.Dias_Pesificado} Días de diferimiento contra mercadería entregada";

                // Default
                return "72 hs contra mercadería descargada.";
            }

            return null;
        }

        // ════════════════════════════════════════════════════════════════════════
        // PARTES
        // ════════════════════════════════════════════════════════════════════════

        private List<ConfirmaParteDto> ArmarPartes(BasicoContrato contrato, string nroContInterno, string nroContratoInternoVendedor, string nroContratoInternoCorredor)
        {

            bool esAmbientePrueba = ambienteLocal == "1" || ambientePruebas == "1";

            var partes = new List<ConfirmaParteDto>
            {
                new ConfirmaParteDto
                {
                    CodLista           = "1",
                    NroContratoInterno = nroContratoInternoVendedor,
                    CUIT               = esAmbientePrueba ? cuit1 : contrato.Cuit,
                    Sucursal           = string.Empty
                },
                new ConfirmaParteDto
                {
                    CodLista           = "3",
                    NroContratoInterno = nroContInterno + "V01",
                    CUIT               = cuitMOA,
                    Sucursal           = string.Empty
                }
            };

            if (contrato.CorredorId > 0)
                partes.Add(new ConfirmaParteDto
                {
                    CodLista = "2",
                    NroContratoInterno = nroContratoInternoCorredor,
                    CUIT = esAmbientePrueba ? cuit2 : contrato.CUITCorredor,
                    Sucursal = string.Empty
                });

            return partes;
        }

        // ════════════════════════════════════════════════════════════════════════
        // CONSTRUCCIÓN DEL LOTE
        // ════════════════════════════════════════════════════════════════════════

        private Lote ConstruirLote(
            BasicoContrato contrato,
            List<ResultadoClausula> clausulas,
            List<ConfirmaParteDto> partes,
            EstadoSAPDto estadoSAP,
            CondicionFijacionEstadoBoletoDto condiciones,
            string tipoDocumento,
            string nroContInterno,
            bool esCanje,
            bool esConvenio)
        {
            var xmlDoc = new XmlDocument();
            var nodeList = new List<XmlNode>();

            nodeList.Add(CrearNodoPartes(xmlDoc, partes));

            XmlElement detalleContrato = CrearDetalleContrato(
                xmlDoc, contrato, estadoSAP, condiciones,
                tipoDocumento, esCanje, esConvenio);

            nodeList.Add(detalleContrato);
            nodeList.Add(CrearExtendedData(xmlDoc));
            nodeList.Add(CrearClausulas(xmlDoc, clausulas));

            var documento = new LoteDocumento
            {
                CabeceraDocumento = new LoteDocumentoCabeceraDocumento
                {
                    Bolsa = new LoteDocumentoCabeceraDocumentoBolsa
                    {
                        CodLista = contrato.BolsaConfirma,
                        Text = new List<string>().ToArray()
                    },
                    TipoDocumento = new LoteDocumentoCabeceraDocumentoTipoDocumento
                    {
                        CodLista = tipoDocumento,
                        Text = new List<string>().ToArray()
                    },
                    Formulario = new LoteDocumentoCabeceraDocumentoFormulario
                    {
                        formversion = "1.04",
                        Text = new List<string>().ToArray()
                    }
                },
                UploadInfo = new LoteDocumentoUploadInfo
                {
                    Any = new XmlElement[]
                    {
                        CreateXmlElement("Workflow", contrato.CorredorId > 0 ? "4" : "7")
                    }
                },
                DetalleDocumento = new LoteDocumentoDetalleDocumento
                {
                    Any = nodeList.ToArray()
                }
            };

            return new Lote
            {
                EmpresaPresentante = cuitMOA,
                Documento = new[] { documento }
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        // NODOS XML — PARTES / EXTENDED DATA / CLAUSULAS
        // ════════════════════════════════════════════════════════════════════════

        private XmlElement CrearNodoPartes(XmlDocument xmlDoc, List<ConfirmaParteDto> partes)
        {
            XmlElement partesEl = xmlDoc.CreateElement("Partes");

            foreach (var parte in partes)
            {
                XmlElement parteEl = xmlDoc.CreateElement("Parte");
                parteEl.SetAttribute("CodLista", parte.CodLista);

                AgregarElemento(xmlDoc, parteEl, "NroContratoInterno", parte.NroContratoInterno);
                AgregarElemento(xmlDoc, parteEl, "CUIT", parte.CUIT);

                XmlElement sucursal = xmlDoc.CreateElement("Sucursal");
                sucursal.SetAttribute("CodLista", string.Empty);
                parteEl.AppendChild(sucursal);

                partesEl.AppendChild(parteEl);
            }

            return partesEl;
        }

        private XmlElement CrearExtendedData(XmlDocument xmlDoc)
        {
            XmlElement extendedData = xmlDoc.CreateElement("ExtendedData");
            XmlElement item = xmlDoc.CreateElement("ExtendedDataItem");
            item.SetAttribute("Caption", string.Empty);
            item.SetAttribute("DataName", string.Empty);
            extendedData.AppendChild(item);
            return extendedData;
        }

        private XmlElement CrearClausulas(XmlDocument xmlDoc, List<ResultadoClausula> clausulas)
        {
            XmlElement clausulasEl = xmlDoc.CreateElement("Clausulas");

            foreach (var clausula in clausulas)
            {
                XmlElement clausulaEl = xmlDoc.CreateElement("Clausula");
                clausulaEl.SetAttribute("Orden", string.Empty);
                AgregarElementoCData(xmlDoc, clausulaEl, "TextoClausula", clausula.Texto);
                AgregarElementoCData(xmlDoc, clausulaEl, "TextoAdicionalClausula", string.Empty);
                clausulasEl.AppendChild(clausulaEl);
            }

            return clausulasEl;
        }

        // ════════════════════════════════════════════════════════════════════════
        // DETALLE CONTRATO — orquesta todas las sub-secciones
        // ════════════════════════════════════════════════════════════════════════

        private XmlElement CrearDetalleContrato(
            XmlDocument xmlDoc,
            BasicoContrato contrato,
            EstadoSAPDto estadoSAP,
            CondicionFijacionEstadoBoletoDto condiciones,
            string tipoDocumento,
            bool esCanje,
            bool esConvenio)
        {
            XmlElement det = xmlDoc.CreateElement("DetalleContrato");

            AgregarProducto(xmlDoc, det, contrato);
            AgregarElemento(xmlDoc, det, "DescAdicional", esCanje ? "INSUMO" : null);
            AgregarElemento(xmlDoc, det, "FechaConcertacion",
                contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null);
            AgregarAtributo(xmlDoc, det, "Cosecha", "CodLista", contrato.CampanaConfirma);
            AgregarAtributo(xmlDoc, det, "UnidadMedida", "CodLista", "K");
            AgregarElemento(xmlDoc, det, "CantidadDesde",
                (contrato.Cantidad).ToString());
            AgregarElemento(xmlDoc, det, "CantidadHasta",
                (contrato.Cantidad).ToString());
            AgregarAtributo(xmlDoc, det, "Ajuste", "CodLista", string.Empty);
            AgregarElemento(xmlDoc, det, "CantCamiones", contrato.CantidadCamiones.ToString());

            if (esCanje || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                det.AppendChild(xmlDoc.CreateElement("MontoImponible"));

            AgregarAtributo(xmlDoc, det, "Moneda", "CodLista",
                contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty);


            decimal? precioNetoSustentable = null;
            if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId.HasValue)
            {
                if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                {
                    precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                }
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                var precioContrato = Convert.ToString((contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio);
                AgregarElemento(xmlDoc, det, "Precio", precioContrato);
                AgregarAtributo(xmlDoc, det, "UnidadMedidaPrecio", "CodLista", "T");
            }
            if (contrato.CorredorId > 0)
                AgregarElemento(xmlDoc, det, "PorcComisionComprador", "1");

            AgregarCalidad(xmlDoc, det, contrato);
            AgregarAtributo(xmlDoc, det, "MedioTransporte", "CodLista", "C");
            AgregarEntregas(xmlDoc, det, contrato);
            AgregarOrigen(xmlDoc, det, contrato);
            AgregarDestino(xmlDoc, det, contrato);
            AgregarDecisionDeclaraSiCanje(xmlDoc, det, esCanje);
            AgregarAtributo(xmlDoc, det, "ProvinciaInstrumentacion", "CodLista", "B");
            AgregarPagosSiCorresponde(xmlDoc, det, contrato, esCanje, esConvenio);
            AgregarInsumosSiCanje(xmlDoc, det, contrato, esCanje);
            AgregarFijacionSiCorresponde(xmlDoc, det, contrato, condiciones);
            AgregarProduccionVendedor(xmlDoc, det, contrato);

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                AgregarAtributo(xmlDoc, det, "APrecio", "CodLista", "1");

            AgregarAtributo(xmlDoc, det, "TipoOperacion", "CodLista", "1");
            AgregarSioGranos(xmlDoc, det, estadoSAP, esCanje);

            return det;
        }

        // ════════════════════════════════════════════════════════════════════════
        // SUB-SECCIONES DE DETALLE CONTRATO
        // ════════════════════════════════════════════════════════════════════════
        private decimal? PrecioNetoSustentableSobrePrecio(BasicoContrato contrato, decimal? precioNetoSustentable)
        {
            if (contrato.AperturaPrecios != null && contrato.AperturaPrecios.Count > 0)
            {
                decimal porcentajeComision = contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 3).FirstOrDefault()?.Porcentaje ?? 0;
                decimal precioOriginal = contrato.Precio;
                decimal precioTarifaFlete = contrato.TarifaFlete ?? 0;
                precioOriginal += contrato.Importe_Sustentable ?? 0;
                precioOriginal += contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 1).FirstOrDefault()?.Importe ?? 0;
                precioOriginal += contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 2).FirstOrDefault()?.Importe ?? 0;
                precioOriginal += contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 4).FirstOrDefault()?.Importe ?? 0;
                precioOriginal += (contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 4).FirstOrDefault()?.Porcentaje ?? 0) * contrato.Precio / 100;
                porcentajeComision /= 100;
                precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
                precioOriginal += contrato.AperturaPrecios.Where(a => a.ConceptoAperturaPrecioId == 3).FirstOrDefault()?.Importe ?? 0;
                precioNetoSustentable = Math.Round(precioOriginal, 2);
            }
            return precioNetoSustentable;
        }
        private void AgregarProducto(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            string codLista =
                contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" :
                contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" :
                contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" :
                contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" :
                contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" :
                string.Empty;

            AgregarAtributo(xmlDoc, det, "Producto", "CodLista", codLista);
        }

        private void AgregarCalidad(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            string codLista =
                (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA ||
                 contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" :
                 contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" :
                 string.Empty;

            XmlElement calidad = xmlDoc.CreateElement("Calidad");
            AgregarAtributo(xmlDoc, calidad, "CondicionesCalidad", "CodLista", codLista);
            calidad.AppendChild(xmlDoc.CreateElement("OtrasCondicionesCalidad"));
            det.AppendChild(calidad);
        }

        private void AgregarEntregas(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            XmlElement entregas = xmlDoc.CreateElement("Entregas");
            AgregarElemento(xmlDoc, entregas, "EntregaDesde",
                contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null);
            AgregarElemento(xmlDoc, entregas, "EntregaHasta",
                contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null);
            det.AppendChild(entregas);
        }

        private void AgregarOrigen(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            XmlElement origen = xmlDoc.CreateElement("Origen");
            AgregarElemento(xmlDoc, origen, "LocalidadOrigen", contrato.LocalidadConfirma);
            AgregarAtributo(xmlDoc, origen, "ProvinciaOrigen", "CodLista", contrato.ProvinciaConfirma);
            det.AppendChild(origen);
        }

        private void AgregarDestino(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            XmlElement destino = xmlDoc.CreateElement("Destino");
            destino.SetAttribute("CodLista", contrato.DestinoConfirma);
            destino.SetAttribute("CodPrv", "0000");
            det.AppendChild(destino);
        }

        private void AgregarDecisionDeclaraSiCanje(XmlDocument xmlDoc, XmlElement det, bool esCanje)
        {
            if (!esCanje) return;
            AgregarAtributo(xmlDoc, det, "DecisionDeclaraPrecioUnit", "CodLista", "0");
            AgregarAtributo(xmlDoc, det, "DecisionDeclaraCantidad", "CodLista", "0");
        }

        private void AgregarPagosSiCorresponde(
            XmlDocument xmlDoc,
            XmlElement det,
            BasicoContrato contrato,
            bool esCanje,
            bool esConvenio)
        {
            if (esCanje) return;

            XmlElement pagos = xmlDoc.CreateElement("Pagos");
            AgregarAtributo(xmlDoc, pagos, "ProvinciaPago", "CodLista", "B");

            string condPago = ResolverCondicionPago(contrato, esConvenio);
            if (condPago != null)
                AgregarElemento(xmlDoc, pagos, "FechaCondicionPago", condPago);

            AgregarElemento(xmlDoc, pagos, "LugarPago", "BUENOS AIRES");
            AgregarAtributo(xmlDoc, pagos, "PagoAOrdenDe", "CodLista",
                contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1");
            AgregarElemento(xmlDoc, pagos, "PorcPago",
                contrato.PorcentajeDePago.HasValue
                    ? contrato.PorcentajeDePago.Value.ToString("F2", CultureInfo.InvariantCulture)
                    : string.Empty);

            det.AppendChild(pagos);
        }

        private void AgregarInsumosSiCanje(
            XmlDocument xmlDoc,
            XmlElement det,
            BasicoContrato contrato,
            bool esCanje)
        {
            if (!esCanje) return;

            XmlElement insumos = xmlDoc.CreateElement("Insumos");
            XmlElement productos = xmlDoc.CreateElement("Productos");
            XmlElement insumo = xmlDoc.CreateElement("Insumo");

            AgregarAtributo(xmlDoc, insumo, "Producto", "CodLista", "1");
            AgregarElemento(xmlDoc, insumo, "DescAdicional", "insumos");
            insumo.AppendChild(xmlDoc.CreateElement("Cantidad"));
            insumo.AppendChild(xmlDoc.CreateElement("Precio"));
            AgregarAtributo(xmlDoc, insumo, "UnidadMedida", "CodLista", string.Empty);
            AgregarAtributo(xmlDoc, insumo, "UnidadMedidaPrecio", "CodLista", string.Empty);

            productos.AppendChild(insumo);
            insumos.AppendChild(productos);

            AgregarAtributo(xmlDoc, insumos, "Moneda", "CodLista",
                contrato.Monto.HasValue
                    ? (contrato.MonedaCanjeId.Trim() == "ARP" ? "1" : "2")
                    : string.Empty);

            XmlElement precioTotal = xmlDoc.CreateElement("PrecioTotal");
            if (contrato.Monto.HasValue)
                precioTotal.InnerText = contrato.Monto.Value.ToString("F2", CultureInfo.InvariantCulture);
            insumos.AppendChild(precioTotal);

            insumos.AppendChild(xmlDoc.CreateElement("Factura"));
            insumos.AppendChild(xmlDoc.CreateElement("PorcentajeGastos"));
            insumos.AppendChild(xmlDoc.CreateElement("TipoCambioPesos"));
            insumos.AppendChild(xmlDoc.CreateElement("LugarEntrega"));
            AgregarElemento(xmlDoc, insumos, "ProvinciaEntrega", contrato.ProvinciaConfirma);

            det.AppendChild(insumos);
        }

        private void AgregarFijacionSiCorresponde(
            XmlDocument xmlDoc,
            XmlElement det,
            BasicoContrato contrato,
            CondicionFijacionEstadoBoletoDto condiciones)
        {
            bool aplica = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                       || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR;
            if (!aplica) return;

            XmlElement fij = xmlDoc.CreateElement("Fijacion");

            // FijMinima / FijMaxima
            XmlElement fijMin = xmlDoc.CreateElement("FijMinima");
            if (condiciones != null) fijMin.InnerText = Convert.ToInt32(condiciones.CantidadMinima).ToString();
            else if (contrato.KgMinimo > 0) fijMin.InnerText = contrato.KgMinimo.ToString();
            fij.AppendChild(fijMin);

            XmlElement fijMax = xmlDoc.CreateElement("FijMaxima");
            if (condiciones != null) fijMax.InnerText = Convert.ToInt32(condiciones.CantidadMaxima).ToString();
            else if (contrato.KgMaximo > 0) fijMax.InnerText = contrato.KgMaximo.ToString();
            fij.AppendChild(fijMax);

            XmlElement umFij = xmlDoc.CreateElement("UnidadMedidaFijacion");
            umFij.SetAttribute("Caption", "K");
            umFij.SetAttribute("CodLista", "K");
            fij.AppendChild(umFij);

            AgregarElemento(xmlDoc, fij, "FijPeriodo", "1");

            // Fechas desde / hasta
            XmlElement fecDesde = xmlDoc.CreateElement("FijFecDesde");
            if (condiciones != null) fecDesde.InnerText = CorregirFormatoFecha(condiciones.FechaDesde);
            else if (contrato.FechaDesde.HasValue) fecDesde.InnerText = contrato.FechaDesde.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            fij.AppendChild(fecDesde);

            XmlElement fecHasta = xmlDoc.CreateElement("FijFecHasta");
            if (condiciones != null) fecHasta.InnerText = CorregirFormatoFecha(condiciones.FechaHasta);
            else if (contrato.FechaHasta.HasValue) fecHasta.InnerText = contrato.FechaHasta.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
            fij.AppendChild(fecHasta);

            AgregarElemento(xmlDoc, fij, "PorcMultaIncumplimiento", "010");
            AgregarAtributo(xmlDoc, fij, "ComunicacionFijacion", "CodLista",
                contrato.PagoDirectoVendedor == true ? "2" : "1");

            if (contrato.Pizarra == true)
                AgregarAtributo(xmlDoc, fij, "PizarraFijacion", "CodLista", "1");

            det.AppendChild(fij);
        }

        private void AgregarProduccionVendedor(XmlDocument xmlDoc, XmlElement det, BasicoContrato contrato)
        {
            string codLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor
                ? (contrato.CorredorId > 0 ? "4" : "1")
                : (contrato.Consignatario == true ? "5" : "2");

            AgregarAtributo(xmlDoc, det, "ProduccionVendedor", "CodLista", codLista);
        }

        private void AgregarSioGranos(
            XmlDocument xmlDoc,
            XmlElement det,
            EstadoSAPDto estadoSAP,
            bool esCanje)
        {
            if (estadoSAP.NumeroSio > 0)
            {

                XmlElement sio = xmlDoc.CreateElement("SioGranos");

                XmlElement nroDecl = xmlDoc.CreateElement("NumeroDeclaracion");
                if (estadoSAP.NumeroSio > 0)
                    nroDecl.InnerText = estadoSAP.NumeroSio.ToString("D11");
                sio.AppendChild(nroDecl);

                XmlElement detalleDec = xmlDoc.CreateElement("DetalleDeclaracion");
                AgregarAtributo(xmlDoc, detalleDec, "ModalidadOperacion", "CodLista", esCanje ? "2" : "1");
                detalleDec.AppendChild(xmlDoc.CreateElement("EsCompradorFinal"));
                AgregarAtributo(xmlDoc, detalleDec, "ProvinciaDestino", "CodLista", string.Empty);
                detalleDec.AppendChild(xmlDoc.CreateElement("LocalidadDestino"));
                AgregarAtributo(xmlDoc, detalleDec, "LugarEntregaSIO", "CodLista", string.Empty);
                AgregarAtributo(xmlDoc, detalleDec, "CondicionPago", "CodLista", string.Empty);
                AgregarAtributo(xmlDoc, detalleDec, "OpcionFijacion", "CodLista", string.Empty);
                detalleDec.AppendChild(xmlDoc.CreateElement("Observaciones"));

                sio.AppendChild(detalleDec);
                det.AppendChild(sio);
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // SERVICIO CONFIRMA
        // ════════════════════════════════════════════════════════════════════════

        private altaLoteResult LlamarServicioConfirma(Lote lote)
        {
            using (var agent = new LoteBorradorServiceClient("Default11"))
            {
                try
                {
                    agent.ClientCredentials.UserName.UserName = userConfirma;
                    agent.ClientCredentials.UserName.Password = passConfirma;
                    return agent.AltaBorrador(lote);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error al llamar al servicio AltaBorrador de Confirma");
                    if (agent.State == System.ServiceModel.CommunicationState.Faulted)
                        agent.Abort();
                    throw;
                }
            }
        }

        // ════════════════════════════════════════════════════════════════════════
        // LOG
        // ════════════════════════════════════════════════════════════════════════

        private int GuardarLogXml(string xml)
        {
            var log = new Log { Fecha = DateTime.Now, Xml = xml };
            var logId = repositorio.Agregar(log);
            repositorio.GuardarCambios();
            return logId.Id;
        }

        private void ActualizarLogXml(int logId, string xmlAdicional)
        {
            var log = repositorio.Obtener<Log>(logId);
            log.Xml += xmlAdicional;
            repositorio.GuardarCambios();
        }

        // ════════════════════════════════════════════════════════════════════════
        // RESULTADO ALTA DEFINITIVA  (método público existente — sin cambios)
        // ════════════════════════════════════════════════════════════════════════

        public ConfirmaAltaLoteBorradorResultDto ResultadoAltaDefinitiva(
            altaLoteResult devolucion,
            EstadosConfirmaDto estadosConfirmaDto)
        {
            var result = new ConfirmaAltaLoteBorradorResultDto
            {
                altaIdLote = devolucion.altaIdLote,
                altaEstado = SoloDigitos(devolucion.altaEstado.ToString()),
            };

            result.confirmaAltaEstado = estadosConfirmaDto.ConfirmaAltaEstadoDto
                .Find(x => x.CodigoConfirmaAltaEstado == result.altaEstado);
            result.altaEstadoSpecified = devolucion.altaEstadoSpecified;
            result.altaEstadoDetalleError = devolucion.altaEstadoDetalleError;
            result.altaEstadoLote = SoloDigitos(devolucion.altaEstadoLote.ToString());
            result.confirmaAltaEstadoLote = estadosConfirmaDto.ConfirmaAltaEstadoLoteDto
                .Find(x => x.CodigoConfirmaAltaEstadoLote == result.altaEstadoLote);
            result.altaEstadoLoteSpecified = devolucion.altaEstadoLoteSpecified;
            result.altaItem = new List<altaItemBorradorDto>();

            if (devolucion.altaItem == null) return result;

            foreach (var item in devolucion.altaItem)
            {
                var altaItem = new altaItemBorradorDto
                {
                    altaIdLote = item.altaIdLote,
                    altaIdBolsa = item.altaIdBolsa,
                    altaEstadoDocumento = SoloDigitos(item.altaEstadoDocumento.ToString())
                };

                altaItem.confirmaAltaEstadoDocumento = estadosConfirmaDto.ConfirmaAltaEstadoDocumentoDto
                    .Find(x => x.CodigoConfirmaAltaEstadoDocumento == altaItem.altaEstadoDocumento);

                altaItem.altaErrores = item.altaErrores != null
                    ? new List<string>(item.altaErrores)
                    : new List<string>();

                altaItem.altaIdDocumentoExistenteLote = item.altaIdDocumentoExistenteLote;
                altaItem.altaIdDocumentoExistente = item.altaIdDocumentoExistente;
                altaItem.codigo = item.codigo;

                result.altaItem.Add(altaItem);
            }

            return result;
        }

        // ════════════════════════════════════════════════════════════════════════
        // RESULTADO DE PRUEBA
        // ════════════════════════════════════════════════════════════════════════

        private static ConfirmaAltaLoteBorradorResultDto DevolverResultadoPrueba()
        {
            return new ConfirmaAltaLoteBorradorResultDto
            {
                altaIdLote = "18443050",
                altaEstado = 1,
                altaEstadoSpecified = true,
                altaEstadoLote = 4,
                altaEstadoLoteSpecified = true,
                altaItem = new List<altaItemBorradorDto>
                {
                    new altaItemBorradorDto
                    {
                        altaIdLote                   = "",
                        altaIdBolsa                  = "",
                        altaEstadoDocumento          = 6,
                        altaErrores                  = new List<string> { "Documento existente" },
                        altaIdDocumentoExistente      = "18390504",
                        altaIdDocumentoExistenteLote  = "25007245",
                        codigo                       = "",
                        confirmaAltaEstadoDocumento  = new ConfirmaAltaEstadoDocumentoDto
                        {
                            Id = 6,
                            Descripcion = "Documento existente",
                            CodigoConfirmaAltaEstadoDocumento = 6
                        }
                    }
                },
                confirmaAltaEstado = new ConfirmaAltaEstadoDto
                {
                    Id = 1,
                    Descripcion = "Correcta",
                    CodigoConfirmaAltaEstado = 1
                },
                confirmaAltaEstadoLote = new ConfirmaAltaEstadoLoteDto
                {
                    Id = 4,
                    Descripcion = "Procesado",
                    CodigoConfirmaAltaEstadoLote = 4
                }
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPERS XML
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Agrega un elemento hijo con InnerText al padre. Usar para códigos, fechas y números.</summary>
        private static void AgregarElemento(
            XmlDocument xmlDoc, XmlElement padre, string nombre, string valor)
        {
            XmlElement el = xmlDoc.CreateElement(nombre);
            if (valor != null) el.InnerText = valor;
            padre.AppendChild(el);
        }

        /// <summary>
        /// Agrega un elemento hijo envuelto en CDATA al padre.
        /// Usar para texto libre que pueda contener caracteres especiales XML: comillas, ampersands, corchetes angulares.
        /// </summary>
        private static void AgregarElementoCData(
            XmlDocument xmlDoc, XmlElement padre, string nombre, string valor)
        {
            XmlElement el = xmlDoc.CreateElement(nombre);
            if (valor != null)
            {
                XmlCDataSection cdata = xmlDoc.CreateCDataSection(valor);
                el.AppendChild(cdata);
            }
            padre.AppendChild(el);
        }

        /// <summary>Agrega un elemento hijo con un único atributo CodLista (o similar) al padre.</summary>
        private static void AgregarAtributo(
            XmlDocument xmlDoc, XmlElement padre,
            string nombreElemento, string nombreAtributo, string valorAtributo)
        {
            XmlElement el = xmlDoc.CreateElement(nombreElemento);
            el.SetAttribute(nombreAtributo, valorAtributo);
            padre.AppendChild(el);
        }

        /// <summary>Crea un XmlElement independiente con InnerText (para UploadInfo.Any).</summary>
        private static XmlElement CreateXmlElement(string nombre, string valor)
        {
            var doc = new XmlDocument();
            XmlElement el = doc.CreateElement(nombre);
            el.InnerText = valor;
            return el;
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPERS GENERALES
        // ════════════════════════════════════════════════════════════════════════

        /// <summary>Extrae solo los dígitos de un entero (patrón usado en ResultadoAltaDefinitiva).</summary>
        private static int SoloDigitos(string valor)
        {
            return int.Parse(new string(valor.Where(char.IsDigit).ToArray()));
        }

        public string CorregirFormatoFecha(string cadena)
        {
            return DateTime.Parse(cadena).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
