using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Net.Mail;
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

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status, IEnviarBoletoAgent oEnviarBoletoAgent,
            IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent, IMailManager mailManager, IHttpContextManager httpContextManager,
            IServicioClausulas servicioClausula, IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent, IConfirmaLoteDocumentosAgent confirmaLoteDocumentosAgent)
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
        }

        public DatosIniContrato TraerDatosCombos()
        {
            var datosCombo = new DatosIniContrato();
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 1, Descripcion = "Contrato" });
            datosCombo.clasenegocio.Add(new ClaseNegocioQry { ClaseNegocioId = 2, Descripcion = "Fijación" });
            return datosCombo;
        }

        public ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo)
        {
            var codigos = AgregarCeros(codigosSap);
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, equipo));
            if (consulta == null) logger.Info($"Generacion Confirma: El resultado de la consulta es nulo");
            else logger.Info($"Generacion Confirma: Del resultado de la consulta, la longitud es {consulta.Count()}");
            var contratos = FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
            var resultado = new ConfirmaResult();
            string activarConfirmaWS = ConfigurationManager.AppSettings["ActivarConfirmaWS"];
            string ambienteLocal = ConfigurationManager.AppSettings["AmbienteLocal"];
            string pruebaRapidaDeConfirma = ConfigurationManager.AppSettings["PruebaRapidaDeConfirma"];

            if (usarWebServiceConfirma && (ambienteLocal == "1" || pruebaRapidaDeConfirma == "1"))
            {
                //confirmaConsultaDocumentosAgent.ConsultaDocumentos(1, "1");

                string CodigoSapCompleto = codigos[0].TrimStart('0').PadLeft(10, '0');
                IQueryable<BasicoContrato> consultaIQ = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { CodigoSapCompleto }, equipo));
                BasicoContrato contrato = consultaIQ.First();
                List<ResultadoClausula> clausulas = ObtenerClausulas(contrato);

                EstadosConfirmaDto estadosConfirmaDto = new EstadosConfirmaDto
                {
                    ConfirmaAltaEstadoDto = repositorio.Listar<ConfirmaAltaEstado, ConfirmaAltaEstadoDto>(x => new ConfirmaAltaEstadoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstado = x.CodigoConfirmaAltaEstado }),
                    ConfirmaAltaEstadoLoteDto = repositorio.Listar<ConfirmaAltaEstadoLote, ConfirmaAltaEstadoLoteDto>(x => new ConfirmaAltaEstadoLoteDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoLote = x.CodigoConfirmaAltaEstadoLote }),
                    ConfirmaAltaEstadoDocumentoDto = repositorio.Listar<ConfirmaAltaEstadoDocumento, ConfirmaAltaEstadoDocumentoDto>(x => new ConfirmaAltaEstadoDocumentoDto { Id = x.Id, Descripcion = x.Descripcion, CodigoConfirmaAltaEstadoDocumento = x.CodigoConfirmaAltaEstadoDocumento }),
                };

                ConfirmaAltaLoteResultDto confirmaAltaLoteResult = confirmaLoteDocumentosAgent.ConfirmaLoteDocumentos(clausulas, equipo, contrato, estadosConfirmaDto);
            }

            try
            {
                if (contratos == null || contratos.Count == 0)
                {
                    logger.Info($"Generacion Confirma: No se hallaron Negocios SAP {String.Join("\n", codigosSap)} siendo los codigos completos: {String.Join("\n", codigos)}");
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
                    var mensaje = ValidarContrato(contrato, claseNegocio);
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
                            Version = "01",
                            ComercialId = ComercialId,
                            FechaGeneracion = DateTime.Now,
                            ContratoSAP = contrato.ContratoSAP,
                            FijacionSAP = contrato.FijacionSAP,
                            TipoBoletoId = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? 1 : contrato.BoletoId.GetValueOrDefault(),
                            IsWebService = usarWebServiceConfirma,
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
                            //Se Almacena en DB el nuevo Confirma
                            var nuevoConfirma = repositorio.Agregar(ConvertirDtoAEntidad(tempConfirma));
                            resultado.confirmasGenerados.Add(tempConfirma);

                            if (usarWebServiceConfirma && activarConfirmaWS == "1")
                            {
                                List<ResultadoClausula> clausulas = ObtenerClausulas(contrato);
                                ConfirmaAltaLoteResultDto confirmaAltaLoteResult = confirmaLoteDocumentosAgent.ConfirmaLoteDocumentos(clausulas, equipo, contrato, estadosConfirmaDto);
                                confirmaAltaLoteResult.altaItem?.ForEach(x => x.altaErrores?.ForEach(y => resultado.Errores.Add(new ErrorMessage("WS Confirma: " + y))));
                            }
                        }
                        else
                        {
                            resultado.confirmasGenerados.Add(DevolverDto(contrato, false, res));
                        }
                    }
                    else
                    {
                        logger.Info($"Confirma.Generado = {consultaConfirma.Generado} -- contrato SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");
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



        public List<string> ListarNegociosPorRangoCodigoSAP(int negocioDesde, int negocioHasta, int claseNegocio, List<int> equipo)
        {
            var listaNegocios = new List<string>();
            if (claseNegocio == 1)
            {
                var lista = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.EstadoId == 5);
                listaNegocios = lista.Where(x => int.Parse(x.ContratoSAP) >= negocioDesde && int.Parse(x.ContratoSAP) <= negocioHasta).Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            }
            else
            {
                var lista = repositorio.Listar<FijacionDePrecioContrato>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.FijacionSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.Cantidad >= 10000 && x.EstadoId == 5);
                listaNegocios = lista.Where(x => int.Parse(x.FijacionSAP) >= negocioDesde && int.Parse(x.FijacionSAP) <= negocioHasta).Select(x => x.FijacionSAP.TrimStart('0')).ToList();
            }
            listaNegocios.Sort();
            var result = listaNegocios.FindAll(x => ValidarNegocio(x, claseNegocio, equipo) == string.Empty);
            return result;
        }

        public List<string> ValidarNegocios(List<string> codigosSAP, int claseNegocio, List<int> equipo)
        {
            var codigos = AgregarCeros(codigosSAP);
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, equipo));
            var contratos = FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
            List<string> rechazados = new List<string>();
            foreach (var contrato in contratos)
            {
                var mensaje = ValidarContrato(contrato, claseNegocio);
                if (!string.IsNullOrEmpty(mensaje))
                {
                    rechazados.Add(mensaje);
                }
            }
            return rechazados;
        }

        public List<string> FiltrarNegociosPorFecha(string desde, string hasta, int claseNegocio, List<int> equipo)
        {
            var fechaDesde = DateTime.ParseExact(desde, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var fechaHasta = hasta == "" ? DateTime.Now : DateTime.ParseExact(hasta, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1);
            var listaNegocios = new List<string>();
            if (claseNegocio == 1)
            {
                listaNegocios = repositorio.Listar<Negocio>(x => (x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) && !string.IsNullOrEmpty(x.ContratoSAP) && x.ConfirmadoSAP == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta && x.EstadoId == 5).Select(x => x.ContratoSAP.TrimStart('0')).ToList();
            }
            else
            {
                listaNegocios = repositorio.Listar<FijacionDePrecioContrato>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && !string.IsNullOrEmpty(x.FijacionSAP) && x.ConfirmadoSAP == true && x.Canje == true && x.FechaConfirmacion >= fechaDesde && x.FechaConfirmacion <= fechaHasta && x.Cantidad >= 10000 && x.EstadoId == 5).Select(x => x.FijacionSAP.TrimStart('0')).ToList();
            }
            listaNegocios.Sort();
            var result = listaNegocios.FindAll(x => ValidarNegocio(x, claseNegocio, equipo) == string.Empty);
            return result;
        }

        public string ValidarNegocio(string codigoSAP, int claseNegocio, List<int> equipo)
        {
            var codigoSAPcompleto = codigoSAP.TrimStart('0').PadLeft(10, '0');
            var mensaje = "";

            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { codigoSAPcompleto }, equipo));
            var contrato = consulta.First();

            mensaje += ValidarContrato(contrato, claseNegocio);
            return mensaje;
        }

        private string ValidarContrato(BasicoContrato contrato, int claseNegocio)
        {
            var tiposNegocios = ConvertirClaseNegocioATiposNegocios(claseNegocio);
            var mensaje = "";
            var kilosMinimos = 10000;

            if (contrato != null)
            {
                //Validar que corresponda la clase de negocio
                if (!tiposNegocios.Contains(contrato.TipoNegocioId))
                {
                    mensaje = $"No se puede generar el confirma {contrato.ContratoSAP}. Ha seleccionado el tipo incorrecto.";
                    logger.Debug($"No se puede generar el confirma el confirma {contrato.ContratoSAP}. Ha seleccionado el tipo incorrecto.");
                    return mensaje;
                }

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

        public byte[] ConfirmaEnByte(string codigoSAP, List<int> equipo)
        {
            try
            {
                //Cargar Datos
                var CodigoSapCompleto = codigoSAP.TrimStart('0').PadLeft(10, '0');
                var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { CodigoSapCompleto }, equipo));
                var contrato = consulta.FirstOrDefault();
                if (consulta is null || contrato is null) throw new ArgumentNullException(paramName: "BasicoContrato", message: $"Descargar XML Confirma - Error al recuperar BasicoContrato con CodigoSAP: {CodigoSapCompleto}");
                else logger.Info($"Se Recupera BasicoContrado con CodigoSAP {CodigoSapCompleto}");
                var existeConfirma = repositorio.Existe<Confirma>(x => contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? ((x.Negocio as FijacionDePrecioContrato).FijacionSAP == CodigoSapCompleto) : x.Negocio.ContratoSAP == CodigoSapCompleto);
                if (existeConfirma is false) throw new ArgumentNullException("Confirma", $"Descargar XML Confirma - No existe el confirma asociado al contrato: {CodigoSapCompleto}");
                logger.Info($"Datos del Negocio de Confirma Precargados; BolsaConfirma:{contrato.BolsaConfirma}, CorredorId:{contrato.CorredorId}, TipoNegocioId:{contrato.TipoNegocioId}, MaterialId:{contrato.MaterialId}, FechaOperacion:{contrato.FechaOperacion}, CampañaConfirma: {contrato.CampanaConfirma}, KgMaximo:{contrato.KgMaximo}, KgMinimo:{contrato.KgMinimo}, Cantidad:{contrato.Cantidad}, CantidadCamiones:{contrato.CantidadCamiones}, Moneda:{contrato.Moneda}, Precio:{contrato.Precio}, PorcentajeComision:{contrato.PorcentajeComision}, StandardDeCalidadId:{contrato.StandardDeCalidadId}, FechaDesde:{contrato.FechaDesde}, FechaHasta:{contrato.FechaHasta}, LocalidadConfirma:{contrato.LocalidadConfirma}, ProvinciaConfirma:{contrato.ProvinciaConfirma}, DestinoConfirma:{contrato.DestinoConfirma}, CD:{contrato.CD}, Warrant:{contrato.Warrant}, PagoDiferido:{contrato.PagoDiferido}, PagoDirectoVendedor:{contrato.PagoDirectoVendedor}, PorcentajeDePago:{contrato.PorcentajeDePago}, Monto:{contrato.Monto}, Pizarra:{contrato.Pizarra}, ClasificacionId:{contrato.ClasificacionId}");
                //Calcular datos para el XML
                var estadoSAP = status.ValidarEstado(contrato.ContratoSAP);
                if (estadoSAP is null) throw new ArgumentNullException("EstadoSAP", $"Descargar XML Confirma - Error al consultar el estadoSAP asociado al contrato: {CodigoSapCompleto}");
                else logger.Info($"Descargar XML Confirma - Se Consulta el status del contrato SAP {CodigoSapCompleto} resultando STATUS: {estadoSAP.Status} y Mensaje: {estadoSAP.Mensaje}");
                var datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(contrato.ContratoSAP, contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : "");
                var condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
                if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
                {
                    if (datosConfirma is null || condiciones is null) throw new ArgumentNullException("Error CondicionFijacion", $"Descargar XML Confirma - Se consultó el Estado del Boleto SAP del contrato {CodigoSapCompleto} y no tiene condiciones de fijacion asociadas.");
                    else logger.Info($"Descargar XML Confirma - Se consultó el Estado del Boleto SAP del contrato {CodigoSapCompleto}. Resultando las condiciones fijacion: CantidadMaxima: {condiciones.CantidadMaxima} y CantidadMinima: {condiciones.CantidadMinima}.");
                }
                var CuitMolinos = ConfigurationManager.AppSettings["Cuit"];
                var esConvenio = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && contrato.Madre == true;
                var esCanje = contrato.Canje == true;
                var nroContratoInterno = (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP).TrimStart('0');
                var Partes = (contrato.CorredorId > 0) ?
                    new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = contrato.Cuit, Sucursal = string.Empty }, new { CodLista = "2", NroContratoInterno = nroContratoInterno, CUIT = contrato.CUITCorredor, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = CuitMolinos, Sucursal = string.Empty } }
                    : new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = contrato.Cuit, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = CuitMolinos, Sucursal = string.Empty } };
                logger.Info($"Datos Precalculados del Negocio de Confirma; Codigo:{CodigoSapCompleto}, esCanje:{esCanje}, esConvenio:{esConvenio}, Partes: {string.Join(" - ", Partes.Select(e => "NroInterno: " + e.NroContratoInterno + " Cuit:" + e.CUIT))}.");
                var clausulas = ObtenerClausulas(contrato);
                if (clausulas is null || clausulas.Count == 0) throw new ArgumentNullException("Clausulas", $"Descargar XML Confirma - No se pudieron recuperar las clausulas asociadas al contrato: {CodigoSapCompleto}.");
                //Fin de carga de datos
                MemoryStream ms = new MemoryStream(); //Memory Stream
                                                      //Inicia formateo del XML
                var doc = new XDocument(
                    new XElement("Lote",
                        new XElement("Documento", new XAttribute("xmlns-fakexmlns", "Documento"),

                #region CabeceraDocumento

                            new XElement("CabeceraDocumento",
                                new XElement("Bolsa", new XAttribute("CodLista", contrato.BolsaConfirma)),
                                new XElement("TipoDocumento", new XAttribute("CodLista", contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : esCanje ? "17" : "")),
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
                                    new XElement("Moneda", new XAttribute("CodLista", contrato.Moneda == "ARP" ? "1" : contrato.Moneda == "USD" ? "2" : string.Empty)),
                                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("Precio", contrato.Precio) : null),
                                    (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("UnidadMedidaPrecio", new XAttribute("CodLista", "T")) : null),
                                    (contrato.CorredorId > 0 ? new XElement("PorcComisionComprador", contrato.PorcentajeComision > 0 ? contrato.PorcentajeComision : null) : null),

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
                                                                    "72 hrs contra mercadería entregada"
                                                                )
                                                            )
                                                        ) : null
                                                    )
                                                )
                                                : null
                                            ),
                                            new XElement("LugarPago", "BUENOS AIRES"),
                                            new XElement("PagoAOrdenDe", new XAttribute("CodLista", contrato.CorredorId > 0 ? (contrato.PagoDirectoVendedor == true ? "1" : "2") : "1")),
                                            new XElement("PorcPago", contrato.PorcentajeDePago.Value)
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
                                            new XElement("Moneda", new XAttribute("CodLista", string.Empty)),
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
                                            new XElement("FijMinima", Convert.ToInt32(condiciones.CantidadMinima).ToString()),
                                            new XElement("FijMaxima", Convert.ToInt32(condiciones.CantidadMaxima).ToString()),
                                            new XElement("UnidadMedidaFijacion", new XAttribute("Caption", "K"), new XAttribute("CodLista", "K")),
                                            new XElement("FijPeriodo", "1"),
                                            new XElement("FijFecDesde", CorregirFormatoFecha(condiciones.FechaDesde)),
                                            new XElement("FijFecHasta", CorregirFormatoFecha(condiciones.FechaHasta)),
                                            new XElement("PorcMultaIncumplimiento", "010"),
                                            new XElement("ComunicacionFijacion", new XAttribute("CodLista", contrato.PagoDirectoVendedor == true ? "2" : "1")),
                                            new XElement("PizarraFijacion", new XAttribute("CodLista", contrato.Pizarra == true ? "1" : ""))
                                        ) : null,

                #endregion Fijacion

                                    new XElement("ProduccionVendedor", new XAttribute("CodLista", contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (contrato.PagoDirectoVendedor == true ? "1" : "4") : (contrato.Consignatario == true ? "5" : "2"))),

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
                                            new XElement("ModalidadOperacion", new XAttribute("CodLista", string.Empty)),
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
                                    clausulas.Select(x => new XElement("Clausula", new XAttribute("Orden", string.Empty),
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
            int claseNegocio = codigoSAP.TrimStart('0').Length >= 9 ? 2 : 1;
            var CodigoSapCompleto = codigoSAP.TrimStart('0').PadLeft(10, '0');
            var confirma = repositorio.Obtener<Confirma>(x => claseNegocio == 2 ? ((x.Negocio as FijacionDePrecioContrato).FijacionSAP == CodigoSapCompleto) : x.Negocio.ContratoSAP == CodigoSapCompleto);
            var adicional = string.Empty;
            if (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION)
            {
                var fijacion = repositorio.Obtener<FijacionDePrecioContrato>(x => x.FijacionSAP == CodigoSapCompleto);
                adicional += fijacion != null ? "_" + fijacion.FijacionSAP : string.Empty;
            }
            var nombreArchivo = "confirma" + confirma.FechaGeneracion.ToString("yyyy/MM/dd").Replace("/", string.Empty) + "_" + confirma.Negocio.ContratoSAP + adicional + ".xml";
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
                ContratoSAP = itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.FijacionSAP : itemNegocio.ContratoSAP,
                Generado = generado,
                Mensaje = mensaje,
                FechaGeneracion = default(DateTime),
                IsWebService = false,
            };
        }

        private static BoletoDto ConfirmaABoletoDto(ConfirmaGeneradoDto tempConfirma)
        {
            return new BoletoDto
            {
                NegocioId = tempConfirma.NegocioId,
                Version = 1,
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

        public List<ConfirmaArchivoDto> ListarConfirmas()
        {
            return repositorio.Listar<Confirma, ConfirmaArchivoDto>
                (a => new ConfirmaArchivoDto
                {
                    Id = a.Id,
                    NegocioId = a.NegocioId,
                    ComercialId = a.ComercialId,
                    ContratoSAP = a.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (a.Negocio as FijacionDePrecioContrato).FijacionSAP : a.Negocio.ContratoSAP,
                    Nombre = "confirma" + a.FechaGeneracion.Year + (a.FechaGeneracion.Month > 9 ? "" : "0") + a.FechaGeneracion.Month + (a.FechaGeneracion.Day > 9 ? "" : "0") + a.FechaGeneracion.Day + "_" + a.Negocio.ContratoSAP + (a.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? "_" + (a.Negocio as FijacionDePrecioContrato).FijacionSAP : string.Empty) + ".XML",
                    FechaGeneracion = a.FechaGeneracion.Day + "/" + a.FechaGeneracion.Month + "/" + a.FechaGeneracion.Year,
                    IsWebService = a.IsWebService,
                }, null, 0, null, Entities.Helpers.DirOrden.Asc).OrderByDescending(x => x.FechaGeneracion)
                .Where(c => !c.IsWebService).ToList();
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

        private List<string> AgregarCeros(List<string> lista)
        {
            var result = new List<string>();
            foreach (var item in lista)
            {
                result.Add(item.TrimStart('0').PadLeft(10, '0'));
            }
            return result;
        }
    }
}