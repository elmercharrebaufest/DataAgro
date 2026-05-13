using Molinos.DataAgro.Agent.ConfirmaQALoteDocumentos;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaLoteDocumentosAgent : IConfirmaLoteDocumentosAgent
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
        private readonly string cuit1 = "30646328450";
        private readonly string cuit2 = "30500120882";

        // ── Constructor ──────────────────────────────────────────────────────────
        public ConfirmaLoteDocumentosAgent(
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

        public ConfirmaAltaLoteResultDto ConfirmaLoteDocumentos(
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
                string nroContratoInterno = numeroSAP.TrimStart('0');
                string nroContratoInternoVendedor = contrato.ContratoVendedor ?? nroContratoInterno;
                List<ConfirmaParteDto> partes = ArmarPartes(contrato, nroContratoInterno, nroContratoInternoVendedor);

                logger.Info(
                    $"Datos Precalculados del Negocio de Confirma; Codigo:{numeroSAP}, " +
                    $"esCanje:{esCanje}, esConvenio:{esConvenio}, " +
                    $"Partes: {string.Join(" - ", partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");

                if (clausulas == null || clausulas.Count == 0)
                    throw new ArgumentNullException("Clausulas",
                        $"WS Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {numeroSAP}.");

                Lote lote = ConstruirLote(
                    contrato, clausulas, partes, condiciones,
                    estadoSAP, nroContratoInterno, esCanje, esConvenio);

                logger.Debug(lote.ToXml());

                int logId = GuardarLogXml(lote.ToXml());

                altaLoteResult devolucion = LlamarServicioConfirma(lote);

                logger.Debug(devolucion.ToXml());

                ActualizarLogXml(logId, devolucion.ToXml());

                return ResultadoAltaDefinitiva(devolucion, estadosConfirmaDto);
            }
            catch (Exception e)
            {
                logger.Error(e, "Error: No se pudo procesar XML en WS Confirma");
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
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) return "3";
            if (esCanje) return "17";
            return string.Empty;
        }

        private string ResolverCondicionPago(BasicoContrato contrato, bool esConvenio)
        {
            if (contrato.Warrant == true)
                return "Pago contra Warrant";

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio)
                return "4 días hábiles de fecha de fijación";

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                if (contrato.CD != true && contrato.FechaCierta.HasValue)
                    return contrato.FechaCierta.Value.ToString("dd/MM/yyyy");
                if (contrato.CD == true && contrato.FechaCierta.HasValue)
                    return contrato.FechaCierta.Value.ToString("dd/MM/yyyy") + " Pago Anticipado";
                if (contrato.CD == true && !contrato.FechaCierta.HasValue)
                    return "Pago Anticipado";
                if (contrato.PagoDiferido == true)
                    return $"{contrato.Dias_Pesificado} Días de diferimiento contra mercadería entregada";
                return "72 hs contra mercadería descargada.";
            }

            return null;
        }

        private string ResolverCodListaMaterial(BasicoContrato contrato)
        {
            switch (contrato.MaterialId)
            {
                case (int)EnumMateriales.TRIGO: return "1";
                case (int)EnumMateriales.MAIZ: return "2";
                case (int)EnumMateriales.SORGO: return "3";
                case (int)EnumMateriales.GIRASOL: return "20";
                case (int)EnumMateriales.SOJA: return "21";
                default: return string.Empty;
            }
        }

        private string ResolverCodListaCalidad(BasicoContrato contrato)
        {
            if (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA ||
                contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) return "1";
            if (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA) return "4";
            return string.Empty;
        }

        private string ResolverCodListaProduccionVendedor(BasicoContrato contrato)
        {
            if (contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor)
                return contrato.CorredorId > 0 ? "4" : "1";
            return contrato.Consignatario == true ? "5" : "2";
        }

        // ════════════════════════════════════════════════════════════════════════
        // PARTES
        // ════════════════════════════════════════════════════════════════════════

        private List<ConfirmaParteDto> ArmarPartes(
            BasicoContrato contrato,
            string nroContratoInterno,
            string nroContratoInternoVendedor)
        {
            bool esAmbienteLocal = ambienteLocal == "1";

            var partes = new List<ConfirmaParteDto>
            {
                new ConfirmaParteDto
                {
                    CodLista           = "1",
                    NroContratoInterno = nroContratoInternoVendedor,
                    CUIT               = esAmbienteLocal ? cuit1 : contrato.Cuit,
                    Sucursal           = string.Empty
                },
                new ConfirmaParteDto
                {
                    CodLista           = "3",
                    NroContratoInterno = nroContratoInterno + "V01",
                    CUIT               = cuitMOA,
                    Sucursal           = string.Empty
                }
            };

            if (contrato.CorredorId > 0)
                partes.Add(new ConfirmaParteDto
                {
                    CodLista = "2",
                    NroContratoInterno = contrato.ContratoCorredor,
                    CUIT = esAmbienteLocal ? cuit2 : contrato.CUITCorredor,
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
            CondicionFijacionEstadoBoletoDto condiciones,
            EstadoSAPDto estadoSAP,
            string nroContratoInterno,
            bool esCanje,
            bool esConvenio)
        {
            bool esFijar = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                        || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR;

            object item1;
            if (esFijar && !esCanje)
                item1 = ConstruirDocumentoFijarPrecio(contrato, clausulas, partes, esCanje, esConvenio, condiciones, estadoSAP);
            else if (esFijar && esCanje)
                item1 = ConstruirDocumentoPagoEspecieFijarPrecio(contrato, clausulas, partes, esCanje, esConvenio, condiciones, estadoSAP);
            else if (!esFijar && esCanje)
                item1 = ConstruirDocumentoPagoEspeciePrecioHecho(contrato, clausulas, partes, esCanje, esConvenio, estadoSAP);
            else
                item1 = ConstruirDocumentoContratoPrecioHecho(contrato, clausulas, partes, esCanje, esConvenio, estadoSAP);

            return new Lote
            {
                EmpresaPresentante = cuitMOA,
                Items = new[]
                {
                    new Item
                    {
                        itemInfo = new ItemItemInfo
                        {
                            Workflow = contrato.CorredorId > 0
                                ? ItemItemInfoWorkflow.Item1
                                : ItemItemInfoWorkflow.Item7
                        },
                        Item1  = item1,
                        codigo = nroContratoInterno
                    }
                }
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        // DOCUMENTOS TYPED
        // ════════════════════════════════════════════════════════════════════════

        private DocumentoFijarPrecio ConstruirDocumentoFijarPrecio(
            BasicoContrato contrato,
            List<ResultadoClausula> clausulas,
            List<ConfirmaParteDto> partes,
            bool esCanje,
            bool esConvenio,
            CondicionFijacionEstadoBoletoDto condiciones,
            EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método ConstruirDocumentoFijarPrecio()");

            var det = new DetalleDocumentoFijarPrecioDetalleContrato
            {
                Producto = ConstruirProducto(contrato),
                DescAdicional = new TCaption { Value = esCanje ? "INSUMO" : string.Empty },
                FechaConcertacion = new TCaption { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null },
                Cosecha = new TCodLista { CodLista = contrato.CampanaConfirma },
                UnidadMedida = new TCodCaption { CodLista = "K" },
                CantidadDesde = new TCaption { Value = ((int)contrato.Cantidad).ToString() },
                CantidadHasta = new TCaption { Value = ((int)contrato.Cantidad).ToString() },
                Ajuste = new TCodLista { CodLista = string.Empty },
                CantCamiones = new TCaption(),
                MontoImponible = new TCaption { Value = string.Empty },
                Moneda = new TCodLista { CodLista = "2" },
                Calidad = ConstruirCalidad(contrato),
                MedioTransporte = new TCodCaption { CodLista = "C" },
                Entregas = ConstruirEntregas(contrato),
                Origen = ConstruirOrigen(contrato),
                Destino = ConstruirDestino(contrato),
                ProvinciaInstrumentacion = new TCodCaption { CodLista = "B" },
                ProduccionVendedor = new DetalleDocumentoFijarPrecioDetalleContratoProduccionVendedor { CodLista = ResolverCodListaProduccionVendedor(contrato) },
                TipoOperacion = new TCodLista { CodLista = "1" },
                DecisionPagoVoluntario = ConstruirDecisionPagoVoluntario(),

            };
            if (estadoSAP.NumeroSio > 0)
                det.SioGranos = ConstruirSioGranos(estadoSAP);

            if (contrato.CorredorId > 0)
                det.ComisionPorComprador = new TCaption { Value = "1" };

            if (!esCanje)
                det.Pagos = ConstruirPagos(contrato, esConvenio);

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ||
                contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                det.Fijacion = ConstruirFijacion(contrato, condiciones);

            return new DocumentoFijarPrecio
            {
                CabeceraDocumento = ConstruirCabeceraDocumento(contrato, esCanje, esConvenio),
                DetalleDocumento = new DetalleDocumentoFijarPrecio
                {
                    Partes = ConstruirPartes(partes),
                    DetalleContrato = det,
                    Clausulas = ConstruirClausulasDetalle(clausulas)
                }
            };
        }

        private DocumentoPagoEspecieFijarPrecio ConstruirDocumentoPagoEspecieFijarPrecio(
            BasicoContrato contrato,
            List<ResultadoClausula> clausulas,
            List<ConfirmaParteDto> partes,
            bool esCanje,
            bool esConvenio,
            CondicionFijacionEstadoBoletoDto condiciones,
            EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método ConstruirDocumentoPagoEspecieFijarPrecio()");

            var det = new DetalleDocumentoPagoEspecieFijarPrecioDetalleContrato
            {
                Producto = ConstruirProducto(contrato),
                DescAdicional = new TCaption { Value = esCanje ? "INSUMO" : string.Empty },
                FechaConcertacion = new TCaption { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null },
                Cosecha = new TCodLista { CodLista = contrato.CampanaConfirma },
                UnidadMedida = new TCodCaption { CodLista = "K" },
                CantidadDesde = new TCaption { Value = ((int)contrato.Cantidad).ToString() },
                CantidadHasta = new TCaption { Value = ((int)contrato.Cantidad).ToString() },
                Ajuste = new TCodLista { CodLista = string.Empty },
                CantCamiones = new TCaption(),
                MontoImponible = new TCaption { Value = string.Empty },
                Moneda = new TCodLista { CodLista = "2" },
                Calidad = ConstruirCalidad(contrato),
                MedioTransporte = new TCodCaption { CodLista = "C" },
                Entregas = ConstruirEntregas(contrato),
                Origen = ConstruirOrigen(contrato),
                Destino = ConstruirDestino(contrato),
                ProvinciaInstrumentacion = new TCodCaption { CodLista = "B" },
                ProduccionVendedor = new ProduccionVendedor { CodLista = ResolverCodListaProduccionVendedor(contrato) },
                TipoOperacion = new TCodLista { CodLista = "1" },
                DecisionPagoVoluntario = ConstruirDecisionPagoVoluntario(),

            };
            if (estadoSAP.NumeroSio > 0)
                det.SioGranos = ConstruirSioGranos(estadoSAP);

            if (contrato.CorredorId > 0)
                det.ComisionPorComprador = new TCaption { Value = "1" };

            if (esCanje)
            {
                det.DecisionDeclaraPrecioUnit = new DecisionDeclaraPrecioUnit { CodLista = "0" };
                det.DecisionDeclaraCantidad = new DecisionDeclaraCantidad { CodLista = "0" };
            }

            det.Insumos = ConstruirInsumosFijarPrecio(contrato);

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ||
                contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                det.Fijacion = ConstruirFijacion(contrato, condiciones);

            return new DocumentoPagoEspecieFijarPrecio
            {
                CabeceraDocumento = ConstruirCabeceraDocumento(contrato, esCanje, esConvenio),
                DetalleDocumento = new DetalleDocumentoPagoEspecieFijarPrecio
                {
                    Partes = ConstruirPartes(partes),
                    DetalleContrato = det,
                    Clausulas = ConstruirClausulasDetalle(clausulas)
                }
            };
        }

        private DocumentoPagoEspeciePrecioHecho ConstruirDocumentoPagoEspeciePrecioHecho(
            BasicoContrato contrato,
            List<ResultadoClausula> clausulas,
            List<ConfirmaParteDto> partes,
            bool esCanje,
            bool esConvenio,
            EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método ConstruirDocumentoPagoEspeciePrecioHecho()");

            decimal? precioNetoSustentable = ResolverPrecioNetoSustentable(contrato);
            string moneda = contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty;

            var det = new DetalleDocumentoPagoEspeciePrecioHechoDetalleContrato
            {
                Producto = ConstruirProducto(contrato),
                DescAdicional = new TCaption { Value = esCanje ? "INSUMO" : string.Empty },
                FechaConcertacion = new TCaption { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null },
                Cosecha = new TCodLista { CodLista = contrato.CampanaConfirma },
                UnidadMedida = new TCodCaption { CodLista = "K" },
                UnidadMedidaPrecio = new TCodCaption { CodLista = "T" },
                CantidadDesde = new TCaption { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() },
                CantidadHasta = new TCaption { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() },
                Ajuste = new TCodLista { CodLista = string.Empty },
                CantCamiones = new TCaption(),
                MontoImponible = string.Empty,
                Moneda = new TCodLista { CodLista = moneda },
                Precio = Convert.ToString(precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio),
                Calidad = ConstruirCalidad(contrato),
                MedioTransporte = new TCodCaption { CodLista = "C" },
                Entregas = ConstruirEntregas(contrato),
                Origen = ConstruirOrigen(contrato),
                Destino = ConstruirDestino(contrato),
                ProvinciaInstrumentacion = new TCodCaption { CodLista = "B" },
                ProduccionVendedor = new ProduccionVendedor { CodLista = ResolverCodListaProduccionVendedor(contrato) },
                TipoOperacion = new TCodLista { CodLista = "1" },
                DecisionPagoVoluntario = ConstruirDecisionPagoVoluntario(),
                OperacionExentaImpSantaFe = new OperacionExentaImpSantaFe(),

            };
            if (estadoSAP.NumeroSio > 0)
                det.SioGranos = ConstruirSioGranos(estadoSAP);

            if (contrato.CorredorId > 0)
                det.ComisionPorComprador = new TCaption { Value = "1" };

            if (esCanje)
            {
                det.DecisionDeclaraPrecioUnit = new DecisionDeclaraPrecioUnit { CodLista = "0" };
                det.DecisionDeclaraCantidad = new DecisionDeclaraCantidad { CodLista = "0" };
            }

            det.Insumos = ConstruirInsumosPrecioHecho(contrato);

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                det.APrecio = new TCodLista { CodLista = "1" };

            return new DocumentoPagoEspeciePrecioHecho
            {
                CabeceraDocumento = ConstruirCabeceraDocumento(contrato, esCanje, esConvenio),
                DetalleDocumento = new DetalleDocumentoPagoEspeciePrecioHecho
                {
                    Partes = ConstruirPartes(partes),
                    DetalleContrato = det,
                    Clausulas = ConstruirClausulasDetalle(clausulas)
                }
            };
        }

        private DocumentoContratoPrecioHecho ConstruirDocumentoContratoPrecioHecho(
            BasicoContrato contrato,
            List<ResultadoClausula> clausulas,
            List<ConfirmaParteDto> partes,
            bool esCanje,
            bool esConvenio,
            EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método ConstruirDocumentoContratoPrecioHecho()");

            decimal? precioNetoSustentable = ResolverPrecioNetoSustentable(contrato);

            var det = new DetalleDocumentoContratoPrecioHechoDetalleContrato
            {
                Producto = ConstruirProducto(contrato),
                DescAdicional = new TCaption { Value = esCanje ? "INSUMO" : string.Empty },
                FechaConcertacion = new TCaption { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null },
                Cosecha = new TCodLista { CodLista = contrato.CampanaConfirma },
                UnidadMedida = new TCodCaption { CodLista = "K" },
                CantidadDesde = new TCaption { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() },
                CantidadHasta = new TCaption { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() },
                Ajuste = new TCodLista { CodLista = string.Empty },
                CantCamiones = new TCaption(),
                Moneda = new TCodLista { CodLista = contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty },
                Calidad = ConstruirCalidad(contrato),
                MedioTransporte = new TCodCaption { CodLista = "C" },
                Entregas = ConstruirEntregas(contrato),
                Origen = ConstruirOrigen(contrato),
                Destino = ConstruirDestino(contrato),
                ProvinciaInstrumentacion = new TCodCaption { CodLista = "B" },
                ProduccionVendedor = new ProduccionVendedor { CodLista = ResolverCodListaProduccionVendedor(contrato) },
                TipoOperacion = new TCodLista { CodLista = "1" },
                DecisionPagoVoluntario = ConstruirDecisionPagoVoluntario(),
            };
            if (estadoSAP.NumeroSio > 0)
                det.SioGranos = ConstruirSioGranos(estadoSAP);

            if (contrato.CorredorId > 0)
                det.ComisionPorComprador = new TCaption { Value = "1" };

            if (esCanje || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                det.MontoImponible = new TCaption { Value = string.Empty };

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                det.Precio = Convert.ToString(precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio);
                det.UnidadMedidaPrecio = new TCodCaption { CodLista = "T" };
                det.APrecio = new TCodLista { CodLista = "1" };
            }

            if (!esCanje)
                det.Pagos = ConstruirPagos(contrato, esConvenio);

            return new DocumentoContratoPrecioHecho
            {
                CabeceraDocumento = ConstruirCabeceraDocumento(contrato, esCanje, esConvenio),
                DetalleDocumento = new DetalleDocumentoContratoPrecioHecho
                {
                    Partes = ConstruirPartes(partes),
                    DetalleContrato = det,
                    Clausulas = ConstruirClausulasDetalle(clausulas)
                }
            };
        }

        // ════════════════════════════════════════════════════════════════════════
        // BUILDERS COMPARTIDOS
        // ════════════════════════════════════════════════════════════════════════

        private CabeceraDocumento ConstruirCabeceraDocumento(BasicoContrato contrato, bool esCanje, bool esConvenio)
        {
            return new CabeceraDocumento
            {
                Bolsa = new TCodLista { CodLista = contrato.BolsaConfirma },
                TipoDocumento = new TCodLista { CodLista = ResolverTipoDocumento(contrato, esCanje, esConvenio) }
            };
        }
        private SioGranos ConstruirSioGranos(EstadoSAPDto estadoSAP)
        {
            SioGranos sioGranos = new SioGranos();

            if (estadoSAP.NumeroSio > 0)
            {
                sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio.ToString("D11");
            }

            return sioGranos;
        }
        private Parte[] ConstruirPartes(List<ConfirmaParteDto> partes)
        {
            return partes.Select(p => new Parte
            {
                NroContratoInterno = new TCaption { Value = p.NroContratoInterno },
                CUIT = new TCodCaption { Value = p.CUIT },
                CodLista = p.CodLista == "1" ? codigoParte.Item1 : p.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                CodListaSpecified = true
            }).ToArray();
        }

        private ConfirmaQALoteDocumentos.Clausula[] ConstruirClausulasDetalle(List<ResultadoClausula> clausulas)
        {
            return clausulas.Select(c => new ConfirmaQALoteDocumentos.Clausula
            {
                Orden = string.Empty,
                Value = c.Texto
            }).ToArray();
        }

        private Producto ConstruirProducto(BasicoContrato contrato)
        {
            return new Producto { CodLista = ResolverCodListaMaterial(contrato) };
        }

        private ConfirmaQALoteDocumentos.Calidad ConstruirCalidad(BasicoContrato contrato)
        {
            return new ConfirmaQALoteDocumentos.Calidad
            {
                CondicionesCalidad = new TCodLista { CodLista = ResolverCodListaCalidad(contrato) },
                OtrasCondicionesCalidad = new TCaption { Value = string.Empty }
            };
        }

        private Entrega ConstruirEntregas(BasicoContrato contrato)
        {
            return new Entrega
            {
                EntregaDesde = new TCaption { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
        }

        private Origen ConstruirOrigen(BasicoContrato contrato)
        {
            return new Origen
            {
                LocalidadOrigen = new OrigenLocalidadOrigen { Value = contrato.LocalidadConfirma, LocalidadText = "" },
                ProvinciaOrigen = new TCodCaption { CodLista = contrato.ProvinciaConfirma }
            };
        }

        private TCodCaption ConstruirDestino(BasicoContrato contrato)
        {
            return new TCodCaption { CodLista = contrato.DestinoConfirma };
        }

        private Fijacion ConstruirFijacion(BasicoContrato contrato, CondicionFijacionEstadoBoletoDto condiciones)
        {
            bool noTieneCondiciones = contrato.EsFason == true || contrato.PrestamoDevolucion == true;

            var fij = new Fijacion
            {
                FijMinima = new TCaption { Value = noTieneCondiciones ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                FijMaxima = new TCaption { Value = noTieneCondiciones ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                UnidadMedidaFijacion = new TCodCaption { Caption = "K", CodLista = "K" },
                FijPeriodo = new TCodCaption { CodLista = "1", Value = "1" },
                FijFecDesde = new TCaption { Value = noTieneCondiciones ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                FijFecHasta = new TCaption { Value = noTieneCondiciones ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                PorcMultaIncumplimiento = new TCaption { Value = "010" },
                ComunicacionFijacion = new TCodCaption { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" }
            };

            if (contrato.Pizarra == true)
                fij.PizarraFijacion = new TCodCaption { CodLista = "1" };

            return fij;

        }

        private Pagos ConstruirPagos(BasicoContrato contrato, bool esConvenio)
        {
            return new Pagos
            {
                ProvinciaPago = new TCodCaption { CodLista = "B" },
                FechaCondicionPago = new TCaption { Value = ResolverCondicionPago(contrato, esConvenio) },
                LugarPago = new TCaption { Value = "BUENOS AIRES" },
                PagoAOrdenDe = new PagoAOrdenDe { CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1" },
                PorcPago = new TCaption { Value = contrato.PorcentajeDePago.Value.ToString() }
            };
        }

        private DetalleDocumentoPagoEspecieFijarPrecioDetalleContratoInsumos ConstruirInsumosFijarPrecio(BasicoContrato contrato)
        {
            return new DetalleDocumentoPagoEspecieFijarPrecioDetalleContratoInsumos
            {
                Productos = new[] { ConstruirInsumo() },
                Moneda = new TCodLista { CodLista = string.Empty },
                PrecioTotal = new TCaption { Value = contrato.Monto.ToString() },
                Factura = new TCaption(),
                PorcentajeGastos = new TCaption(),
                TipoCambioPesos = new TCaption(),
                LugarEntrega = new TCaption(),
                ProvinciaEntrega = new TCodCaption { Value = contrato.ProvinciaConfirma }
            };
        }

        private DetalleDocumentoPagoEspeciePrecioHechoDetalleContratoInsumos ConstruirInsumosPrecioHecho(BasicoContrato contrato)
        {
            return new DetalleDocumentoPagoEspeciePrecioHechoDetalleContratoInsumos
            {
                Productos = new[] { ConstruirInsumo() },
                Moneda = new TCodLista { CodLista = string.Empty },
                PrecioTotal = new TCaption { Value = contrato.Monto.ToString() },
                Factura = new TCaption(),
                PorcentajeGastos = new TCaption(),
                TipoCambioPesos = new TCaption(),
                LugarEntrega = new TCaption(),
                ProvinciaEntrega = new TCodCaption { Value = contrato.ProvinciaConfirma }
            };
        }

        private Insumo ConstruirInsumo()
        {
            return new Insumo
            {
                Producto = new Producto { CodLista = "1" },
                DescAdicional = new TCaption { Value = "insumos" },
                Cantidad = new TCaption(),
                Precio = new TCaption(),
                UnidadMedida = new TCodCaption { CodLista = string.Empty },
                UnidadMedidaPrecio = new TCodCaption { CodLista = string.Empty }
            };
        }

        private DecisionPagoVoluntario ConstruirDecisionPagoVoluntario()
        {
            return new DecisionPagoVoluntario { CodLista = "2", FondoFederalText = "" };
        }

        private decimal? ResolverPrecioNetoSustentable(BasicoContrato contrato)
        {
            if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) &&
                contrato.SustentableTipoDBId.HasValue &&
                contrato.TipoNegocioId == 2 &&
                contrato.SustentableTipoDBId == 1)
                return PrecioNetoSustentableSobrePrecio(contrato, null);
            return null;
        }

        // ════════════════════════════════════════════════════════════════════════
        // SERVICIO CONFIRMA
        // ════════════════════════════════════════════════════════════════════════

        private altaLoteResult LlamarServicioConfirma(Lote lote)
        {
            using (var agent = new LoteDocumentosServiceClient("Default"))
            {
                try
                {
                    agent.ClientCredentials.UserName.UserName = userConfirma;
                    agent.ClientCredentials.UserName.Password = passConfirma;
                    return agent.AltaDefinitiva(lote);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error al llamar al servicio AltaDefinitiva de Confirma");
                    if (agent.State == System.ServiceModel.CommunicationState.Faulted)
                        agent.Abort();
                    throw;
                }
            }
        }

        private string LimpiarTexto(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            return texto
                .Replace("“", "\"")
                .Replace("”", "\"")
                .Replace("’", "'")
                .Replace("\u00A0", " ")
                .Trim();
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
        // RESULTADO DE PRUEBA
        // ════════════════════════════════════════════════════════════════════════

        private static ConfirmaAltaLoteResultDto DevolverResultadoPrueba()
        {
            return new ConfirmaAltaLoteResultDto
            {
                altaIdLote = "18443050",
                altaEstado = 1,
                altaEstadoLote = 4,
                altaItem = new List<altaItemDto>
                {
                    new altaItemDto
                    {
                        altaIdDocumento              = "",
                        altaEstadoDocumento          = 6,
                        altaErrores                  = new List<string> { "Documento existente" },
                        altaIdDocumentoExistente     = "18390504",
                        altaIdDocumentoExistenteLote = "25007245",
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
        // RESULTADO ALTA DEFINITIVA
        // ════════════════════════════════════════════════════════════════════════

        public ConfirmaAltaLoteResultDto ResultadoAltaDefinitiva(
            altaLoteResult devolucion,
            EstadosConfirmaDto estadosConfirmaDto)
        {
            var result = new ConfirmaAltaLoteResultDto
            {
                altaIdLote = devolucion.altaIdLote,
                altaEstado = SoloDigitos(devolucion.altaEstado.ToString()),
                altaEstadoSpecified = devolucion.altaEstadoSpecified,
                altaEstadoDetalleError = devolucion.altaEstadoDetalleError,
                altaEstadoLote = SoloDigitos(devolucion.altaEstadoLote.ToString()),
                altaEstadoLoteSpecified = devolucion.altaEstadoLoteSpecified,
                altaItem = new List<altaItemDto>()
            };

            result.confirmaAltaEstado = estadosConfirmaDto.ConfirmaAltaEstadoDto
                .Find(x => x.CodigoConfirmaAltaEstado == result.altaEstado);
            result.confirmaAltaEstadoLote = estadosConfirmaDto.ConfirmaAltaEstadoLoteDto
                .Find(x => x.CodigoConfirmaAltaEstadoLote == result.altaEstadoLote);

            if (devolucion.altaItem == null) return result;

            foreach (var item in devolucion.altaItem)
            {
                var altaItem = new altaItemDto
                {
                    altaIdDocumento = item.altaIdDocumento,
                    altaEstadoDocumento = SoloDigitos(item.altaEstadoDocumento.ToString()),
                    altaErrores = item.altaErrores != null ? new List<string>(item.altaErrores) : new List<string>(),
                    altaIdDocumentoExistenteLote = item.altaIdDocumentoExistenteLote,
                    altaIdDocumentoExistente = item.altaIdDocumentoExistente,
                    codigo = item.codigo
                };

                altaItem.confirmaAltaEstadoDocumento = estadosConfirmaDto.ConfirmaAltaEstadoDocumentoDto
                    .Find(x => x.CodigoConfirmaAltaEstadoDocumento == altaItem.altaEstadoDocumento);

                result.altaItem.Add(altaItem);
            }

            return result;
        }

        // ════════════════════════════════════════════════════════════════════════
        // HELPERS GENERALES
        // ════════════════════════════════════════════════════════════════════════

        public string CorregirFormatoFecha(string cadena)
        {
            return DateTime.Parse(cadena).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private static int SoloDigitos(string valor)
        {
            return int.Parse(new string(valor.Where(char.IsDigit).ToArray()));
        }

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
    }
}
