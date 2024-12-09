using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web;
using System.Web.WebPages;
using System.Xml.Linq;

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
        private readonly IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent;
        private readonly IConfirmaLoteDocumentosAgent confirmaLoteDocumentosAgent;
        private readonly IConfirmaLoteBorradorAgent confirmaLoteBorradorAgent;
        private readonly string pathConfirmas;

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status, IEnviarBoletoAgent oEnviarBoletoAgent,
            IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent, IMailManager mailManager, IHttpContextManager httpContextManager,
            IServicioClausulas servicioClausula, IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent, IConfirmaLoteDocumentosAgent confirmaLoteDocumentosAgent,
            IConfirmaLoteBorradorAgent confirmaLoteBorradorAgent)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.status = status;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
            this.servicioClausula = servicioClausula;
            this.oEnviarBoletoAgent = oEnviarBoletoAgent;
            this.confirmaConsultaDocumentosAgent = confirmaConsultaDocumentosAgent;
            this.confirmaLoteDocumentosAgent = confirmaLoteDocumentosAgent;
            this.confirmaLoteBorradorAgent = confirmaLoteBorradorAgent;
            pathConfirmas = ConfigurationManager.AppSettings["PathConfirmas"].ToString();
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<string> clausulas, List<int> equipo)
        {
            var codigos = CompletarCodigoLista(codigosSap);
            var contratos = ObtenerContratos(codigos, claseNegocio, equipo);
            var resultado = new ConfirmaResult();
            string activarConfirmaWS = ConfigurationManager.AppSettings["ActivarConfirmaWS"];
            string ambienteLocal = ConfigurationManager.AppSettings["AmbienteLocal"];
            string pruebaRapidaDeConfirma = ConfigurationManager.AppSettings["PruebaRapidaDeConfirma"];

            if (usarWebServiceConfirma && (ambienteLocal == "1" || pruebaRapidaDeConfirma == "1"))
            {
                logger.Info("---- PRUEBA: INICIO WS CONFIRMA ----");
                string CodigoSapCompleto = codigos[0].TrimStart('0').PadLeft(10, '0');
                IQueryable<BasicoContrato> consultaIQ = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { CodigoSapCompleto }, equipo));
                BasicoContrato contrato = consultaIQ.First();
                List<ResultadoClausula> clausulasConfirma = new List<ResultadoClausula>();
                clausulasConfirma = clausulas.Count() == 0 ? ObtenerClausulas(contrato) : clausulas.Select(x => new ResultadoClausula() { Texto = x, Orden = 0 }).ToList();

                EstadosConfirmaDto estadosConfirmaDto = new EstadosConfirmaDto
                {
                    ConfirmaAltaEstadoDto = repositorio.Listar<ConfirmaAltaEstado, ConfirmaAltaEstadoDto>(x => new ConfirmaAltaEstadoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstado = x.CodigoConfirmaAltaEstado }),
                    ConfirmaAltaEstadoLoteDto = repositorio.Listar<ConfirmaAltaEstadoLote, ConfirmaAltaEstadoLoteDto>(x => new ConfirmaAltaEstadoLoteDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoLote = x.CodigoConfirmaAltaEstadoLote }),
                    ConfirmaAltaEstadoDocumentoDto = repositorio.Listar<ConfirmaAltaEstadoDocumento, ConfirmaAltaEstadoDocumentoDto>(x => new ConfirmaAltaEstadoDocumentoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoDocumento = x.CodigoConfirmaAltaEstadoDocumento }),
                };

                ConfirmaAltaLoteBorradorResultDto confirmaAltaLoteBorradorResult = confirmaLoteBorradorAgent.ConfirmaLoteBorrador(clausulasConfirma, equipo, contrato, estadosConfirmaDto);
                bool tieneItems = false;
                if (confirmaAltaLoteBorradorResult.altaItem.Count() > 0) tieneItems = true;
                logger.Info($"WS: altaIdLote = {confirmaAltaLoteBorradorResult.altaIdLote}. altaEstado = {confirmaAltaLoteBorradorResult.confirmaAltaEstado?.Descripcion}. " +
                    $"altaEstadoLote = {confirmaAltaLoteBorradorResult.confirmaAltaEstadoLote?.Descripcion}. " +
                    $"altaEstadoDocumento = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].confirmaAltaEstadoDocumento.Descripcion}. " : ". ") +
                    $"altaIdDocumentoExistenteLote = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistenteLote}. " : ". ") +
                    $"altaIdDocumentoExistente = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistente}. " : ". ") +
                    $"codigo = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistente}. " : ". "));

                bool webServiceOK = false;
                if (confirmaAltaLoteBorradorResult.altaItem.Count() > 0 && confirmaAltaLoteBorradorResult.altaItem[0].confirmaAltaEstadoDocumento?.Id == (int)EnumConfirmaAltaEstadoDocumento.RECEPCION_CON_EXITO)
                    webServiceOK = true;

                string wsErrorConfirma = "";
                wsErrorConfirma += $"{confirmaAltaLoteBorradorResult.confirmaAltaEstado.Descripcion}";

                if (confirmaAltaLoteBorradorResult.altaEstadoDetalleError == null)
                    wsErrorConfirma += ". ";
                else
                    wsErrorConfirma += $": {confirmaAltaLoteBorradorResult.altaEstadoDetalleError}. ";

                wsErrorConfirma += $"{confirmaAltaLoteBorradorResult.confirmaAltaEstadoLote.Descripcion}. ";

                confirmaAltaLoteBorradorResult.altaItem?.ForEach(x => x.altaErrores?.ForEach(y =>
                {
                    wsErrorConfirma += $"{x.confirmaAltaEstadoDocumento.Descripcion}: {y}. ";
                }));

                resultado.confirmasGenerados.Add(DevolverDto(contrato, false, $"WS: {wsErrorConfirma}", webServiceOK));

                logger.Info("---- PRUEBA: FIN WS CONFIRMA ----");
            }

            try
            {
                if (contratos == null || contratos.Count == 0)
                {
                    logger.Info($"Generacion Confirma: No se hallaron Negocios SAP {String.Join(",", codigos)}.");
                    resultado.Errores.Add(new ErrorMessage(404, "Ningun Negocio Encontrado"));
                    return resultado;
                }

                EstadosConfirmaDto estadosConfirmaDto = new EstadosConfirmaDto
                {
                    ConfirmaAltaEstadoDto = repositorio.Listar<ConfirmaAltaEstado, ConfirmaAltaEstadoDto>(x => new ConfirmaAltaEstadoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstado = x.CodigoConfirmaAltaEstado }),
                    ConfirmaAltaEstadoLoteDto = repositorio.Listar<ConfirmaAltaEstadoLote, ConfirmaAltaEstadoLoteDto>(x => new ConfirmaAltaEstadoLoteDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoLote = x.CodigoConfirmaAltaEstadoLote }),
                    ConfirmaAltaEstadoDocumentoDto = repositorio.Listar<ConfirmaAltaEstadoDocumento, ConfirmaAltaEstadoDocumentoDto>(x => new ConfirmaAltaEstadoDocumentoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoDocumento = x.CodigoConfirmaAltaEstadoDocumento }),
                };

                foreach (var contrato in contratos)
                {
                    var mensaje = ValidarContrato(contrato);
                    var esValido = mensaje == "" ? true : false;
                    if (!esValido)
                    {
                        logger.Info($"Generacion Confirma: No es valido el Negocio SAP {contrato.Negocio}");
                        resultado.confirmasGenerados.Add(DevolverDto(contrato, false, mensaje));
                        continue;
                    }

                    //CONSULTAR EXISTE CONFIRMA/BOLETO A RFC
                    var consultaConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(contrato.ContratoSAP, contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : "");
                    if (consultaConfirma.Generado == "" || consultaConfirma.Anulado.Equals("X")) // probar casos anulados
                    { //Consulta: El Boleto/Confirma no existe o no esta generado en SAP???
                        //PRECARGAR CONFIRMA GENERADO DTO
                        var tempConfirma = new ConfirmaGeneradoDto()
                        {
                            NegocioId = contrato.Id,
                            Version = String.IsNullOrEmpty(consultaConfirma.Version) ? 1 : Convert.ToInt32(consultaConfirma.Version) + 1,
                            ComercialId = ComercialId,
                            FechaGeneracion = DateTime.Now,
                            ContratoSAP = contrato.ContratoSAP,
                            FijacionSAP = contrato.FijacionSAP,
                            TipoBoletoId = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? 1 : contrato.BoletoId.GetValueOrDefault(),
                            IsWebService = false,
                            NegocioSAP = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP,
                            Mensaje = string.Empty,
                            Generado = true
                        };
                        //Enviando Confirma a RFC como BoletoGeneradoDto
                        logger.Debug("Confirma: Enviando Boleto confirma" + tempConfirma.ToString());
                        var res = oEnviarBoletoAgent.EnviarBoleto(ConfirmaABoletoDto(tempConfirma));
                        logger.Debug("Confirma: Respuesta de la RFC" + res.ToString());
                        if (res == "Se actualizan correctamente los datos")
                        { //Generado exitosamente en RFC
                            try
                            {
                                //Se Almacena el ArchivoXML 
                                var xml = GenerarXML(contrato, clausulas);
                                File.WriteAllBytes(ConfigurationManager.AppSettings["PathConfirmas"].ToString() + "\\"
                                     + tempConfirma.Archivo, xml);
                                //Se Almacena en DB el nuevo Confirma
                                var nuevoConfirma = repositorio.Agregar(ConvertirDtoAEntidad(tempConfirma));

                                if (usarWebServiceConfirma && activarConfirmaWS == "1")
                                {
                                    logger.Info("---- INICIO WS CONFIRMA ----");
                                    List<ResultadoClausula> clausulasConfirma = new List<ResultadoClausula>();
                                    clausulasConfirma = clausulas.Count() == 0 ? ObtenerClausulas(contrato) : clausulas.Select(x => new ResultadoClausula() { Texto = x, Orden = 0 }).ToList();
                                    ConfirmaAltaLoteBorradorResultDto confirmaAltaLoteBorradorResult = confirmaLoteBorradorAgent.ConfirmaLoteBorrador(clausulasConfirma, equipo, contrato, estadosConfirmaDto);

                                    bool tieneItems = false;
                                    if (confirmaAltaLoteBorradorResult.altaItem.Count() > 0) tieneItems = true;
                                    logger.Info($"WS: altaIdLote = {confirmaAltaLoteBorradorResult.altaIdLote}. altaEstado = {confirmaAltaLoteBorradorResult.confirmaAltaEstado?.Descripcion}. " +
                                        $"altaEstadoLote = {confirmaAltaLoteBorradorResult.confirmaAltaEstadoLote?.Descripcion}. " +
                                        $"altaEstadoDocumento = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].confirmaAltaEstadoDocumento.Descripcion}. " : ". ") +
                                        $"altaIdDocumentoExistenteLote = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistenteLote}. " : ". ") +
                                        $"altaIdDocumentoExistente = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistente}. " : ". ") +
                                        $"codigo = " + (tieneItems ? $"{confirmaAltaLoteBorradorResult.altaItem[0].altaIdDocumentoExistente}. " : ". "));

                                    if (confirmaAltaLoteBorradorResult.altaItem.Count() > 0 && confirmaAltaLoteBorradorResult.altaItem[0].confirmaAltaEstadoDocumento?.Id == (int)EnumConfirmaAltaEstadoDocumento.RECEPCION_CON_EXITO)
                                    {
                                        nuevoConfirma.IsWebService = true;
                                        tempConfirma.IsWebService = true;
                                    }
                                    else
                                    {
                                        string wsErrorConfirma = "";
                                        wsErrorConfirma += $"{confirmaAltaLoteBorradorResult.confirmaAltaEstado.Descripcion}";

                                        if (confirmaAltaLoteBorradorResult.altaEstadoDetalleError == null)
                                            wsErrorConfirma += ". ";
                                        else
                                            wsErrorConfirma += $": {confirmaAltaLoteBorradorResult.altaEstadoDetalleError}. ";

                                        wsErrorConfirma += $"{confirmaAltaLoteBorradorResult.confirmaAltaEstadoLote.Descripcion}. ";

                                        confirmaAltaLoteBorradorResult.altaItem?.ForEach(x => x.altaErrores?.ForEach(y =>
                                        {
                                            wsErrorConfirma += $"{x.confirmaAltaEstadoDocumento.Descripcion}: {y}. ";
                                        }));

                                        tempConfirma.Mensaje += tempConfirma.Mensaje.IsEmpty() ? $"WS: {wsErrorConfirma}" : $". WS: {wsErrorConfirma}";
                                    }

                                    logger.Info("---- FIN WS CONFIRMA ----");
                                }

                                resultado.confirmasGenerados.Add(tempConfirma);
                            }
                            catch (Exception ex)
                            {
                                tempConfirma.Mensaje += "Error al guardar el XML del confirma: " + ex.Message;
                                resultado.confirmasGenerados.Add(DevolverDto(contrato, false, "El XML del confirma No se ha almacenado correctamente."));
                                logger.Error(ex);
                            }
                        }
                        else
                        {
                            resultado.confirmasGenerados.Add(DevolverDto(contrato, false, res));
                        }
                    }
                    else
                    {
                        DateTime? fechaGeneracionUltimoConfirma = repositorio.Listar<Confirma>(x => x.NegocioId == contrato.Id && x.FechaAnulacion == null)
                            .Select(x => x.FechaGeneracion)
                            .FirstOrDefault();

                        logger.Info($"Confirma.Generado = {consultaConfirma.Generado} -- contrato SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");
                        resultado.confirmasGenerados.Add(DevolverDto(contrato, false, "El boleto ya se encuentra generado en SAP.", false, fechaGeneracionUltimoConfirma));
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                resultado.Errores.Add(new ErrorMessage(400, e.Message));
            }
            return resultado;
        }

        public DataSourceResult TraerNegociosFiltrados(DataSourceRequest filtro, List<int> equipo)
        {
            var result = repositorio.ObtenerConsultaEscalar(new TraerConfirmasConFiltro(filtro, equipo)) ?? throw new InvalidOperationException("El resultado de la consulta es nulo.");
            var data = result.Data as IEnumerable<BasicoConfirma>;

            // Iterar sobre los datos y modificar atributos
            foreach (var boleto in data)
            {
                boleto.Estado_Version = ObtenerEstadoBoleto(boleto);
                if (boleto.Estado_Version == "Anulado")
                {
                    boleto.Estado_Version = "Pendiente";
                    boleto.Version = boleto.Version + 1;
                    boleto.FechaAnulacion = null;
                    boleto.FechaGeneracion = null;
                    boleto.UsuarioAnulacion = null;
                }
            }
            return result;
        }

        private string ValidarContrato(BasicoContrato contrato)
        {
            var mensaje = "";
            var kilosMinimos = 10000;

            if (contrato != null)
            {
                if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION) //FIJACION
                {
                    if (contrato.Cantidad < kilosMinimos)
                    {
                        mensaje = $"No se puede generar el confirma {contrato.FijacionSAP} por su cantidad menor a 10 toneladas.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {contrato.FijacionSAP} por cantidad menor a 10 toneladas.");
                        return mensaje;
                    }

                    if (contrato.Canje != true)
                    {
                        mensaje = $"No se puede generar el confirma {contrato.FijacionSAP} por no ser de Canje el A Fijar correspondiente.";
                        logger.Debug($"No se puede generar el confirma para la fijacion {contrato.FijacionSAP} por no ser de plan canje el A Fijar correspondiente.");
                        return mensaje;
                    }
                }
                else
                { //CONTRATO
                    //Validar estado del contrato
                    var res = status.ValidarEstado(contrato.ContratoSAP);
                    if (!string.IsNullOrEmpty(res.Status) && res.Status != "X")
                    {
                        string motivoStatus = StatusNegocioConfirma(res);
                        mensaje = $"No se puede generar el confirma con negocio {contrato.ContratoSAP} para el contrato por su estado: {motivoStatus}";
                        logger.Debug($"No se puede generar el confirma por el status: {res.Status} ({motivoStatus}) - ContratoSAP: {contrato.ContratoSAP}");
                        return mensaje;
                    }
                    else if (string.IsNullOrEmpty(res.Status))
                    {
                        mensaje = $"No se puede generar el confirma con negocio {contrato.ContratoSAP} para el contrato por estar en slip.";
                        logger.Debug($"No se puede generar el confirma por tener status vacío (slip) - ContratoSAP: {contrato.ContratoSAP}");
                        return mensaje;
                    }

                    // Validar que tenga tilde CONFIRMA
                    if (contrato.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA)
                    {
                        mensaje = $"No se puede generar el confirma {contrato.ContratoSAP} por no tener tilde de confirma.";
                        logger.Debug($"No se puede generar el confirma para el negocio {contrato.ContratoSAP} por no tener tilde de confirma.");
                        return mensaje;
                    }
                }
            }
            else
            {
                mensaje = $"No se encontró el negocio {contrato.ContratoSAP} seleccionado.";
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

        public byte[] ObtenerArchivoXML(string nombreArchivo)
        {
            // Obtener la ruta base del archivo desde la configuración
            string rutaBase = ConfigurationManager.AppSettings["PathConfirmas"];

            // Construir la ruta completa del archivo XML
            string rutaArchivo = Path.Combine(rutaBase, nombreArchivo);

            // Verificar si el archivo existe
            if (!File.Exists(rutaArchivo))
            {
                // Retornar null si el archivo no existe
                return null;
            }

            // Leer y devolver el archivo como un array de bytes
            return File.ReadAllBytes(rutaArchivo);
        }

        private byte[] GenerarXML(BasicoContrato contrato, List<string> clausulas)
        {
            try
            {
                logger.Info($"Datos del Negocio de Confirma Precargados; BolsaConfirma:{contrato.BolsaConfirma}, CorredorId:{contrato.CorredorId}, TipoNegocioId:{contrato.TipoNegocioId}, MaterialId:{contrato.MaterialId}, FechaOperacion:{contrato.FechaOperacion}, CampañaConfirma: {contrato.CampanaConfirma}, KgMaximo:{contrato.KgMaximo}, KgMinimo:{contrato.KgMinimo}, Cantidad:{contrato.Cantidad}, CantidadCamiones:{contrato.CantidadCamiones}, Moneda:{contrato.Moneda}, Precio:{contrato.Precio}, PorcentajeComision:{contrato.PorcentajeComision}, StandardDeCalidadId:{contrato.StandardDeCalidadId}, FechaDesde:{contrato.FechaDesde}, FechaHasta:{contrato.FechaHasta}, LocalidadConfirma:{contrato.LocalidadConfirma}, ProvinciaConfirma:{contrato.ProvinciaConfirma}, DestinoConfirma:{contrato.DestinoConfirma}, CD:{contrato.CD}, Warrant:{contrato.Warrant}, PagoDiferido:{contrato.PagoDiferido}, PagoDirectoVendedor:{contrato.PagoDirectoVendedor}, PorcentajeDePago:{contrato.PorcentajeDePago}, Monto:{contrato.Monto}, Pizarra:{contrato.Pizarra}, ClasificacionId:{contrato.ClasificacionId}");
                //Calcular datos para el XML
                var estadoSAP = status.ValidarEstado(contrato.ContratoSAP);
                if (estadoSAP is null) throw new ArgumentNullException("EstadoSAP", $"Descargar XML Confirma - Error al consultar el estadoSAP asociado al contrato: {(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)}");
                else logger.Info($"Descargar XML Confirma - Se Consulta el status del contrato SAP {(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)} resultando STATUS: {estadoSAP.Status} y Mensaje: {estadoSAP.Mensaje}");
                var datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(contrato.ContratoSAP, contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : "");
                var condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
                if (datosConfirma is null || condiciones is null) logger.Info("Alerta CondicionFijacion", $"Descargar XML Confirma - Se consultó el Estado del Boleto SAP del contrato {(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)} y no tiene condiciones de fijacion asociadas.");
                else logger.Info($"Descargar XML Confirma - Se consultó el Estado del Boleto SAP del contrato {(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)}. Resultando las condiciones fijacion: CantidadMaxima: {condiciones.CantidadMaxima} y CantidadMinima: {condiciones.CantidadMinima}.");
                var CuitMolinos = ConfigurationManager.AppSettings["Cuit"];
                var esConvenio = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && contrato.Madre == true;
                var esCanje = contrato.Canje == true;
                var nroContratoInterno = (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP).TrimStart('0');
                string tipoDocumento = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : esCanje ? "17" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : string.Empty;
                var Partes = (contrato.CorredorId > 0) ?
                    new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = contrato.Cuit, Sucursal = string.Empty }, new { CodLista = "2", NroContratoInterno = nroContratoInterno, CUIT = contrato.CUITCorredor, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = CuitMolinos, Sucursal = string.Empty } }
                    : new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = contrato.Cuit, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = CuitMolinos, Sucursal = string.Empty } };
                logger.Info($"Datos Precalculados del Negocio de Confirma; Codigo:{(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)}, esCanje:{esCanje}, esConvenio:{esConvenio}, Partes: {string.Join(" - ", Partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");
                var clausulasConfirma = clausulas.Count() == 0 ? ObtenerClausulas(contrato) : clausulas.Select(x => new ResultadoClausula() { Texto = x, Orden = 0 }).ToList();
                if (clausulasConfirma is null || clausulasConfirma.Count == 0) throw new ArgumentNullException("Clausulas", $"Descargar XML Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {(contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP)}.");
                //Fin de carga de datos
                MemoryStream ms = new MemoryStream(); //Memory Stream
                                                      //Inicia formateo del XML
                var doc = new XDocument(
                    new XElement("Lote",
                        new XElement("Documento", new XAttribute("xmlns-fakexmlns", "Documento"),

                #region CabeceraDocumento

                            new XElement("CabeceraDocumento",
                                new XElement("Bolsa", new XAttribute("CodLista", contrato.BolsaConfirma)),
                                new XElement("TipoDocumento", new XAttribute("CodLista", tipoDocumento)),
                                new XElement("Formulario", new XAttribute("formversion", "1.04"))
                            ),//Fin Nodo CabeceraDocumento

                #endregion CabeceraDocumento

                            new XElement("UploadInfo",
                                new XElement("Workflow", contrato.CorredorId > 0 ? "4" : "7")
                            ),
                            new XElement("DetalleDocumento",

                #region Partes

                                //Inicio Nodo Partes
                                new XElement("Partes",
                                    //Inicio Parte
                                    Partes.Select(x =>
                                        new XElement("Parte",
                                            new XAttribute("CodLista", x.CodLista),
                                            new XElement("NroContratoInterno", x.NroContratoInterno),
                                            new XElement("CUIT", x.CUIT),
                                            new XElement("Sucursal", new XAttribute("CodLista", string.Empty))
                                        )
                                    )//Fin Nodo Parte
                                ),//Fin Nodo Partes

                #endregion Partes

                #region DetalleContrato

                                //Inicio DetalleContrato
                                new XElement("DetalleContrato",
                                    new XElement("Producto", new XAttribute("CodLista", contrato.MaterialId == (int)EnumMateriales.TRIGO ? "1" : contrato.MaterialId == (int)EnumMateriales.MAIZ ? "2" : contrato.MaterialId == (int)EnumMateriales.SORGO ? "3" : contrato.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : contrato.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty)),
                                    new XElement("DescAdicional", esCanje ? "INSUMO" : null),
                                    new XElement("FechaConcertacion", contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : null),
                                    new XElement("Cosecha", new XAttribute("CodLista", contrato.CampanaConfirma)),
                                    new XElement("UnidadMedida", new XAttribute("CodLista", "K")),
                                    new XElement("CantidadDesde", contrato.KgMinimo > 0 ? contrato.KgMinimo : (int)contrato.Cantidad),
                                    new XElement("CantidadHasta", contrato.KgMaximo > 0 ? contrato.KgMaximo : (int)contrato.Cantidad),
                                    new XElement("Ajuste", new XAttribute("CodLista", string.Empty)),
                                    new XElement("CantCamiones", contrato.CantidadCamiones),
                                    (esCanje || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? new XElement("MontoImponible") : null),
                                    new XElement("Moneda", new XAttribute("CodLista", contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : (String.IsNullOrEmpty(contrato.Moneda) ? "2" : string.Empty))),
                                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("Precio", contrato.Precio) : null),
                                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("UnidadMedidaPrecio", new XAttribute("CodLista", "T")) : null),
                                    (tipoDocumento != "17" ? new XElement("PorcComisionComprador", contrato.PorcentajeComision.HasValue ? contrato.PorcentajeDePago.Value.ToString("F2", CultureInfo.InvariantCulture) : string.Empty) : null),

                #region Calidad

                                    new XElement("Calidad",
                                        new XElement("CondicionesCalidad", new XAttribute("CodLista", (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.CAMARA || contrato.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (contrato.StandardDeCalidadId == (int)EnumStandarCalidad.FABRICA ? "4" : ""))),
                                        new XElement("OtrasCondicionesCalidad")
                                    ),

                #endregion Calidad

                                    new XElement("MedioTransporte", new XAttribute("CodLista", "C")),

                #region Entregas

                                    new XElement("Entregas",
                                        new XElement("EntregaDesde", contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy") : null),
                                        new XElement("EntregaHasta", contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy") : null)
                                    ),

                #endregion Entregas

                #region Origen

                                    new XElement("Origen",
                                        new XElement("LocalidadOrigen", contrato.LocalidadConfirma),
                                        new XElement("ProvinciaOrigen", new XAttribute("CodLista", contrato.ProvinciaConfirma))
                                    ),

                #endregion Origen

                                    new XElement("Destino", new XAttribute("CodLista", contrato.DestinoConfirma), new XAttribute("CodPrv", "0000")),

                #region DecisionDeclara

                                    (esCanje ? new XElement("DecisionDeclaraPrecioUnit", new XAttribute("CodLista", "0")) : null),
                                    (esCanje ? new XElement("DecisionDeclaraCantidad", new XAttribute("CodLista", "0")) : null),

                #endregion DecisionDeclara

                                    new XElement("ProvinciaInstrumentacion", new XAttribute("CodLista", "B")),

                #region Pagos

                                        ((esCanje != true) ? new XElement("Pagos",
                                            new XElement("ProvinciaPago", new XAttribute("CodLista", "B")),
                                            new XElement("FechaCondicionPago",
                                            esCanje != true ? (
                                                (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                                                    "4 días hábiles de fecha de fijación" : (
                                                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                                                        (contrato.CD == true) ?
                                                            "Pago Anticipado" : (
                                                            (contrato.Warrant == true) ?
                                                                "Pago contra Warrant" : (
                                                                (contrato.PagoDiferido == true) ?
                                                                    "Días de diferimiento contra mercadería entregada" :
                                                                    "72 hs contra mercadería descargada."
                                                                )
                                                            )
                                                        ) : null
                                                    )
                                                )
                                                : null
                                            ),
                                            new XElement("LugarPago", "BUENOS AIRES"),
                                            new XElement("PagoAOrdenDe", new XAttribute("CodLista", contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1")),
                                            new XElement("PorcPago", contrato.PorcentajeDePago.Value.ToString("F2", CultureInfo.InvariantCulture))
                                        ) : null),

                #endregion Pagos

                #region Insumos

                                        (esCanje ? new XElement("Insumos",
                                            new XElement("Productos",
                                                new XElement("Insumo",
                                                    new XElement("Producto", new XAttribute("CodLista", "1")),
                                                    new XElement("DescAdicional", "insumos"),
                                                    new XElement("Cantidad"),
                                                    new XElement("Precio"),
                                                    new XElement("UnidadMedida", new XAttribute("CodLista", string.Empty)),
                                                    new XElement("UnidadMedidaPrecio", new XAttribute("CodLista", string.Empty))
                                                )
                                            ),
                                            new XElement("Moneda", new XAttribute("CodLista", contrato.Monto.HasValue ? (contrato.MonedaCanjeId.Trim() == "ARP" ? "1" : "2") : string.Empty)),
                                            new XElement("PrecioTotal", contrato.Monto),
                                            new XElement("Factura"),
                                            new XElement("PorcentajeGastos"),
                                            new XElement("TipoCambioPesos"),
                                            new XElement("LugarEntrega"),
                                            new XElement("ProvinciaEntrega", contrato.ProvinciaConfirma)
                                        ) : null),

                #endregion Insumos

                #region Fijacion

                                        (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) ? new XElement("Fijacion",
                                            new XElement("FijMinima", condiciones != null ? Convert.ToInt32(condiciones.CantidadMinima).ToString() : contrato.KgMinimo > 0 ? contrato.KgMinimo.ToString() : null),
                                            new XElement("FijMaxima", condiciones != null ? Convert.ToInt32(condiciones.CantidadMaxima).ToString() : contrato.KgMaximo > 0 ? contrato.KgMaximo.ToString() : null),
                                            new XElement("UnidadMedidaFijacion", new XAttribute("Caption", "K"), new XAttribute("CodLista", "K")),
                                            new XElement("FijPeriodo", "1"),
                                            new XElement("FijFecDesde", condiciones != null ? CorregirFormatoFecha(condiciones.FechaDesde) : contrato.FechaDesde.HasValue ? contrato.FechaDesde.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null),
                                            new XElement("FijFecHasta", condiciones != null ? CorregirFormatoFecha(condiciones.FechaHasta) : contrato.FechaHasta.HasValue ? contrato.FechaHasta.Value.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null),
                                            new XElement("PorcMultaIncumplimiento", "010"),
                                            new XElement("ComunicacionFijacion", new XAttribute("CodLista", contrato.PagoDirectoVendedor == true ? "2" : "1")),
                                            (contrato.Pizarra == true ? new XElement("PizarraFijacion", new XAttribute("CodLista", "1")) : null)
                                        ) : null,

                #endregion Fijacion

                                    new XElement("ProduccionVendedor", new XAttribute("CodLista", contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.CorredorId > 0 ? "4" : "1") : (contrato.Consignatario == true ? "5" : "2"))),

                #region APRECIO

                                    ((contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ?
                                        new XElement("APrecio", new XAttribute("CodLista", "1"))
                                    : null),

                #endregion APRECIO

                                    new XElement("TipoOperacion", new XAttribute("CodLista", "1")),

                #region SioGranos

                                    new XElement("SioGranos",
                                        new XElement("NumeroDeclaracion", estadoSAP.NumeroSio > 0 ? estadoSAP.NumeroSio.ToString() : null),
                                        new XElement("DetalleDeclaracion",
                                            new XElement("ModalidadOperacion", new XAttribute("CodLista", esCanje ? "2" : "1")),
                                            new XElement("EsCompradorFinal"),
                                            new XElement("ProvinciaDestino", new XAttribute("CodLista", string.Empty)),
                                            new XElement("LocalidadDestino"),
                                            new XElement("LugarEntregaSIO", new XAttribute("CodLista", string.Empty)),
                                            new XElement("CondicionPago", new XAttribute("CodLista", string.Empty)),
                                            new XElement("OpcionFijacion", new XAttribute("CodLista", string.Empty)),
                                            new XElement("Observaciones", null)
                                        )
                                    )

                #endregion SioGranos

                                ),//Fin DetalleContrato

                #endregion DetalleContrato

                #region ExtendedData

                                new XElement("ExtendedData",
                                    new XElement("ExtendedDataItem", new XAttribute("Caption", string.Empty), new XAttribute("DataName", string.Empty))
                               ),

                #endregion ExtendedData

                #region Clausulas

                                new XElement("Clausulas",
                                    clausulasConfirma.Select(x => new XElement("Clausula", new XAttribute("Orden", string.Empty),
                                        new XElement("TextoClausula", x.Texto),
                                        new XElement("TextoAdicionalClausula", string.Empty)
                                    ))
                                )

                #endregion Clausulas

                            )//Fin Detalle Documento
                        )//Fin Nodo Documento
                    )//Fin Nodo Lote
                );
                var xml = doc.ToString().Replace("-fakexmlns", string.Empty);
                var document = XDocument.Parse(xml);
                document.Declaration = new XDeclaration("1.0", "iso-8859-1", null);
                //Fin Formateo del XML
                document.Save(ms); //Guarda el Documento en el Stream
                byte[] bytes = ms.ToArray(); //Devuelve el documento
                return bytes;
            }
            catch (Exception e)
            {
                logger.Error(e);
                return null;
            }
        }

        private static Confirma ConvertirDtoAEntidad(ConfirmaGeneradoDto tempConfirma)
        {
            return new Confirma
            {
                NegocioId = tempConfirma.NegocioId,
                ComercialId = tempConfirma.ComercialId,
                FechaGeneracion = tempConfirma.FechaGeneracion,
                IsWebService = tempConfirma.IsWebService,
                Archivo = tempConfirma.Archivo,
                Version = tempConfirma.Version,
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

        public string ObtenerNombreArchivoConfirma(string codigoSAP)
        {
            int claseNegocio = codigoSAP.TrimStart('0').Length >= 9 ? 2 : 1;
            var confirma = repositorio.Obtener<Confirma>(x => claseNegocio == 2 ? ((x.Negocio as FijacionDePrecioContrato).FijacionSAP == codigoSAP) : x.Negocio.ContratoSAP == codigoSAP);
            return confirma.Archivo;
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

        private static ConfirmaGeneradoDto DevolverDto(BasicoContrato itemNegocio, bool generado, string mensaje, bool webServicesOK = false, DateTime? fechaGeneracionUltimoConfirma = null)
        {
            return new ConfirmaGeneradoDto
            {
                NegocioSAP = itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.FijacionSAP : itemNegocio.ContratoSAP,
                Generado = generado,
                Mensaje = mensaje,
                FechaGeneracion = fechaGeneracionUltimoConfirma == null ? default(DateTime) : (DateTime)fechaGeneracionUltimoConfirma,
                IsWebService = webServicesOK,
                ContratoSAP = itemNegocio.ContratoSAP,
                FijacionSAP = itemNegocio.FijacionSAP,
                TieneFechaGeneracionUltimoConfirma = fechaGeneracionUltimoConfirma != null,
            };
        }

        private static BoletoDto ConfirmaABoletoDto(ConfirmaGeneradoDto tempConfirma)
        {
            return new BoletoDto
            {
                NegocioId = tempConfirma.NegocioId,
                Version = tempConfirma.Version,
                ComercialId = tempConfirma.ComercialId,
                FechaGeneracion = tempConfirma.FechaGeneracion,
                ContratoSAP = tempConfirma.ContratoSAP,
                FijacionSAP = tempConfirma.FijacionSAP,
                TipoBoletoId = tempConfirma.TipoBoletoId
            };
        }

        public void EnviarMailConfirma(DateTime fecha)
        {
            var includes = new List<Expression<Func<Confirma, object>>> { c => c.Negocio, c => c.Comercial };
            var confirmaDeHoy = repositorio.Listar(includes, c => c.IsWebService && DbFunctions.TruncateTime(c.FechaGeneracion) == DbFunctions.TruncateTime(fecha))
                .GroupBy(g => g.Negocio.CorredorId ?? g.Negocio.ProveedorId).ToList();

            foreach (var grupo in confirmaDeHoy)
            {
                string emailComercial = mailManager.GetEmailUserActiveDirectory(grupo.First().Comercial.IdActiveDirectory);
                var enCopia = new List<string> { emailComercial, "dataagro@molinosagro.com.ar" };
                var negocios = grupo.Select(c => c.Negocio).ToList();

                var emailProveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1,
                    x => x.ProveedorId == grupo.Key && x.Boleto == true);

                string razonSocial = grupo.FirstOrDefault().Negocio.CorredorId.HasValue ? grupo.FirstOrDefault().Negocio.Corredor.RazonSocial : grupo.FirstOrDefault().Negocio.Proveedor.RazonSocial;
                string asunto = $"Confirma Molinos Agro S.A. - {razonSocial}";
                AlternateView cuerpo = CuerpoMailConfirma(httpContextManager.ObtenerPathLogoMail(), negocios);

                mailManager.EnviarMail(emailProveedor, asunto, "", enCopia, cuerpo);
            }
        }

        private AlternateView CuerpoMailConfirma(String filePath, List<Negocio> contratos)
        {
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };

            string htmlBody = "Le informamos que ya se encuentran subidos al sistema Confirma los siguientes contratos: <br/><br/>";
            foreach (Negocio contrato in contratos)
            {
                htmlBody += $"• {contrato.ContratoSAP.TrimStart('0')} de Molinos Agro S.A.";
                if (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
                    htmlBody += " - Fijación N°" + (contrato as FijacionDePrecioContrato).FijacionSAP.TrimStart('0') + "<br/>";
                else
                    htmlBody += "<br/>";
                string bolsa = contrato.Bolsa != null ? contrato.Bolsa.Descripcion : contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (contrato as FijacionDePrecioContrato).Contrato.Bolsa.Descripcion : "-no definida-";
                htmlBody += $"&emsp;Bolsa de {bolsa}.<br/><br/>";
            }
            htmlBody += "<br/>En caso de tener alguna consulta, ingresar a www.moaoperaciones.com.ar " +
                "<br/><br/>Saludos Cordiales," +
                "<br/><br/>Molinos Agro S.A.<br/><br/>" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br/>www.molinosagro.com.ar";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);

            return alternateView;
        }

        public List<ConfirmaArchivoDto> ListarConfirmas() //Pantalla descargas
        {
            var result = repositorio.Listar<Confirma, ConfirmaArchivoDto>(a => new ConfirmaArchivoDto
            {
                Id = a.Id,
                NegocioId = a.NegocioId,
                ComercialId = a.ComercialId,
                ContratoSAP = a.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (a.Negocio as FijacionDePrecioContrato).FijacionSAP : a.Negocio.ContratoSAP,
                Nombre = a.Archivo,
                FechaGeneracion = a.FechaGeneracion,
                IsWebService = a.IsWebService,
            }).OrderByDescending(x => x.FechaGeneracion).OrderByDescending(x => x.Nombre);

            foreach (var confirma in result)
            {
                confirma.FechaGeneracionGrilla = confirma.FechaGeneracion.ToString("dd/MM/yyyy");
            }

            return result.ToList();
        }

        public List<ResultadoClausula> ObtenerClausulas(BasicoContrato basico)
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

        public string CorregirFormatoFecha(string cadena)
        {
            var date = DateTime.Parse(cadena);
            return date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        private List<string> CompletarCodigoLista(List<string> lista)
        {
            var result = new List<string>();
            foreach (var item in lista)
            {
                result.Add(item.TrimStart('0').PadLeft(10, '0'));
            }
            return result;
        }

        public List<string> ObtenerClausulasPorNegocio(string contratoSap, List<int> equipo)
        {
            contratoSap = contratoSap.TrimStart('0').PadLeft(10, '0');
            List<string> clausulas = new List<string>();
            var contratos = new List<string> { contratoSap };
            var basicoContrato = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, equipo)).FirstOrDefault();
            clausulas = ObtenerClausulas(basicoContrato).Select(x => x.Texto).ToList();
            return clausulas;
        }

        public string ValidarNegocio(string negocioSAP, List<int> equipo)
        {
            var contratoSap = negocioSAP.TrimStart('0').PadLeft(10, '0');
            var contratos = new List<string> { contratoSap };
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(contratos, equipo));
            var basicoContrato = consulta.FirstOrDefault();
            var mensaje = basicoContrato is null ? "No encontrado" : ValidarContrato(basicoContrato);
            return mensaje;
        }

        private List<BasicoContrato> ObtenerContratos(List<string> codigos, int claseNegocio, List<int> equipo)
        {
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, equipo));
            if (consulta == null) logger.Info($"Generacion Confirma: El resultado de la consulta es nulo");
            else logger.Info($"Generacion Confirma: Del resultado de la consulta, la longitud es {consulta.Count()}");
            return FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
        }

        private string ObtenerEstadoBoleto(BasicoConfirma boleto)
        {
            var mensaje = boleto.Estado_Version;
            try
            {
                var consultaBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(boleto.ContratoSAP, boleto.FijacionSAP ?? string.Empty);
                var version = Int32.Parse(consultaBoleto.Version);
                if (version > boleto.Version)
                {
                    mensaje = "Anulado";
                }
                else if (version == boleto.Version)
                {
                    if (consultaBoleto.Anulado == "X")
                    {
                        mensaje = "Anulado";
                    }
                    else if (consultaBoleto.Generado == "X" && consultaBoleto.Anulado == "")
                    {
                        mensaje = "Vigente";
                    }
                    else
                    {
                        mensaje = "Pendiente";
                    }
                }
                else if (version == 0 && consultaBoleto.Anulado == "" && consultaBoleto.Generado == "")
                {
                    mensaje = "Pendiente";
                }
                else
                {
                    logger.Info($"Generar Confirma - Listar Negocios - Error al consultar el status del contrato SAP {boleto.NegocioSAP}, Las Versiones No Coinciden.");
                }
            }
            catch (Exception ex)
            {
                logger.Info($"Generar Confirma - Listar Negocios - Error al consultar el status del contrato SAP {boleto.NegocioSAP}, Mensaje: {ex.Message}.");
            }

            return mensaje;
        }
        public List<string> ListarComerciales()
        {
            return repositorio.Listar<Comercial>()
                .Select(x => x.Nombres + " " + x.Apellido) // Asegúrate de que tengas la propiedad Nombre
                .ToList(); // Agrega ToList() para devolver una lista
        }

        public List<string> ListarCorredores()
        {
            return repositorio.Listar<Proveedor>()
                .Where(x => x.Segmentacion.Grupo == "Corredores") // Filtrar por el grupo
                .Select(x => x.RazonSocial) // Concatenar Nombre y Apellido
                .ToList(); // Convertir a lista
        }

        public List<string> ListarVendedores()
        {
            return repositorio.Listar<Proveedor>()
                .Where(x => x.Segmentacion.Grupo != "Corredores") // Filtrar proveedores que no son Corredores
                .Select(x => x.RazonSocial) // Concatenar Nombre y Apellido
                .ToList(); // Convertir a lista
        }

        public byte[] DescargarZipConfirmas(List<string> nombresArchivos)
        {
            // Crear un MemoryStream para almacenar el ZIP en memoria
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Crear el archivo ZIP
                using (ZipArchive zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var filePath in nombresArchivos)
                    {
                        string pathCompleto = Path.Combine(pathConfirmas, filePath);
                        // Asegúrate de que el archivo exista antes de agregarlo al ZIP
                        if (File.Exists(pathCompleto))
                        {
                            // Agregar cada archivo al ZIP
                            string fileName = Path.GetFileName(pathCompleto);

                            ZipArchiveEntry entry = zip.CreateEntry(fileName, CompressionLevel.Fastest);

                            using (var entryStream = entry.Open())
                            using (var fileStream = File.OpenRead(pathCompleto))
                            {
                                fileStream.CopyTo(entryStream);
                            }
                        }
                    }
                }

                return memoryStream.ToArray(); // Devolver los datos ZIP como byte[]
            }
        }
    }
}