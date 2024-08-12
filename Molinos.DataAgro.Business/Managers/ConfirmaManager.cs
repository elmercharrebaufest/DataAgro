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
using System.Xml.Linq;
using iTextSharp.tool.xml.html.head;
using static iTextSharp.text.pdf.AcroFields;
using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;
using System.Web.UI.WebControls;
using Molinos.DataAgro.Agent.ScatoRepositorio;

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

        public ConfirmaManager(IRepositorio repositorio, ILogger logger, IStatusContratoAgent status, IEnviarBoletoAgent oEnviarBoletoAgent, IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent, IMailManager mailManager, IHttpContextManager httpContextManager, IServicioClausulas servicioClausula)
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

        public ConfirmaResult GrabarConfirmas(int claseNegocio, int ComercialId, List<string> codigosSap, bool usarWebServiceConfirma, List<int> equipo)
        {
            var codigos = AgregarCeros(codigosSap);
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, false, equipo, new List<int>()));
            var contratos = FiltrarNegocios(consulta, ConvertirClaseNegocioATiposNegocios(claseNegocio));
            var resultado = new ConfirmaResult();
            try
            {
                if (contratos==null || contratos.Count==0)
                {
                    logger.Info($"Generacion Confirma: No se hallaron Negocios SAP {codigosSap.ToArray()} siendo los codigos completos: {codigos.ToArray()}");
                    resultado.Errores.Add(new ErrorMessage(404, "Ningun Negocio Encontrado"));
                    return resultado;
                }
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
                            TipoBoletoId = contrato.TipoNegocioId==(int)EnumTipoNegocio.FIJACION?1:contrato.BoletoId.GetValueOrDefault(),
                            IsWebService = usarWebServiceConfirma,
                            NegocioSAP = contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP:contrato.ContratoSAP,
                            Mensaje = string.Empty,
                            Generado = true
                        };
                        //Enviando Confirma a RFC como BoletoGeneradoDto
                        logger.Debug("Confirma: Enviando Boleto confirma" + tempConfirma.ToString());
                        var res = oEnviarBoletoAgent.Enviar(ConfirmaABoletoDto(tempConfirma));
                        logger.Debug("Confirma: Respuesta de la RFC" + res.ToString());
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
                        logger.Info($"Confirma.Generado = {consultaConfirma.Generado} -- contrato SAP {contrato.FijacionSAP??contrato.ContratoSAP}");
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
            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(codigos, false, equipo, new List<int>()));
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

            var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { codigoSAPcompleto }, false, equipo, new List<int>()));
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
                    var a_fijar = repositorio.Obtener<Negocio>(x => x.ContratoSAP == contrato.ContratoSAP && x.TipoNegocioId==(int)EnumTipoNegocio.A_FIJAR);
                    if (a_fijar.PlanCanje != true)
                    {
                        mensaje = $"No se puede generar el confirma {contrato.FijacionSAP} por no ser de Plan Canje el A Fijar correspondiente.";
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
                var consulta = repositorio.ObtenerConsultaEscalar(new TraerTodosContratosBoleto(new List<string>() { CodigoSapCompleto }, false, equipo, new List<int>()));
                var contrato = consulta.First();
                var confirma = repositorio.Obtener<Confirma>(x => contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? ((x.Negocio as FijacionDePrecioContrato).FijacionSAP == CodigoSapCompleto) : x.Negocio.ContratoSAP == CodigoSapCompleto);
                var a_fijar = confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? repositorio.Obtener<Negocio>(x => x.ContratoSAP == contrato.ContratoSAP && x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) : null;
                logger.Info($"Negocio de Confirma Encontrado Negocio SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");
                //Calcular datos para el XML
                var estadoSAP = status.ValidarEstado(confirma.Negocio.ContratoSAP);
                var esConvenio = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && contrato.Madre == true;
                var nroContratoInterno = (contrato.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? contrato.FijacionSAP : contrato.ContratoSAP).TrimStart('0');
                var Partes = (confirma.Negocio.CorredorId > 0) ?
                    new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = confirma.Negocio.Proveedor.CUIT, Sucursal = string.Empty }, new { CodLista = "2", NroContratoInterno = nroContratoInterno, CUIT = confirma.Negocio.Corredor.CUIT, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = "30715118773", Sucursal = string.Empty } }
                    : new[] { new { CodLista = "1", NroContratoInterno = nroContratoInterno, CUIT = confirma.Negocio.Proveedor.CUIT, Sucursal = string.Empty }, new { CodLista = "3", NroContratoInterno = nroContratoInterno + "V01", CUIT = "30715118773", Sucursal = string.Empty } };
                var datosConfirma = oConsultarEstadoBoletoAgent.EstadoBoleto(codigoSAP, string.Empty);
                var condiciones = datosConfirma.CondicionFijacion.FirstOrDefault();
                var esCanje = (a_fijar != null && a_fijar.PlanCanje == true) || contrato.PlanCanje == true;
                logger.Info($"Negocio de Confirma Datos Precargados Negocio SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");
                var clausulas = ObtenerClausulas(contrato);
                logger.Info($"Negocio de Confirma Clausulas Cargadas Negocio SAP {contrato.FijacionSAP ?? contrato.ContratoSAP}");
                //Fin de carga de datos
                MemoryStream ms = new MemoryStream(); //Memory Stream
                                                      //Inicia formateo del XML
                var doc = new XDocument(
                    new XElement("Lote",
                        new XElement("Documento", new XAttribute("xmlns-fakexmlns", "Documento"),

                #region CabeceraDocumento

                            new XElement("CabeceraDocumento",
                                new XElement("Bolsa", new XAttribute("CodLista", confirma.Negocio.Bolsa.CodigoConfirma)),
                                new XElement("TipoDocumento", new XAttribute("CodLista", esCanje ? "17" : (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "1" : (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio ? "3" : "")))),
                                new XElement("Formulario", new XAttribute("formversion", "1.04"))
                            ),//Fin Nodo CabeceraDocumento

                #endregion CabeceraDocumento

                            new XElement("UploadInfo",
                                new XElement("Workflow", confirma.Negocio.CorredorId > 0 ? "4" : "7")
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
                                    new XElement("Producto", new XAttribute("CodLista", confirma.Negocio.MaterialId == (int)EnumMateriales.TRIGO ? "1" : confirma.Negocio.MaterialId == (int)EnumMateriales.MAIZ ? "2" : confirma.Negocio.MaterialId == (int)EnumMateriales.SORGO ? "3" : confirma.Negocio.MaterialId == (int)EnumMateriales.GIRASOL ? "20" : confirma.Negocio.MaterialId == (int)EnumMateriales.SOJA ? "21" : string.Empty)),
                                    new XElement("DescAdicional", esCanje ? "INSUMO" : null),
                                    new XElement("FechaConcertacion", confirma.Negocio.FechaOperacion.ToString("dd/MM/yyyy")),
                                    new XElement("Cosecha", new XAttribute("CodLista", confirma.Negocio.Campana.CodigoSIO)),
                                    new XElement("UnidadMedida", new XAttribute("CodLista", "K")),
                                    new XElement("CantidadDesde", confirma.Negocio.KgMinimo?? (int)confirma.Negocio.Cantidad),
                                    new XElement("CantidadHasta", confirma.Negocio.KgMaximo?? (int)confirma.Negocio.Cantidad),
                                    new XElement("Ajuste", new XAttribute("CodLista", string.Empty)),
                                    new XElement("CantCamiones", confirma.Negocio.CantidadCamiones),
                                    (esCanje || confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? new XElement("MontoImponible") : null),
                                    new XElement("Moneda", new XAttribute("CodLista", confirma.Negocio.Moneda is null ? string.Empty : (confirma.Negocio.Moneda.Descripcion == "ARP" ? "1" : "2"))),
                                    (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("Precio", confirma.Negocio.Precio) : null),
                                    (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? new XElement("UnidadMedidaPrecio", new XAttribute("CodLista", "T")) : null),//SOLO en A_PRECIO?
                                    (confirma.Negocio.CorredorId > 0 ? new XElement("PorcComisionComprador", confirma.Negocio.PorcentajeComision>0? confirma.Negocio.PorcentajeComision:null) : null),

                #region Calidad

                                    new XElement("Calidad",
                                        new XElement("CondicionesCalidad", new XAttribute("CodLista", (confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.CAMARA || confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.ESPECIAL) ? "1" : (confirma.Negocio.StandardDeCalidad?.Id == (int)EnumStandarCalidad.FABRICA ? "4" : ""))),
                                        new XElement("OtrasCondicionesCalidad")
                                    ),

                #endregion Calidad

                                    new XElement("MedioTransporte", new XAttribute("CodLista", "C")),

                #region Entregas

                                    new XElement("Entregas",
                                        new XElement("EntregaDesde", confirma.Negocio.FechaDesde.ToString("dd/MM/yyyy")),
                                        new XElement("EntregaHasta", confirma.Negocio.FechaHasta.ToString("dd/MM/yyyy"))
                                    ),

                #endregion Entregas

                #region Origen

                                    new XElement("Origen",
                                        new XElement("LocalidadOrigen", confirma.Negocio.Localidad.CodLocalidad),
                                        new XElement("ProvinciaOrigen", new XAttribute("CodLista", confirma.Negocio.Provincia.CodigoConfirma))
                                    ),

                #endregion Origen

                                    new XElement("Destino", new XAttribute("CodLista", confirma.Negocio.Destino.CodigoConfirma.ToString()), new XAttribute("CodPrv", "0000")),

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
                                                (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || esConvenio) ?
                                                    "4 días hábiles de fecha de fijación" : (
                                                    (confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ? (
                                                        (confirma.Negocio.CD == true) ?
                                                            "Pago Contra CD" : (
                                                            (confirma.Negocio.Warrant == true) ?
                                                                "Pago contra Warrant" : (
                                                                (confirma.Negocio.PagoDiferido == true) ?
                                                                    "72 hrs contra mercadería entregada" :
                                                                    "Días de diferimiento contra mercadería entregada"
                                                                )
                                                            )
                                                        ) : null
                                                    )
                                                )
                                                : null
                                            ),
                                            new XElement("LugarPago", "BUENOS AIRES"),
                                            new XElement("PagoAOrdenDe", new XAttribute("CodLista", confirma.Negocio.CorredorId > 0 ? (confirma.Negocio.PagoDirectoVendedor == true ? "1" : "2") : "1")),
                                            new XElement("PorcPago", confirma.Negocio.PorcentajeDePago.HasValue ? confirma.Negocio.PorcentajeDePago : null)
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
                                            new XElement("PrecioTotal", confirma.Negocio.Monto),
                                            new XElement("Factura"),
                                            new XElement("PorcentajeGastos"),
                                            new XElement("TipoCambioPesos"),
                                            new XElement("LugarEntrega"),
                                            new XElement("ProvinciaEntrega", confirma.Negocio.Destino.Localidad.Provincia.CodigoConfirma)
                                        ) : null),

                #endregion Insumos

                #region Fijacion

                                        ((confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION || confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR) ? new XElement("Fijacion",
                                            new XElement("FijMinima", Convert.ToInt32(condiciones.CantidadMinima).ToString()),
                                            new XElement("FijMaxima", Convert.ToInt32(condiciones.CantidadMaxima).ToString()),
                                            new XElement("UnidadMedidaFijacion", new XAttribute("Caption", "K"), new XAttribute("CodLista", "K")),
                                            new XElement("FijPeriodo", "1"),
                                            new XElement("FijFecDesde", CorregirFormatoFecha(condiciones.FechaDesde)),
                                            new XElement("FijFecHasta", CorregirFormatoFecha(condiciones.FechaHasta)),
                                            new XElement("PorcMultaIncumplimiento", "010"),
                                            new XElement("ComunicacionFijacion", new XAttribute("CodLista", confirma.Negocio.PagoDirectoVendedor == true ? "2" : "1"))
                                        ) : null),

                #endregion Fijacion

                                    new XElement("ProduccionVendedor", new XAttribute("CodLista", confirma.Negocio.ClasificacionId == (int)EnumClasificacionCompraNet.Productor ? (confirma.Negocio.PagoDirectoVendedor == true ? "1" : "4") : (confirma.Negocio.Consignatario == true ? "5" : "2"))),

                #region APRECIO

                                    ((confirma.Negocio.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO) ?
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
                ContratoSAP = itemNegocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? itemNegocio.FijacionSAP:itemNegocio.ContratoSAP,
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
                Version = 1,
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
            List<string> correos = new List<string>() { "dataagro@baufest.com" };
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

                    var correoproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == ((item.Negocio.CorredorId != null && item.Negocio.CorredorId > 0) ? item.Negocio.CorredorId : item.Negocio.ProveedorId) && x.Boleto == true);

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
                (a => new ConfirmaArchivoDto
                {
                    Id = a.Id,
                    NegocioId = a.NegocioId,
                    ComercialId = a.ComercialId,
                    ContratoSAP = a.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (a.Negocio as FijacionDePrecioContrato).FijacionSAP : a.Negocio.ContratoSAP,
                    Nombre = "confirma" + a.FechaGeneracion.Year + (a.FechaGeneracion.Month > 9 ? "" : "0") + a.FechaGeneracion.Month + (a.FechaGeneracion.Day > 9 ? "" : "0") + a.FechaGeneracion.Day + "_" + a.Negocio.ContratoSAP + (a.Negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? "_" + (a.Negocio as FijacionDePrecioContrato).FijacionSAP : string.Empty) + ".XML",
                    FechaGeneracion = a.FechaGeneracion.Day + "/" + a.FechaGeneracion.Month + "/" + a.FechaGeneracion.Year,
                    IsWebService = a.IsWebService,
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
                result.Add(item.TrimStart('0').PadLeft(10, '0'));
            }
            return result;
        }
    }
}