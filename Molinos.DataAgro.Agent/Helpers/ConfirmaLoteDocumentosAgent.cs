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
using System.Text;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConfirmaLoteDocumentosAgent : IConfirmaLoteDocumentosAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IStatusContratoAgent status;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;
        private readonly string userConfirma = ConfigurationManager.AppSettings["ConfirmaUser"];
        private readonly string passConfirma = ConfigurationManager.AppSettings["ConfirmaPass"];
        private readonly string ambientePruebas = ConfigurationManager.AppSettings["AmbientePruebas"];
        private readonly string ambienteLocal = ConfigurationManager.AppSettings["AmbienteLocal"];
        private readonly string cuitMOA = ConfigurationManager.AppSettings["Cuit"];
        private readonly string cuit1 = "30646328450";
        private readonly string cuit2 = "30500120882";

        public ConfirmaLoteDocumentosAgent(ILogger logger, IRepositorio repositorio, IStatusContratoAgent status, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
        }

        public ConfirmaAltaLoteResultDto ConfirmaLoteDocumentos(List<ResultadoClausula> clausulas, List<int> equipo, BasicoContrato contrato, EstadosConfirmaDto estadosConfirmaDto)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaConfirma"] == "1")
            {
                ConfirmaAltaLoteResultDto confirmaAltaLoteResult = new ConfirmaAltaLoteResultDto()
                {
                    altaIdLote = "18443050",
                    altaEstado = 1,
                    altaEstadoLote = 4,
                    altaItem = new List<altaItemDto>(){
                        new altaItemDto()
                        {
                            altaIdDocumento = "",
                            altaEstadoDocumento = 6,
                            altaErrores = new List<string>(){ "Documento existente" },
                            altaIdDocumentoExistente = "18390504",
                            altaIdDocumentoExistenteLote = "25007245",
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

                    logger.Info($"Datos del Negocio de Confirma Precargados; BolsaConfirma:{contrato.BolsaConfirma}, CorredorId:{contrato.CorredorId}, TipoNegocioId:{contrato.TipoNegocioId}, MaterialId:{contrato.MaterialId}, " +
                        $"FechaOperacion:{contrato.FechaOperacion}, CampañaConfirma: {contrato.CampanaConfirma}, KgMaximo:{contrato.KgMaximo}, KgMinimo:{contrato.KgMinimo}, Cantidad:{contrato.Cantidad}, " +
                        $"CantidadCamiones:{contrato.CantidadCamiones}, Moneda:{contrato.Moneda}, Precio:{contrato.Precio}, PorcentajeComision:{contrato.PorcentajeComision}, StandardDeCalidadId:{contrato.StandardDeCalidadId}, " +
                        $"FechaDesde:{contrato.FechaDesde}, FechaHasta:{contrato.FechaHasta}, LocalidadConfirma:{contrato.LocalidadConfirma}, ProvinciaConfirma:{contrato.ProvinciaConfirma}, DestinoConfirma:{contrato.DestinoConfirma}, " +
                        $"CD:{contrato.CD}, Warrant:{contrato.Warrant}, PagoDiferido:{contrato.PagoDiferido}, PagoDirectoVendedor:{contrato.PagoDirectoVendedor}, PorcentajeDePago:{contrato.PorcentajeDePago}, Monto:{contrato.Monto}, " +
                        $"Pizarra:{contrato.Pizarra}, ClasificacionId:{contrato.ClasificacionId}");

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
                    string nroContratoInterno = numeroSAP.TrimStart('0');

                    //bool existeConfirma = repositorio.Existe<Confirma>(x => contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? ((x.Negocio as FijacionDePrecioContrato).FijacionSAP == contrato.ContratoSAP) : x.Negocio.ContratoSAP == contrato.ContratoSAP);
                    //if (existeConfirma is false) throw new ArgumentNullException("Confirma", "No existe el confirma");
                    //logger.Info($"Se Consulta el status del Negocio SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");

                    List<ConfirmaParteDto> Partes = new List<ConfirmaParteDto> {
                        new ConfirmaParteDto { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = ambienteLocal == "1" ? cuit1 : contrato.Cuit, Sucursal = string.Empty },
                        new ConfirmaParteDto { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = cuitMOA, Sucursal = string.Empty }
                    };
                    if (contrato.CorredorId > 0)
                        Partes.Add(new ConfirmaParteDto { CodLista = "2", NroContratoInterno = nroContratoInterno, CUIT = ambienteLocal == "1" ? cuit2 : contrato.CUITCorredor, Sucursal = string.Empty });

                    logger.Info($"Datos Precalculados del Negocio de Confirma; Codigo:{numeroSAP}, esCanje:{esCanje}, esConvenio:{esConvenio}, Partes: {string.Join(" - ", Partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");
                    if (clausulas is null || clausulas.Count == 0) throw new ArgumentNullException("Clausulas", $"WS Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {numeroSAP}.");

                    Lote lote = new Lote();
                    #region Lote
                    lote.EmpresaPresentante = cuitMOA;

                    List<Item> items = new List<Item>();
                    #region Item[]
                    ItemItemInfo itemInfo = new ItemItemInfo() { Workflow = contrato.CorredorId > 0 ? ItemItemInfoWorkflow.Item1 : ItemItemInfoWorkflow.Item7 };

                    object item1 = null;
                    if ((contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !esCanje)
                        item1 = DevolverItemDocumentoFijarPrecio(contrato, clausulas, Partes, esCanje, esConvenio, condiciones, estadoSAP);
                    else if ((contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && esCanje)
                        /* tiene Insumos */
                        item1 = DevolverItemDocumentoPagoEspecieFijarPrecio(contrato, clausulas, Partes, esCanje, esConvenio, condiciones, estadoSAP);
                    else if (!(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && esCanje)
                        /* tiene Insumos */
                        item1 = DevolverItemDocumentoPagoEspeciePrecioHecho(contrato, clausulas, Partes, esCanje, esConvenio, condiciones, estadoSAP);
                    else if (!(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !esCanje)
                        item1 = DevolverItemDocumentoContratoPrecioHecho(contrato, clausulas, Partes, esCanje, esConvenio, condiciones, estadoSAP);

                    items.Add(new Item
                    {
                        itemInfo = itemInfo,
                        Item1 = item1,
                        codigo = nroContratoInterno,
                    });
                    #endregion Item[]

                    lote.Items = items.ToArray();
                    #endregion Lote

                    logger.Debug(lote.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = lote.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    //altaLoteResult devolucion = agent.AltaDefinitiva(lote);
                    altaLoteResult devolucion;
                    using (LoteDocumentosServiceClient agent = new LoteDocumentosServiceClient("Default"))
                    {
                        try
                        {
                            agent.ClientCredentials.UserName.UserName = userConfirma;
                            agent.ClientCredentials.UserName.Password = passConfirma;

                            devolucion = agent.AltaDefinitiva(lote);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex, "Error al llamar al servicio AltaDefinitiva de Confirma");
                            // Si el canal está en estado Faulted, hay que abortarlo
                            if (agent.State == System.ServiceModel.CommunicationState.Faulted)
                            {
                                agent.Abort();
                            }
                            throw;
                        }
                    }

                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    ConfirmaAltaLoteResultDto confirmaAltaLoteResult = ResultadoAltaDefinitiva(devolucion, estadosConfirmaDto);

                    return confirmaAltaLoteResult;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error: No se pudo procesar XML en WS Confirma");
                    throw;
                }
            }
        }

        // EN USO
        DocumentoFijarPrecio DevolverItemDocumentoFijarPrecio(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método DevolverItemDocumentoFijarPrecio()");
            DocumentoFijarPrecio item1 = new DocumentoFijarPrecio();
            #region DocumentoFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    //CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                    CodLista = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : esCanje ? "17" : "",
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoFijarPrecio detalleDocumento = new DetalleDocumentoFijarPrecio();
            #region DetalleDocumentoFijarPrecio
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoFijarPrecioDetalleContrato detalleContrato = new DetalleDocumentoFijarPrecioDetalleContrato();
            #region DetalleDocumentoFijarPrecioDetalleContrato

            detalleContrato.Producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
            };

            detalleContrato.DescAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.FechaConcertacion = new TCaption() { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null, };

            //// GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            //string monedaAFijar = "";
            //if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            //{
            //    monedaAFijar = contrato.Moneda;
            //}
            //else
            //{
            //    string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
            //    x.ContratoSAP == contrato.ContratoSAP &&
            //    x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
            //    x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
            //    x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
            //    monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            //}

            //TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };

            // GSIAN: Comentado por el caso de QA 2687753
            //detalleContrato.UnidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada

            detalleContrato.Cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.UnidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.CantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.Ajuste = new TCodLista() { CodLista = string.Empty };

            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            detalleContrato.CantCamiones = new TCaption();

            detalleContrato.MontoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.Moneda = new TCodLista() { CodLista = "2" }; // Para los A FIJAR le pasamos USD, como hace SAP.;

            // GSIAN: Comentado por el caso de QA 2687753
            if (contrato.CorredorId > 0 && contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision.Value > 0)
            {
                TCaption comisionPorComprador = new TCaption();
                comisionPorComprador.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
                detalleContrato.ComisionPorComprador = comisionPorComprador;
            }

            detalleContrato.Calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };

            detalleContrato.MedioTransporte = new TCodCaption() { CodLista = "C" }; // Camión

            detalleContrato.Entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };

            detalleContrato.Origen = new Origen()
            {
                LocalidadOrigen = new OrigenLocalidadOrigen() { Value = contrato.LocalidadConfirma, LocalidadText = "" },
                ProvinciaOrigen = new TCodCaption() { CodLista = contrato.ProvinciaConfirma },
            };

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            detalleContrato.ProvinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ? ("4 días hábiles de fecha de fijación"):
                (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (contrato.CD == true ? "Pago Anticipado" :
                (contrato.Warrant == true ? "Pago contra Warrant" :
                (contrato.PagoDiferido == true ? ("Días de diferimiento contra mercadería entregada") :
                ("72 hs contra mercadería descargada.")))) : null);

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = new TCaption() { Value = "BUENOS AIRES" };
                pagos.PagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };
                pagos.PorcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };
                pagos.ProvinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;
                detalleContrato.Fijacion = new Fijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { CodLista = "K", Caption = "K" },
                    FijPeriodo = new TCodCaption() { CodLista = "1", Value = "1" },
                    FijFecDesde = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                    //PizarraFijacion = new TCodCaption() { CodLista = contrato.Pizarra == true ? "1" : "" },
                };

                if (contrato.Pizarra == true)
                    detalleContrato.Fijacion.PizarraFijacion = new TCodCaption() { CodLista = "1" };
            }

            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            detalleContrato.ProduccionVendedor = new DetalleDocumentoFijarPrecioDetalleContratoProduccionVendedor() { CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2") };

            detalleContrato.TipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal;

            //GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado 
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;


            //OperacionExentaImpSantaFe operacionExentaImpSantaFe = new OperacionExentaImpSantaFe();
            //operacionExentaImpSantaFe.CodLista = "";
            ////operacionExentaImpSantaFe.Caption = "";
            //operacionExentaImpSantaFe.OperacionExentaImpSantaFeText = "";
            ////operacionExentaImpSantaFe.Value = "";

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoFijarPrecioDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = (item.Texto);

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoFijarPrecio

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoFijarPrecio

            return item1;
        }

        // EN USO - tiene INSUMOS
        DocumentoPagoEspecieFijarPrecio DevolverItemDocumentoPagoEspecieFijarPrecio(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método DevolverItemDocumentoPagoEspecieFijarPrecio()");
            DocumentoPagoEspecieFijarPrecio item1 = new DocumentoPagoEspecieFijarPrecio();
            #region DocumentoPagoEspecieFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    //CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                    CodLista = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : esCanje ? "17" : "",
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoPagoEspecieFijarPrecio detalleDocumento = new DetalleDocumentoPagoEspecieFijarPrecio();
            #region DetalleDocumentoPagoEspecieFijarPrecio
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoPagoEspecieFijarPrecioDetalleContrato detalleContrato = new DetalleDocumentoPagoEspecieFijarPrecioDetalleContrato();
            #region DetalleDocumentoPagoEspecieFijarPrecioDetalleContrato

            detalleContrato.Producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
            };

            detalleContrato.DescAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.FechaConcertacion = new TCaption() { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null, };
            detalleContrato.Cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.UnidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.CantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.Ajuste = new TCodLista() { CodLista = string.Empty };

            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            detalleContrato.CantCamiones = new TCaption();

            detalleContrato.MontoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.Moneda = new TCodLista() { CodLista = "2" }; // Para los A FIJAR le pasamos USD, como hace SAP.

            if (contrato.CorredorId > 0 && contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision.Value > 0)
            {
                TCaption comisionPorComprador = new TCaption();
                comisionPorComprador.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
                detalleContrato.ComisionPorComprador = comisionPorComprador;
            }

            detalleContrato.Calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };

            detalleContrato.MedioTransporte = new TCodCaption() { CodLista = "C" }; // Camión

            detalleContrato.Entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };

            detalleContrato.Origen = new Origen()
            {
                LocalidadOrigen = new OrigenLocalidadOrigen() { LocalidadText = "", Value = contrato.LocalidadConfirma },
                ProvinciaOrigen = new TCodCaption() { CodLista = contrato.ProvinciaConfirma },
            };

            detalleContrato.Destino = new TCodCaption()
            {
                CodLista = contrato.DestinoConfirma,
                //CodPrv = "0000", // no está en Staging?
            };

            detalleContrato.ProvinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            //detalleContrato.UnidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada

            if (esCanje)
            {
                detalleContrato.DecisionDeclaraPrecioUnit = new DecisionDeclaraPrecioUnit() { CodLista = "0" };
                detalleContrato.DecisionDeclaraCantidad = new DecisionDeclaraCantidad() { CodLista = "0" };
            }

            #region Insumos
            DetalleDocumentoPagoEspecieFijarPrecioDetalleContratoInsumos insumos = new DetalleDocumentoPagoEspecieFijarPrecioDetalleContratoInsumos();
            List<Insumo> productos = new List<Insumo>();
            Insumo insumo = new Insumo()
            {
                Producto = new Producto { CodLista = "1" },
                DescAdicional = new TCaption { Value = "insumos" },
                Cantidad = new TCaption(),
                Precio = new TCaption(),
                UnidadMedida = new TCodCaption { CodLista = string.Empty },
                UnidadMedidaPrecio = new TCodCaption { CodLista = string.Empty },
                //PrecioTotal = new TCaption { Caption = "", Value = "" },
            };
            productos.Add(insumo);
            insumos.Productos = productos.ToArray();
            insumos.Moneda = new TCodLista { CodLista = string.Empty };
            insumos.PrecioTotal = new TCaption { Value = contrato.Monto.ToString() };
            insumos.Factura = new TCaption();
            insumos.PorcentajeGastos = new TCaption();
            insumos.TipoCambioPesos = new TCaption();
            insumos.LugarEntrega = new TCaption();
            insumos.ProvinciaEntrega = new TCodCaption { Value = contrato.ProvinciaConfirma };
            detalleContrato.Insumos = insumos;
            #endregion Insumos

            #region Fijacion
            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;

                detalleContrato.Fijacion = new Fijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { Caption = "K", CodLista = "K" },
                    FijPeriodo = new TCodCaption() { /*CodLista = "1",*/ Value = "1" },
                    FijFecDesde = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                    //PizarraFijacion = new TCodCaption() { CodLista = contrato.Pizarra == true ? "1" : "" },
                };

                if (contrato.Pizarra == true)
                    detalleContrato.Fijacion.PizarraFijacion = new TCodCaption() { CodLista = "1" };
            }
            #endregion Fijacion

            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            detalleContrato.ProduccionVendedor = new ProduccionVendedor() { CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2") };

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            detalleContrato.DecisionPagoVoluntario = new DecisionPagoVoluntario()
            {
                CodLista = "2",
                FondoFederalText = "",
            };

            detalleContrato.TipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal

            // GSIAN: Cómo se completa??
            //detalleContrato.OperacionExentaImpSantaFe = new OperacionExentaImpSantaFe();

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoPagoEspecieFijarPrecioDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = (item.Texto);

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoPagoEspecieFijarPrecio

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoPagoEspecieFijarPrecio

            return item1;
        }

        // EN USO - tiene INSUMOS
        DocumentoPagoEspeciePrecioHecho DevolverItemDocumentoPagoEspeciePrecioHecho(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método DevolverItemDocumentoPagoEspeciePrecioHecho()");
            DocumentoPagoEspeciePrecioHecho item1 = new DocumentoPagoEspeciePrecioHecho();
            #region DocumentoPagoEspecieFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    //CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                    CodLista = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : esCanje ? "17" : "",
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoPagoEspeciePrecioHecho detalleDocumento = new DetalleDocumentoPagoEspeciePrecioHecho();
            #region DetalleDocumentoPagoEspeciePrecioHecho
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoPagoEspeciePrecioHechoDetalleContrato detalleContrato = new DetalleDocumentoPagoEspeciePrecioHechoDetalleContrato();
            #region DetalleDocumentoPagoEspeciePrecioHechoDetalleContrato

            detalleContrato.Producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
            };

            detalleContrato.FechaConcertacion = new TCaption()
            {
                Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null,
            };

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            //else
            //{
            //    string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
            //    x.ContratoSAP == contrato.ContratoSAP &&
            //    x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
            //    x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
            //    x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
            //    monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            //}

            detalleContrato.Moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.DescAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.UnidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.CantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.Cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Ajuste = new TCodLista() { CodLista = string.Empty };

            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            detalleContrato.CantCamiones = new TCaption();

            if (contrato.CorredorId > 0 && contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision.Value > 0)
            {
                TCaption comisionPorComprador = new TCaption();
                comisionPorComprador.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
                detalleContrato.ComisionPorComprador = comisionPorComprador;
            }

            decimal? precioNetoSustentable = null;
            if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId.HasValue)
            {
                if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                {
                    precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                }
            }

            // GSIAN: Cómo se completa??
            //detalleContrato.Precio = "";
            var precioContrato = Convert.ToString((contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio);
            detalleContrato.Precio = precioContrato;
            detalleContrato.MontoImponible = "";

            detalleContrato.Calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };

            detalleContrato.MedioTransporte = new TCodCaption() { CodLista = "C" }; // Camión

            detalleContrato.Entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };

            detalleContrato.Origen = new Origen()
            {
                LocalidadOrigen = new OrigenLocalidadOrigen() { LocalidadText = "", Value = contrato.LocalidadConfirma },
                ProvinciaOrigen = new TCodCaption() { CodLista = contrato.ProvinciaConfirma },
            };

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            if (esCanje)
            {
                detalleContrato.DecisionDeclaraPrecioUnit = new DecisionDeclaraPrecioUnit() { CodLista = "0" };
                detalleContrato.DecisionDeclaraCantidad = new DecisionDeclaraCantidad() { CodLista = "0" };
            }

            detalleContrato.ProvinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

            #region Insumos
            DetalleDocumentoPagoEspeciePrecioHechoDetalleContratoInsumos insumos = new DetalleDocumentoPagoEspeciePrecioHechoDetalleContratoInsumos();
            List<Insumo> productos = new List<Insumo>();
            Insumo insumo = new Insumo()
            {
                Producto = new Producto { CodLista = "1" },
                DescAdicional = new TCaption { Value = "insumos" },
                Cantidad = new TCaption(),
                Precio = new TCaption(),
                UnidadMedida = new TCodCaption { CodLista = string.Empty },
                UnidadMedidaPrecio = new TCodCaption { CodLista = string.Empty },
                //PrecioTotal = new TCaption { Caption = "", Value = "" },
            };
            productos.Add(insumo);
            insumos.Productos = productos.ToArray();
            insumos.Moneda = new TCodLista { CodLista = string.Empty };
            insumos.PrecioTotal = new TCaption { Value = contrato.Monto.ToString() };
            insumos.Factura = new TCaption();
            insumos.PorcentajeGastos = new TCaption();
            insumos.TipoCambioPesos = new TCaption();
            insumos.LugarEntrega = new TCaption();
            insumos.ProvinciaEntrega = new TCodCaption { Value = contrato.ProvinciaConfirma };
            detalleContrato.Insumos = insumos;
            #endregion Insumos

            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            detalleContrato.ProduccionVendedor = new ProduccionVendedor() { CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2") };

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            detalleContrato.TipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                detalleContrato.APrecio = new TCodLista() { CodLista = "1" };

            // GSIAN: Cómo se completa??
            detalleContrato.OperacionExentaImpSantaFe = new OperacionExentaImpSantaFe();

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoPagoEspeciePrecioHechoDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = (item.Texto);

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoPagoEspeciePrecioHecho

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoPagoEspeciePrecioHecho

            return item1;
        }

        // EN USO
        DocumentoContratoPrecioHecho DevolverItemDocumentoContratoPrecioHecho(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            logger.Info("WS Confirma - Método DevolverItemDocumentoContratoPrecioHecho()");
            DocumentoContratoPrecioHecho item1 = new DocumentoContratoPrecioHecho();
            #region DocumentoConsignacionPrecioHecho
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    //CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                    CodLista = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : esCanje ? "17" : "",
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoContratoPrecioHecho detalleDocumento = new DetalleDocumentoContratoPrecioHecho();
            #region DetalleDocumentoContratoPrecioHecho
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoContratoPrecioHechoDetalleContrato detalleContrato = new DetalleDocumentoContratoPrecioHechoDetalleContrato();

            decimal? precioNetoSustentable = null;
            if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId.HasValue)
            {
                if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                {
                    precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                }
            }


            #region DetalleDocumentoContratoPrecioHechoDetalleContrato
            detalleContrato.Producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
            };

            detalleContrato.FechaConcertacion = new TCaption() { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null };
            detalleContrato.Moneda = new TCodLista() { CodLista = contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty };

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                //detalleContrato.Precio = contrato.Precio.ToString();
                var precioContrato = Convert.ToString((contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio);
                detalleContrato.Precio = precioContrato;
                detalleContrato.UnidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            }

            if (esCanje || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                detalleContrato.MontoImponible = new TCaption() { Value = "" };

            detalleContrato.DescAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.UnidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.CantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.Cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Ajuste = new TCodLista() { CodLista = string.Empty };

            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            detalleContrato.CantCamiones = new TCaption();
            if (contrato.CorredorId > 0 && contrato.PorcentajeComision.HasValue && contrato.PorcentajeComision.Value > 0)
            {
                detalleContrato.ComisionPorComprador = new TCaption() { Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null };
            }

            detalleContrato.Calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };

            detalleContrato.MedioTransporte = new TCodCaption() { CodLista = "C" }; // Camión

            detalleContrato.Entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };

            detalleContrato.Origen = new Origen()
            {
                LocalidadOrigen = new OrigenLocalidadOrigen() { Value = contrato.LocalidadConfirma, LocalidadText = "" },
                ProvinciaOrigen = new TCodCaption() { CodLista = contrato.ProvinciaConfirma },
            };

            detalleContrato.Destino = new TCodCaption()
            {
                CodLista = contrato.DestinoConfirma,
                //CodPrv = "0000", // no está en Staging?
            };

            detalleContrato.ProvinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ? ("4 días hábiles de fecha de fijación") :
                (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (contrato.CD == true ? "Pago Anticipado" :
                (contrato.Warrant == true ? "Pago contra Warrant" :
                (contrato.PagoDiferido == true ? ("Días de diferimiento contra mercadería entregada") :
                ("72 hs contra mercadería descargada.")))) : null);

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = new TCaption() { Value = "BUENOS AIRES" };
                pagos.PagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };
                pagos.PorcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };
                pagos.ProvinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            detalleContrato.ProduccionVendedor = new ProduccionVendedor() { CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2") };

            // GSIAN: qué va acá?
            //detalleContrato.DecisionPagoVoluntario = new DecisionPagoVoluntario();
            //GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado 
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            detalleContrato.TipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                detalleContrato.APrecio = new TCodLista() { CodLista = "1" };

            //detalleContrato.SioGranos = new SioGranos()
            //{
            //    NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null,
            //};

            // GSIAN: qué va acá?
            //detalleContrato.OperacionExentaImpSantaFe = new OperacionExentaImpSantaFe();

            #endregion DetalleDocumentoContratoPrecioHechoDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = (item.Texto);

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoContratoPrecioHecho

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoConsignacionPrecioHecho

            return item1;
        }

        #region Metodos no utilizados
        // NO SE USA
        DocumentoFijarPrecioRofex DevolverItemDocumentoFijarPrecioRofex(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoFijarPrecioRofex item1 = new DocumentoFijarPrecioRofex();
            #region DocumentoFijarPrecioRofex
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoFijarPrecioRofex detalleDocumento = new DetalleDocumentoFijarPrecioRofex();
            #region DetalleDocumentoFijarPrecioRofex
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoFijarPrecioRofexDetalleContrato detalleContrato = new DetalleDocumentoFijarPrecioRofexDetalleContrato();
            #region DetalleDocumentoFijarPrecioRofexDetalleContrato
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;
            TCaption fechaConcertacion = new TCaption()
            {
                Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null,
            };
            detalleContrato.FechaConcertacion = fechaConcertacion;

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCaption montoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.MontoImponible = montoImponible;

            TCodCaption unidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedidaPrecio = unidadMedidaPrecio;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            TCaption comisionPorComprador = new TCaption();
            comisionPorComprador.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
            detalleContrato.ComisionPorComprador = comisionPorComprador;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                #region Fijacion
                // GSIAN: Cómo se completa?
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;
                DetalleDocumentoFijarPrecioRofexDetalleContratoFijacion fijacion = new DetalleDocumentoFijarPrecioRofexDetalleContratoFijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { CodLista = "K" },
                    FijPeriodo = new TCodCaption() { CodLista = "1" },
                    FijFecDesde1 = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta1 = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    FechaPosicionRofex1 = new TFecha() { Anio = "", Mes = "", Value = "" }, // ????
                    IndiceRofex1 = new DetalleDocumentoFijarPrecioRofexDetalleContratoFijacionIndiceRofex1() { Codigo = "", Value = "" }, // ???
                    FijFecDesde2 = new TCaption(), // ???
                    FijFecHasta2 = new TCaption(), // ???
                    FechaPosicionRofex2 = new TFecha() { Anio = "", Mes = "", Value = "" }, // ????
                    IndiceRofex2 = new DetalleDocumentoFijarPrecioRofexDetalleContratoFijacionIndiceRofex2() { Codigo = "", Value = "" }, // ???
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                    //PizarraFijacion = new TCodCaption() { CodLista = contrato.Pizarra == true ? "1" : "" },
                };

                if (contrato.Pizarra == true)
                    fijacion.PizarraFijacion = new TCodCaption() { CodLista = "1" };
                #endregion Fijacion
                detalleContrato.Fijacion = fijacion;
            }

            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            detalleContrato.ProduccionVendedor = new ProduccionVendedor() { CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2") };

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            // GSIAN: Qué va acá?
            OperacionExentaImpSantaFe operacionExentaImpSantaFe = new OperacionExentaImpSantaFe();
            detalleContrato.OperacionExentaImpSantaFe = operacionExentaImpSantaFe;

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoFijarPrecioRofexDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoFijarPrecioRofex

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoFijarPrecioRofex

            return item1;
        }
        // NO SE USA
        DocumentoConsignacionFijarPrecio DevolverItemDocumentoConsignacionFijarPrecio(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoConsignacionFijarPrecio item1 = new DocumentoConsignacionFijarPrecio();
            #region DocumentoConsignacionFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoConsignacionFijarPrecio detalleDocumento = new DetalleDocumentoConsignacionFijarPrecio();
            #region DetalleDocumentoConsignacionFijarPrecio
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoConsignacionFijarPrecioDetalleContrato detalleContrato = new DetalleDocumentoConsignacionFijarPrecioDetalleContrato();
            #region DetalleDocumentoConsignacionFijarPrecioDetalleContrato
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;

            TCaption fechaConcertacion = new TCaption() { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null };
            detalleContrato.FechaConcertacion = fechaConcertacion;

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCaption montoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.MontoImponible = montoImponible;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            // GSIAN: Que va acá? En otros está "ComisionPorComprador"
            TCaption comisionPorConsignatario = new TCaption();
            detalleContrato.ComisionPorConsignatario = comisionPorConsignatario;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;
                #region Fijacion
                Fijacion fijacion = new Fijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { CodLista = "K" },
                    FijPeriodo = new TCodCaption() { CodLista = "1" },
                    FijFecDesde = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                    //PizarraFijacion = new TCodCaption() { CodLista = contrato.Pizarra == true ? "1" : "" },
                };

                if (contrato.Pizarra == true)
                    fijacion.PizarraFijacion = new TCodCaption() { CodLista = "1" };
                #endregion Fijacion
                detalleContrato.Fijacion = fijacion;
            }

            // GSIAN: qué va acá?
            DetalleDocumentoConsignacionFijarPrecioDetalleContratoProduccionComitente produccionComitente = new DetalleDocumentoConsignacionFijarPrecioDetalleContratoProduccionComitente();
            detalleContrato.ProduccionComitente = produccionComitente;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;

            // GSIAN: qué va acá?
            OperacionExentaImpSantaFe operacionExentaImpSantaFe = new OperacionExentaImpSantaFe();
            detalleContrato.OperacionExentaImpSantaFe = operacionExentaImpSantaFe;

            #endregion DetalleDocumentoConsignacionFijarPrecioDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoConsignacionFijarPrecio

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoConsignacionFijarPrecio

            return item1;
        }
        // NO SE USA
        DocumentoConsignacionPrecioHecho DevolverItemDocumentoConsignacionPrecioHecho(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoConsignacionPrecioHecho item1 = new DocumentoConsignacionPrecioHecho();

            #region DocumentoConsignacionPrecioHecho
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoConsignacionPrecioHecho detalleDocumento = new DetalleDocumentoConsignacionPrecioHecho();
            #region DetalleDocumentoConsignacionPrecioHecho
            List<Parte> partes_detalle = new List<Parte>();
            #region Parte
            foreach (var item in Partes)
            {
                Parte parte1 = new Parte()
                {
                    NroContratoInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParte.Item1 : item.CodLista == "2" ? codigoParte.Item2 : codigoParte.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoConsignacionPrecioHechoDetalleContrato detalleContrato = new DetalleDocumentoConsignacionPrecioHechoDetalleContrato();
            #region DetalleDocumentoConsignacionPrecioHechoDetalleContrato
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;

            TCaption fechaConcertacion = new TCaption() { Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null };
            detalleContrato.FechaConcertacion = fechaConcertacion;

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            // GSIAN: Que va acá?
            detalleContrato.Precio = "";

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCodCaption unidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedidaPrecio = unidadMedidaPrecio;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            // GSIAN: Que va acá? En otros está "ComisionPorComprador"
            TCaption comisionPorConsignatario = new TCaption();
            detalleContrato.ComisionPorConsignatario = comisionPorConsignatario;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            // GSIAN: qué va acá?
            ProduccionComitente produccionComitente = new ProduccionComitente();
            detalleContrato.ProduccionComitente = produccionComitente;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;

            // GSIAN: qué va acá?
            OperacionExentaImpSantaFe operacionExentaImpSantaFe = new OperacionExentaImpSantaFe();
            detalleContrato.OperacionExentaImpSantaFe = operacionExentaImpSantaFe;

            #endregion DetalleDocumentoConsignacionPrecioHechoDetalleContrato

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DetalleContrato = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoConsignacionPrecioHecho

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoConsignacionPrecioHecho

            return item1;
        }
        // NO SE USA
        DocumentoOIVPrecioFijarRofex DevolverItemDocumentoOIVPrecioFijarRofex(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoOIVPrecioFijarRofex item1 = new DocumentoOIVPrecioFijarRofex();
            #region DocumentoFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoOIVPrecioFijarRofex detalleDocumento = new DetalleDocumentoOIVPrecioFijarRofex();
            #region DetalleDocumentoOIVPrecioFijarRofex
            List<ParteOIV> partes_detalle = new List<ParteOIV>();
            #region ParteOIV
            foreach (var item in Partes)
            {
                ParteOIV parte1 = new ParteOIV()
                {
                    NroInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParteOIV.Item1 : item.CodLista == "2" ? codigoParteOIV.Item2 : codigoParteOIV.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion ParteOIV

            DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalle detalleContrato = new DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalle();
            #region DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalle
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;

            TCaption fechaInicioTramite = new TCaption()
            {
                Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null,
            };
            detalleContrato.FechaInicioTramite = fechaInicioTramite;

            // GSIAN: Cómo se completa?
            detalleContrato.Vigencia = new TCaption();

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCaption montoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.MontoImponible = montoImponible;

            TCodCaption unidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedidaPrecio = unidadMedidaPrecio;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            // GSIAN: Cómo se completa?
            TCaption comisionPorDestinatario = new TCaption();
            comisionPorDestinatario.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
            detalleContrato.ComisionPorDestinatario = comisionPorDestinatario;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;
                #region Fijacion
                DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleFijacion fijacion = new DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleFijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { CodLista = "K" },
                    FijPeriodo = new TCodCaption() { CodLista = "1" },
                    FijFecDesde1 = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta1 = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    FechaPosicionRofex1 = new TFecha() { Anio = "", Mes = "", Value = "" }, // ????
                    IndiceRofex1 = new DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleFijacionIndiceRofex1() { Codigo = "", Value = "" }, // ???
                    FijFecDesde2 = new TCaption(), // ???
                    FijFecHasta2 = new TCaption(), // ???
                    FechaPosicionRofex2 = new TFecha() { Anio = "", Mes = "", Value = "" }, // ????
                    IndiceRofex2 = new DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleFijacionIndiceRofex2() { Codigo = "", Value = "" }, // ???
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                };
                #endregion Fijacion
                detalleContrato.Fijacion = fijacion;
            }

            // GSIAN: Cómo se completa?
            DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleProduccionOferente produccionOferente = new DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalleProduccionOferente();
            detalleContrato.ProduccionOferente = produccionOferente;

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoOIVPrecioFijarRofexDocumentoDetalle

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DocumentoDetalle = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoOIVPrecioFijarRofex

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoFijarPrecio

            return item1;
        }
        // NO SE USA
        DocumentoOIVFijarPrecio DevolverItemDocumentoOIVFijarPrecio(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoOIVFijarPrecio item1 = new DocumentoOIVFijarPrecio();
            #region DocumentoOIVFijarPrecio
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoOIVFijarPrecio detalleDocumento = new DetalleDocumentoOIVFijarPrecio();
            #region DetalleDocumentoOIVFijarPrecio
            List<ParteOIV> partes_detalle = new List<ParteOIV>();
            #region Parte
            foreach (var item in Partes)
            {
                ParteOIV parte1 = new ParteOIV()
                {
                    NroInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParteOIV.Item1 : item.CodLista == "2" ? codigoParteOIV.Item2 : codigoParteOIV.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoOIVFijarPrecioDocumentoDetalle detalleContrato = new DetalleDocumentoOIVFijarPrecioDocumentoDetalle();
            #region DetalleDocumentoOIVFijarPrecioDocumentoDetalle
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;

            TCaption fechaInicioTramite = new TCaption()
            {
                Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null,
            };
            detalleContrato.FechaInicioTramite = fechaInicioTramite;

            // GSIAN: Cómo completarlo??
            detalleContrato.Vigencia = new TCaption();

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCaption montoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.MontoImponible = montoImponible;

            TCodCaption unidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedidaPrecio = unidadMedidaPrecio;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            TCaption comisionPorDestinatario = new TCaption();
            comisionPorDestinatario.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
            detalleContrato.ComisionPorDestinatario = comisionPorDestinatario;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                bool noTieneCondicionesDeFijacion = contrato.EsFason == true || contrato.PrestamoDevolucion == true;
                #region Fijacion
                Fijacion fijacion = new Fijacion()
                {
                    FijMinima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMinima).ToString() },
                    FijMaxima = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : Convert.ToInt32(condiciones.CantidadMaxima).ToString() },
                    UnidadMedidaFijacion = new TCodCaption() { CodLista = "K" },
                    FijPeriodo = new TCodCaption() { CodLista = "1" },
                    FijFecDesde = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaDesde) },
                    FijFecHasta = new TCaption() { Value = noTieneCondicionesDeFijacion ? "" : CorregirFormatoFecha(condiciones.FechaHasta) },
                    PorcMultaIncumplimiento = new TCaption() { Value = "010" },
                    ComunicacionFijacion = new TCodCaption() { CodLista = contrato.PagoDirectoVendedor == true ? "2" : "1" },
                    //PizarraFijacion = new TCodCaption() { CodLista = contrato.Pizarra == true ? "1" : "" },
                };

                if (contrato.Pizarra == true)
                    fijacion.PizarraFijacion = new TCodCaption() { CodLista = "1" };
                #endregion Fijacion
                detalleContrato.Fijacion = fijacion;
            }

            DetalleDocumentoOIVFijarPrecioDocumentoDetalleProduccionOferente produccionOferente = new DetalleDocumentoOIVFijarPrecioDocumentoDetalleProduccionOferente();
            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            produccionOferente.CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.PagoDirectoVendedor == true ? "1" : "4") : (contrato.Consignatario == true ? "5" : "2");
            produccionOferente.ClausulaText = "";
            detalleContrato.ProduccionOferente = produccionOferente;

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoOIVFijarPrecioDocumentoDetalle

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DocumentoDetalle = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoOIVFijarPrecio

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoOIVFijarPrecio

            return item1;
        }
        // NO SE USA
        DocumentoOIVPrecioHecho DevolverItemDocumentoOIVPrecioHecho(BasicoContrato contrato, List<ResultadoClausula> clausulas, List<ConfirmaParteDto> Partes, bool esCanje, bool esConvenio, CondicionFijacionEstadoBoletoDto condiciones, EstadoSAPDto estadoSAP)
        {
            DocumentoOIVPrecioHecho item1 = new DocumentoOIVPrecioHecho();
            #region DocumentoOIVPrecioHecho
            #region CabeceraDocumento
            CabeceraDocumento cabeceraDocumento = new CabeceraDocumento()
            {
                Bolsa = new TCodLista()
                {
                    CodLista = contrato.BolsaConfirma,
                },
                TipoDocumento = new TCodLista()
                {
                    CodLista = esCanje ? "17" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")),
                }
            };
            #endregion CabeceraDocumento

            DetalleDocumentoOIVPrecioHecho detalleDocumento = new DetalleDocumentoOIVPrecioHecho();
            #region DetalleDocumentoOIVPrecioHecho
            List<ParteOIV> partes_detalle = new List<ParteOIV>();
            #region Parte
            foreach (var item in Partes)
            {
                ParteOIV parte1 = new ParteOIV()
                {
                    NroInterno = new TCaption()
                    {
                        Value = item.NroContratoInterno,
                    },
                    CUIT = new TCodCaption()
                    {
                        Value = item.CUIT,
                    },
                    CodLista = item.CodLista == "1" ? codigoParteOIV.Item1 : item.CodLista == "2" ? codigoParteOIV.Item2 : codigoParteOIV.Item3,
                    CodListaSpecified = true,
                    // falta sucursal?,
                };

                partes_detalle.Add(parte1);
            }
            #endregion Parte

            DetalleDocumentoOIVPrecioHechoDocumentoDetalle detalleContrato = new DetalleDocumentoOIVPrecioHechoDocumentoDetalle();
            #region DetalleDocumentoOIVPrecioHechoDocumentoDetalle
            Producto producto = new Producto()
            {
                CodLista = contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty,
                CodConv = "",
            };
            detalleContrato.Producto = producto;

            TCaption fechaInicioTramite = new TCaption()
            {
                Value = contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null,
            };
            detalleContrato.FechaInicioTramite = fechaInicioTramite;

            // GSIAN: Cómo completarlo??
            detalleContrato.Vigencia = new TCaption();

            // GSIAN: Tomé la desición que si el A FIJAR no tiene moneda, la buscamos en sus FIJACIONES.
            string monedaAFijar = "";
            if (contrato.Moneda == "ARP" || contrato.Moneda == "USD")
            {
                monedaAFijar = contrato.Moneda;
            }
            else
            {
                string monedaIdAFijar = repositorio.Listar<Negocio>(x =>
                x.ContratoSAP == contrato.ContratoSAP &&
                x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                x.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                x.ConfirmadoSAP == true).Find(x => x.MonedaId != null).MonedaId;
                monedaAFijar = repositorio.Obtener<Moneda>(x => x.MonedaId == monedaIdAFijar)?.Descripcion;
            }

            TCodLista moneda = new TCodLista() { CodLista = monedaAFijar == "ARP" ? "1" : monedaAFijar == "USD" ? "2" : string.Empty };
            detalleContrato.Moneda = moneda;

            // GSIAN: Qué va acá??
            detalleContrato.Precio = "";

            TCaption descAdicional = new TCaption() { Value = esCanje ? "INSUMO" : string.Empty };
            detalleContrato.DescAdicional = descAdicional;

            TCaption montoImponible = new TCaption() { Value = string.Empty };
            detalleContrato.MontoImponible = montoImponible;

            TCodCaption unidadMedidaPrecio = new TCodCaption() { CodLista = "T" }; // Tonelada
            detalleContrato.UnidadMedidaPrecio = unidadMedidaPrecio;

            TCodCaption unidadMedida = new TCodCaption() { CodLista = "K" }; // Kilo
            detalleContrato.UnidadMedida = unidadMedida;

            TCaption cantidadDesde = new TCaption() { Value = contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadDesde = cantidadDesde;

            TCaption cantidadHasta = new TCaption() { Value = contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : ((int)contrato.Cantidad).ToString() };
            detalleContrato.CantidadHasta = cantidadHasta;

            TCodLista cosecha = new TCodLista() { CodLista = contrato.CampanaConfirma };
            detalleContrato.Cosecha = cosecha;

            TCodLista ajuste = new TCodLista() { CodLista = string.Empty };
            detalleContrato.Ajuste = ajuste;

            TCaption cantCamiones = new TCaption();
            // GSIAN: Tomé la desición de agregar el cálculo porque Confirma me exige los camiones. Pero cuando le paso el valor, dice no ser correcto. Le mando sólo el caption. SAP sólo pasa etiqueta.
            //cantCamiones.Value = contrato.CantidadCamiones > 0 ? contrato.CantidadCamiones.ToString() : (Convert.ToInt32(Math.Ceiling((decimal)contrato.Cantidad / 30000))).ToString();
            cantCamiones.Caption = "";
            detalleContrato.CantCamiones = cantCamiones;

            TCaption comisionPorDestinatario = new TCaption();
            comisionPorDestinatario.Value = contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision.ToString() : null; // será PorcComisionComprador ???
            detalleContrato.ComisionPorDestinatario = comisionPorDestinatario;

            #region Calidad
            ConfirmaQALoteDocumentos.Calidad calidad = new ConfirmaQALoteDocumentos.Calidad()
            {
                CondicionesCalidad = new TCodLista() { CodLista = (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : "") },
                OtrasCondicionesCalidad = new TCaption() { Value = string.Empty }
            };
            #endregion Calidad
            detalleContrato.Calidad = calidad;

            TCodCaption medioTransporte = new TCodCaption() { CodLista = "C" }; // Camión
            detalleContrato.MedioTransporte = medioTransporte;

            #region Entrega
            Entrega entregas = new Entrega()
            {
                EntregaDesde = new TCaption() { Value = contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null },
                EntregaHasta = new TCaption() { Value = contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null }
            };
            #endregion Entrega
            detalleContrato.Entregas = entregas;

            Origen origen = new Origen();
            #region Origen
            OrigenLocalidadOrigen localidadOrigen = new OrigenLocalidadOrigen();
            localidadOrigen.LocalidadText = "";
            localidadOrigen.Value = contrato.LocalidadConfirma;

            TCodCaption provinciaOrigen = new TCodCaption();
            provinciaOrigen.CodLista = contrato.ProvinciaConfirma;

            origen.LocalidadOrigen = localidadOrigen;
            origen.ProvinciaOrigen = provinciaOrigen;
            #endregion Origen
            detalleContrato.Origen = origen;

            TCodCaption destino = new TCodCaption();
            destino.CodLista = contrato.DestinoConfirma;
            //destino.CodPrv = "0000"; // no está en Staging?
            detalleContrato.Destino = destino;

            TCodCaption provinciaInstrumentacion = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES
            detalleContrato.ProvinciaInstrumentacion = provinciaInstrumentacion;

            if (esCanje != true)
            {
                Pagos pagos = new Pagos();
                #region Pagos
                TCaption fechaCondicionPago = new TCaption();
                fechaCondicionPago.Value = esCanje != true ? (
                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                "4 días hábiles de fecha de fijación" : (
                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                            (contrato.CD == true) ?
                "Pago Contra CD" : (
                                (contrato.Warrant == true) ?
                "Pago contra Warrant" : (
                                    (contrato.PagoDiferido == true) ?
                                        "72 hs contra mercadería descargada." :
                                        "Días de diferimiento contra mercadería entregada"
                                    )
                                )
                            ) : null
                        )
                    )
                    : null;

                TCaption lugarPago = new TCaption() { Value = "BUENOS AIRES" };

                PagoAOrdenDe pagoAOrdenDe = new PagoAOrdenDe()
                {
                    CodLista = contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1",
                };

                TCaption porcPago = new TCaption() { Value = contrato.PorcentajeDePago.Value.ToString() };

                TCodCaption provinciaPago = new TCodCaption() { CodLista = "B" }; // BUENOS AIRES

                pagos.FechaCondicionPago = fechaCondicionPago;
                pagos.LugarPago = lugarPago;
                pagos.PagoAOrdenDe = pagoAOrdenDe;
                pagos.PorcPago = porcPago;
                pagos.ProvinciaPago = provinciaPago;
                #endregion Pagos
                detalleContrato.Pagos = pagos;
            }

            DetalleDocumentoOIVPrecioHechoDocumentoDetalleProduccionOferente produccionOferente = new DetalleDocumentoOIVPrecioHechoDocumentoDetalleProduccionOferente();
            // GSIAN: Tuve que forzar a "2" porque con "4" me dice "El campo Producción Vendedor no es válido."
            produccionOferente.CodLista = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.PagoDirectoVendedor == true ? "1" : "4") : (contrato.Consignatario == true ? "5" : "2");
            produccionOferente.ClausulaText = "";
            detalleContrato.ProduccionOferente = produccionOferente;

            // GSIAN: está en proyecto SOAP, pero no se usa en ConfirmaManager. Se deja descomentado para prueba momentanea.
            DecisionPagoVoluntario decisionPagoVoluntario = new DecisionPagoVoluntario();
            decisionPagoVoluntario.CodLista = "2"; // NO. Se completa porque me lo solicita Staging.
            decisionPagoVoluntario.FondoFederalText = "";
            detalleContrato.DecisionPagoVoluntario = decisionPagoVoluntario;

            TCodLista tipoOperacion = new TCodLista() { CodLista = "1" }; // Cereal
            detalleContrato.TipoOperacion = tipoOperacion;

            // GSIAN: Qué va acá??
            detalleContrato.EntregadeMercaderia = new TCodLista();

            //SioGranos sioGranos = new SioGranos();
            //sioGranos.NumeroDeclaracion = estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null;
            //detalleContrato.SioGranos = sioGranos;
            #endregion DetalleDocumentoOIVPrecioHechoDocumentoDetalle

            List<ConfirmaQALoteDocumentos.Clausula> clausulas_detalle = new List<ConfirmaQALoteDocumentos.Clausula>();
            #region Cláusulas
            foreach (var item in clausulas)
            {
                ConfirmaQALoteDocumentos.Clausula clausula = new ConfirmaQALoteDocumentos.Clausula();
                clausula.Orden = string.Empty;
                clausula.Value = item.Texto;

                clausulas_detalle.Add(clausula);
            }
            #endregion Cláusulas

            detalleDocumento.Partes = partes_detalle.ToArray();
            detalleDocumento.DocumentoDetalle = detalleContrato;
            detalleDocumento.Clausulas = clausulas_detalle.ToArray();
            #endregion DetalleDocumentoOIVPrecioHecho

            item1.CabeceraDocumento = cabeceraDocumento;
            item1.DetalleDocumento = detalleDocumento;
            //item1.Id = "";
            #endregion DocumentoOIVPrecioHecho

            return item1;
        }
        #endregion

        public string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public ConfirmaAltaLoteResultDto ResultadoAltaDefinitiva(altaLoteResult devolucion, EstadosConfirmaDto estadosConfirmaDto)
        {
            ConfirmaAltaLoteResultDto confirmaAltaLoteResult = new ConfirmaAltaLoteResultDto();

            confirmaAltaLoteResult.altaIdLote = devolucion.altaIdLote;
            confirmaAltaLoteResult.altaEstado = int.Parse(new string(devolucion.altaEstado.ToString().Where(char.IsDigit).ToArray()));
            confirmaAltaLoteResult.confirmaAltaEstado = estadosConfirmaDto.ConfirmaAltaEstadoDto.Find(x => x.CodigoConfirmaAltaEstado == confirmaAltaLoteResult.altaEstado);
            confirmaAltaLoteResult.altaEstadoSpecified = devolucion.altaEstadoSpecified;
            confirmaAltaLoteResult.altaEstadoDetalleError = devolucion.altaEstadoDetalleError;
            confirmaAltaLoteResult.altaEstadoLote = int.Parse(new string(devolucion.altaEstadoLote.ToString().Where(char.IsDigit).ToArray()));
            confirmaAltaLoteResult.confirmaAltaEstadoLote = estadosConfirmaDto.ConfirmaAltaEstadoLoteDto.Find(x => x.CodigoConfirmaAltaEstadoLote == confirmaAltaLoteResult.altaEstadoLote);
            confirmaAltaLoteResult.altaEstadoLoteSpecified = devolucion.altaEstadoLoteSpecified;
            confirmaAltaLoteResult.altaItem = new List<altaItemDto>();

            if (devolucion.altaItem != null)
            {
                foreach (var item in devolucion.altaItem)
                {
                    altaItemDto altaItem = new altaItemDto();
                    altaItem.altaIdDocumento = item.altaIdDocumento;
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
        private decimal? PrecioNetoSustentableSobrePrecio(BasicoContrato contrato, decimal? precioNetoSustentable)
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
            return precioNetoSustentable;
        }

    }
}
