using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteStopV2Agent : IClienteStopV2Agent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly string urlStop = ConfigurationManager.AppSettings["UrlBaseSTOP"];
        private readonly Func<ICupoManager> cupoManagerInj;
        private readonly string versionApi = ConfigurationManager.AppSettings["VersionClienteSTOP"];
        private readonly int nroPlantaRuca = int.Parse(ConfigurationManager.AppSettings["nroPlantaRuca"]);

        public ClienteStopV2Agent(ILogger logger, IRepositorio repositorio, Func<ICupoManager> cupoManagerInj,
            ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
            this.cupoManagerInj = cupoManagerInj;
        }

        public TokenStop ObtenerToken(string clave)
        {
            try
            {
                // Create a new token
                var token = CreateTokenAsync(clave);
                return token;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        private void LogError(Exception e)
        {
            if (string.IsNullOrEmpty(e.Message))
            {
                logger.Error(e.Message);
                LogError(e.InnerException);
            }
        }

        private TokenStop CreateTokenAsync(string clave)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(urlStop)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            try
            {
                HttpResponseMessage response = client.GetAsync(
                    $"v{versionApi}/auths/{clave}").Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsAsync<dynamic>().Result;
                var jObject = JObject.Parse(res.ToString());

                ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                if (!respuesta.isError)
                {
                    TokenStop r = JsonConvert.DeserializeObject<TokenStop>(jObject.ToString());
                    return r;
                }
                else
                {
                    ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject.data.ToString());
                    logger.Error("ObtenerToken Error al obtener token");
                    logger.Debug(error.ToJson());
                    throw new Exception(error.userMessage);
                }
            }
            catch (Exception e)
            {
                LogError(e);
                throw;
            }
        }

        public void CrearCupo(List<string> cupos)
        {
            try
            {
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var listaCupos = repositorio.Listar<Cupo>(x => cupos.Contains(x.CupoSap));
                foreach (var cupo in listaCupos)
                {
                    if (cupo.CupoStop != null)
                    {
                        cupo.EstadoCupoId = 1;
                    }
                    else
                    {
                        var cupoStop = new CupoStop
                        {
                            //cuitOrigen = null,//cupo.CuitOrigen,//preguntar
                            //cuitIntermediario = "",//preguntar
                            //CuitRemComercialProductor = "",//preguntar
                            //CuitCorredorVentaSecundaria = "",//preguntar
                            CuitCorredorVentaSecundaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            //CuitCorredorVentaPrimaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            cuitDestinatario = cupo.Destinatario,
                            cuitDestino = datosConfiguracion.CuitDestinoStop,
                            idCupoTerminal = cupo.CupoSap,
                            idTerminal = datosConfiguracion.TerminalStopId,
                            fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaGeneracion.ToString("HH:mm:ss"),
                            codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                            desvio = "N",
                            //nroPlantaRucaOrigen = 0, //preguntar
                            nroPlantaRuca = nroPlantaRuca,
                            codGrano = cupo.Material.CodigoEspecie.Value
                        };
                        dynamic jObject;
                        ResultadoStop respuesta;
                        ExecuteStopRequest($"v{versionApi}/turnos/", HttpMethod.Post, cupoStop, out jObject, out respuesta, null, datosConfiguracion);
                        if (!respuesta.isError)
                        {
                            RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                            cupo.CupoStop = model.idCupo;
                            cupo.CreacionStop = model.fecha;
                            cupo.EstadoCupoId = model.idCupoEstado;
                        }
                        else
                        {
                            ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                            cupo.ErrorStop = error.userMessage;
                            cupo.EstadoCupoId = 7;
                        }
                    }
                    repositorio.GuardarCambios();


                    var cupoManager = cupoManagerInj();
                    var cupoConId = (cupo.Id == 0) ? repositorio.Obtener<Cupo>(x => x.CupoSap == cupo.CupoSap) : cupo;
                    var cupoEnDto = cupoManager.ObtenerCupo(cupoConId.Id, null);
                    logDataAgroManager.LogCambiosDataAgro(cupoEnDto, TipoAccionLogDataAgro.Modificar);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        public void TransmitirJobCupos()
        {
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionConsultaStop.HasValue && datosConfiguracion.ConexionConsultaStop.Value)
            {
                try
                {
                    var hoy = DateTime.Now.Date;
                    var listaCupos = repositorio.Listar<Cupo>(x => x.EstadoCupoId == 6 || x.EstadoCupoId == 7 &&
                    !x.Centro.Acopio &&
                    x.FechaIngreso >= hoy, 100);

                    foreach (var cupo in listaCupos)
                    {
                        var cupoStop = new CupoStop
                        {
                            //cuitOrigen = null,//cupo.CuitOrigen,//preguntar
                            //cuitIntermediario = "",//preguntar
                            //CuitRemComercialProductor = "",//preguntar
                            //CuitCorredorVentaSecundaria = "",//preguntar
                            CuitCorredorVentaSecundaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            //CuitCorredorVentaPrimaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            cuitDestinatario = cupo.Destinatario,
                            cuitDestino = datosConfiguracion.CuitDestinoStop,
                            idCupoTerminal = cupo.CupoSap,
                            idTerminal = datosConfiguracion.TerminalStopId,
                            fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaGeneracion.ToString("HH:mm:ss"),
                            codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                            desvio = "N",
                            //nroPlantaRucaOrigen = 0, //preguntar
                            nroPlantaRuca = nroPlantaRuca,
                            codGrano = cupo.Material.CodigoEspecie.Value
                        };
                        dynamic jObject;
                        ResultadoStop respuesta;
                        ExecuteStopRequest($"v{versionApi}/turnos/", HttpMethod.Post, cupoStop, out jObject, out respuesta);
                        if (!respuesta.isError)
                        {
                            RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                            cupo.CupoStop = model.idCupo;
                            cupo.CreacionStop = model.creado;
                            cupo.EstadoCupoId = model.idCupoEstado;
                        }
                        else
                        {
                            ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                            cupo.ErrorStop = error.userMessage;
                            cupo.EstadoCupoId = 7;
                        }
                    }
                    repositorio.GuardarCambios();

                    var cupoManager = cupoManagerInj();
                    foreach (var cupoNuevo in listaCupos)
                    {
                        var cupoConId = (cupoNuevo.Id == 0) ? repositorio.Obtener<Cupo>(x => x.CupoSap == cupoNuevo.CupoSap) : cupoNuevo;
                        logDataAgroManager.LogCambiosDataAgro(cupoManager.ObtenerCupo(cupoConId.Id, null), TipoAccionLogDataAgro.Modificar);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

        public Resultado EliminarCupo(Cupo cupo, TokenStop tokenNuevo = null, RepositorioEF repo = null)
        {
            var r = repo ?? repositorio;
            Resultado resultado = new Resultado();
            try
            {
                logger.Debug("Eliminar Cupo en STOP  inicio: " + cupo.CupoSap ?? "");
                Configuracion datosConfiguracion = r.Obtener<Configuracion>(1);
                logger.Debug("datosConfiguracion: " + (datosConfiguracion == null ? "null" : "CORRECTOS"));
                TokenStop token;

                if (tokenNuevo == null)
                    token = ObtenerToken(datosConfiguracion.ClaveStop);
                else
                    token = tokenNuevo;

                dynamic jObject;
                ResultadoStop respuesta;
                ExecuteStopRequest($"v{versionApi}/turnos/{datosConfiguracion.TerminalStopId}/{cupo.CupoStop.Value}", HttpMethod.Delete, null, out jObject, out respuesta, token, datosConfiguracion);

                if (!respuesta.isError)
                {
                    dynamic model = JsonConvert.DeserializeObject<dynamic>(jObject["data"].ToString());
                    if (model == null)
                    {
                        logger.Debug("Eliminar Cupo Stop ok: ", jObject["data"].ToString());
                        return resultado;
                    }
                    else
                    {
                        logger.Debug("Error al Modificar en STOP linea 307" + cupo.CupoSap);
                        resultado.Error("", "Error al Modificar en STOP");
                        return resultado;
                    }
                }
                else
                {
                    ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                    logger.Debug("Error al Modificar en STOP  linea 315" + cupo.CupoSap);
                    logger.Debug(error.ToJson());
                    // Si el Cupo ya fue anulado en STOP, no lo toma como error y podrá anular en SAP.
                    if (error.errorCode == "10011") return resultado;
                    resultado.Error(error.errorCode, error.userMessage);
                    return resultado;
                }
            }
            catch (Exception e)
            {
                logger.Debug("Error Eliminar Cupo en STOP  linea 332: " + cupo.CupoSap ?? "");
                logger.Error(e);
                resultado.Error("", "Error Eliminar Cupo en STOP  linea 334: " + cupo.CupoSap ?? "");
                return resultado;
            }
        }

        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            logger.Debug("Iniciando consulta ConsultarCuposDiarios");
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionConsultaStop.HasValue && datosConfiguracion.ConexionConsultaStop.Value)
            {
                try
                {
                    var fechas = repositorio.Listar<Cupo, DateTime>(x => x.FechaIngreso,
                        x => x.EstadoCupoId != (int)EnumEstadoCupo.Anulado &&
                             //x.EstadoCupoId != (int)EnumEstadoCupo.Arribado && 
                             x.EstadoCupoId != (int)EnumEstadoCupo.Disponible &&
                             !x.Centro.Acopio);

                    DateTime tresDiasAtras = DateTime.Now.AddDays(-3).Date;
                    fechas = fechas.Where(x => x >= tresDiasAtras).ToList();

                    ConsultaCuposStop listaCuposStop;

                    listaCuposStop = ObtenerDatosDeStop(fechas, datosConfiguracion.TerminalStopId);

                    IEnumerable<Cupo> actualizarCupos = listaCuposStop.results.Select(cupo => new Cupo
                    {
                        EstadoPlanta = cupo.estadoEnPlanta,
                        CTGFechaDesde = !String.IsNullOrEmpty(cupo.fechaCTG_Desde) ? DateTime.ParseExact(cupo.fechaCTG_Desde, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null,
                        CTGFechaHasta = !String.IsNullOrEmpty(cupo.fechaCTG_Hasta) ? DateTime.ParseExact(cupo.fechaCTG_Hasta, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null,
                        RemitenteComercial = cupo.cuitRemComercial,
                        CorredorComprador = cupo.cuitCorredorCAfip,
                        CorredorVendedor = cupo.cuitCorredorVAfip,
                        MercadoATermino = cupo.cuitMercadoATerminoAfip,
                        Cosecha = cupo.cosecha,
                        IntermediarioFlete = cupo.cuitIntermediarioFleteAfip,
                        Transportista = cupo.cuitTransportistaAfip,
                        Chofer = cupo.cuitChoferAfip,
                        Km = cupo.kmRecorrer,
                        Peso = cupo.pesoNetoEstimado,
                        CartaPorte = cupo.cartaPorte,
                        CTG = cupo.ctg,
                        CuitOrigen = cupo.cuitOrigen,
                        CuitOrigenAfip = cupo.cuitOrigenAfip,
                        CodLocalidadOrigen = cupo.codLocalidadOrigen,
                        NroEstablecimientoOrigen = cupo.nroEstablecimientoOrigen,
                        EstadoCupoId = cupo.idCupoEstado,
                        CupoSap = cupo.idCupoTerminal,
                        CupoStop = cupo.idCupo,
                        CreacionStop = cupo.creado
                    });
                    var columnas = new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string> ("EstadoPlanta", "EstadoPlanta"),
                        new KeyValuePair<string, string> ("CTGFechaDesde", "CTGFechaDesde"),
                        new KeyValuePair<string, string> ("CTGFechaHasta", "CTGFechaHasta"),
                        new KeyValuePair<string, string> ("RemitenteComercial", "RemitenteComercial"),
                        new KeyValuePair<string, string> ("CorredorComprador", "CorredorComprador"),
                        new KeyValuePair<string, string> ("CorredorVendedor", "CorredorVendedor"),
                        new KeyValuePair<string, string> ("MercadoATermino", "MercadoATermino"),
                        new KeyValuePair<string, string> ("Cosecha", "Cosecha"),
                        new KeyValuePair<string, string> ("IntermediarioFlete", "IntermediarioFlete"),
                        new KeyValuePair<string, string> ("Transportista", "Transportista"),
                        new KeyValuePair<string, string> ("Chofer", "Chofer"),
                        new KeyValuePair<string, string> ("Km", "Km"),
                        new KeyValuePair<string, string> ("Peso", "Peso"),
                        new KeyValuePair<string, string> ("CartaPorte", "CartaPorte"),
                        new KeyValuePair<string, string> ("CTG", "CTG"),
                        new KeyValuePair<string, string> ("CuitOrigen", "CuitOrigen"),
                        new KeyValuePair<string, string> ("CuitOrigenAfip", "CuitOrigenAfip"),
                        new KeyValuePair<string, string> ("NroEstablecimientoOrigen", "NroEstablecimientoOrigen"),
                        new KeyValuePair<string, string> ("EstadoCupoId", "EstadoCupoId"),
                        new KeyValuePair<string, string> ("CupoSap", "CupoSap"),
                        new KeyValuePair<string, string> ("CupoStop", "CupoStop"),
                        new KeyValuePair<string, string> ("CreacionStop", "CreacionStop"),
                    };
                    string where = " where T.EstadoCupoId <> 4 and T.EstadoCupoId <> 5 and T.EstadoCupoId <> 8 ";

                    var cuposSapStop = actualizarCupos.Select(a => a.CupoSap).ToList();

                    List<CupoDto> cuposModificados = repositorio.ListarCupoConsultaCuposDiarios(cuposSapStop).Select(x => new CupoDto { Id = x.Id, EstadoCupoId = x.EstadoCupoId, CupoSap = x.CupoSap }).ToList();
                    logger.Debug("consulta ConsultarCuposDiarios cantidad:" + cuposModificados.Count);

                    repositorio.ActualizarTodos(actualizarCupos, columnas, "CupoSap", where);

                    repositorio.GuardarCambios();
                    logger.Debug("Fin consulta ConsultarCuposDiarios. Fechas: " + string.Join(", ", fechas.Select(a => a.ToString("yyyy/MM/dd")).ToList()));

                    return listaCuposStop.results;
                }
                catch (Exception e)
                {
                    logger.Error("Error consulta ConsultarCuposDiarios");
                    logger.Error(e);
                    throw;
                }
            }
            else
            {
                return new List<RespuestaCupoStop>();
            }
        }

        private ConsultaCuposStop ObtenerDatosDeStop(List<DateTime> fechas, int terminalId)
        {
            var listaCupos = new ConsultaCuposStop() { results = new List<RespuestaCupoStop>() };
            foreach (var fecha in fechas)
            {
                dynamic jObject;
                ResultadoStop respuesta;
                ExecuteStopRequest($"v{versionApi}/turnos/{terminalId}/fecha/{fecha.ToString("yyyy-MM-dd")}", HttpMethod.Get, null, out jObject, out respuesta, null);

                if (!respuesta.isError)
                {
                    ConsultaCuposStop model = JsonConvert.DeserializeObject<ConsultaCuposStop>(jObject["data"].ToString());
                    listaCupos.results.AddRange(model.results);
                }
                else
                {
                    ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                    logger.Debug(error.ToJson() + " fecha: " + fecha.ToString());
                }
            }

            return listaCupos;
        }

        public void ModificarCupo(Cupo cupo)
        {
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            try
            {
                var cupoStop = new CupoStop
                {

                    //cuitOrigen = null,//cupo.CuitOrigen,//preguntar
                    //cuitIntermediario = "",//preguntar
                    //CuitRemComercialProductor = "",//preguntar
                    CuitCorredorVentaSecundaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                    //CuitCorredorVentaPrimaria = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                    cuitDestinatario = cupo.Destinatario,
                    cuitDestino = datosConfiguracion.CuitDestinoStop,
                    idCupoTerminal = cupo.CupoSap,
                    idTerminal = datosConfiguracion.TerminalStopId,
                    fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaGeneracion.ToString("HH:mm:ss"),
                    codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                    desvio = "N",
                    //nroPlantaRucaOrigen = 0, //preguntar
                    nroPlantaRuca = nroPlantaRuca,
                    codGrano = cupo.Material.CodigoEspecie.Value
                };

                dynamic jObject;
                ResultadoStop respuesta;
                ExecuteStopRequest($"v{versionApi}/turnos/{cupo.CupoStop}", HttpMethod.Post, cupoStop, out jObject, out respuesta, null, datosConfiguracion);

                if (!respuesta.isError)
                {
                    RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                }
                else
                {
                    ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString()) ?? new ErrorStop { userMessage = "No se pudo modificar en STOP." };
                    cupo.ErrorStop = error.userMessage;
                    logger.Debug("No se pudo modificar en STOP." + cupo.CupoSap);
                    throw new Exception("Error Stop: " + error.userMessage);
                }

                repositorio.GuardarCambios();

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }

        }

        #region MisturnosActivos(CupoNoPropio)
        public List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos()
        {
            logger.Debug("Iniciando consulta ConsultarMisturnosActivos");
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionConsultaStop.HasValue && datosConfiguracion.ConexionConsultaStop.Value)
            {
                try
                {
                    DateTime fechaDesde = DateTime.Now.AddDays(-2).Date;
                    DateTime fechaHasta = DateTime.Now.AddDays(3).Date;

                    ConsultaTurnosActivosStop listaCuposStop;

                    listaCuposStop = ObtenerMisTurnosActivosDeStop(fechaDesde, fechaHasta);

                    IEnumerable<CupoNoPropio> actualizarCupoNoPropios = listaCuposStop.data.Select(cupoNoPropio => new CupoNoPropio
                    {
                        Codigo = cupoNoPropio.idCupoTerminal,
                        Estado = cupoNoPropio.idCupoEstado,
                        //Disponible = (cupoNoPropio.esAnulado == "N" && cupoNoPropio.esRechazado == "N") ? true : false,
                        FechaIngreso = Convert.ToDateTime(cupoNoPropio.fecha)
                    });
                    var columnasNoPropios = new List<KeyValuePair<string, string>> {
                        new KeyValuePair<string, string> ("Codigo", "Codigo"),
                        new KeyValuePair<string, string> ("Estado", "Estado"),
                        //new KeyValuePair<string, string> ("Disponible", "Disponible")
                    };
                    logger.Debug($"ConsultarMisTurnosActivos, Actualizar todo");
                    repositorio.ActualizarTodos(actualizarCupoNoPropios, columnasNoPropios, "Codigo");
                    logger.Debug($"ConsultarMisTurnosActivos, Actualizar todo fin");

                    logger.Debug($"ConsultarMisTurnosActivos, Actualizar cupo");

                    IEnumerable<Cupo> actualizarCupos = listaCuposStop.data.Select(cupo => new Cupo
                    {
                        EstadoPlanta = cupo.estadoEnPlanta,
                        CTGFechaDesde = !String.IsNullOrEmpty(cupo.fechaCTG_Desde) ? DateTime.ParseExact(cupo.fechaCTG_Desde, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null,
                        CTGFechaHasta = !String.IsNullOrEmpty(cupo.fechaCTG_Hasta) ? DateTime.ParseExact(cupo.fechaCTG_Hasta, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture) : (DateTime?)null,
                        RemitenteComercial = cupo.cuitRemComercial,
                        CorredorComprador = cupo.cuitCorredorCAfip,
                        CorredorVendedor = cupo.cuitCorredorVAfip,
                        MercadoATermino = cupo.cuitMercadoATerminoAfip,
                        Cosecha = cupo.cosecha,
                        IntermediarioFlete = cupo.cuitIntermediarioFleteAfip,
                        Transportista = cupo.cuitTransportistaAfip,
                        Chofer = cupo.cuitChoferAfip,
                        Km = cupo.kmRecorrer,
                        Peso = cupo.pesoNetoEstimado,
                        CartaPorte = cupo.cartaPorte,
                        CTG = cupo.ctg,
                        CuitOrigen = cupo.cuitOrigen,
                        CuitOrigenAfip = cupo.cuitOrigenAfip,
                        CodLocalidadOrigen = cupo.codLocalidadOrigen,
                        NroEstablecimientoOrigen = cupo.nroEstablecimientoOrigen,
                        EstadoCupoId = cupo.idCupoEstado,
                        CupoSap = cupo.idCupoTerminal,
                        CupoStop = cupo.idCupo,
                        CreacionStop = cupo.creado
                    });
                    var cuposSapStop = actualizarCupos.Select(a => a.CupoSap).ToList();

                    var cuposModificados = repositorio.Listar<Cupo>(x => x.FechaIngreso >= fechaDesde && x.FechaIngreso <= fechaHasta && cuposSapStop.Contains(x.CupoSap) && x.EstadoCupoId != 4);
                    logger.Debug($"ConsultarMisTurnosActivos, consulta cupo");
                    var cuposAgrupados = cuposModificados.GroupBy(a => a.CupoSap);

                    foreach (var item in cuposAgrupados)
                    {
                        var cupo = actualizarCupos.FirstOrDefault(a => a.CupoSap == item.First().CupoSap);
                        if (cupo != null)
                        {
                            var cupoEntity = item.OrderByDescending(a => a.Id).First();
                            cupoEntity.EstadoPlanta = cupo.EstadoPlanta;
                            cupoEntity.CTGFechaDesde = cupo.CTGFechaDesde;
                            cupoEntity.CTGFechaHasta = cupo.CTGFechaHasta;
                            cupoEntity.RemitenteComercial = cupo.RemitenteComercial;
                            cupoEntity.CorredorComprador = cupo.CorredorComprador;
                            cupoEntity.CorredorVendedor = cupo.CorredorVendedor;
                            cupoEntity.MercadoATermino = cupo.MercadoATermino;
                            cupoEntity.Cosecha = cupo.Cosecha;
                            cupoEntity.IntermediarioFlete = cupo.IntermediarioFlete;
                            cupoEntity.Transportista = cupo.Transportista;
                            cupoEntity.Chofer = cupo.Chofer;
                            cupoEntity.Km = cupo.Km;
                            cupoEntity.Peso = cupo.Peso;
                            cupoEntity.CartaPorte = cupo.CartaPorte;
                            cupoEntity.CTG = cupo.CTG;
                            cupoEntity.CuitOrigen = cupo.CuitOrigen;
                            cupoEntity.CuitOrigenAfip = cupo.CuitOrigenAfip;
                            cupoEntity.CodLocalidadOrigen = cupo.CodLocalidadOrigen;
                            cupoEntity.NroEstablecimientoOrigen = cupo.NroEstablecimientoOrigen;
                            cupoEntity.EstadoCupoId = cupo.EstadoCupoId;
                            cupoEntity.CupoSap = cupo.CupoSap;
                            cupoEntity.CupoStop = cupo.CupoStop;
                            cupoEntity.CreacionStop = cupo.CreacionStop;
                        }
                    }
                    logger.Debug($"ConsultarMisTurnosActivos, Actualizar cupo fin");

                    logger.Debug($"ConsultarMisTurnosActivos, GuardarCambios");
                    repositorio.GuardarCambios();

                    logger.Debug("Fin consulta ConsultarCuposDiarios. Fechas: " + fechaDesde.ToString() + " - " + DateTime.Now.ToString());

                    return listaCuposStop.data;
                }
                catch (Exception e)
                {
                    logger.Error("Error ConsultarMisTurnosActivosAgent");
                    logger.Error(e);
                    throw;
                }
            }
            else
            {
                return new List<RespuestaCupoNoPropioStop>();
            }
        }

        private ConsultaTurnosActivosStop ObtenerMisTurnosActivosDeStop(DateTime fechaDesde, DateTime fechaHasta)
        {
            try
            {
                var listaCupos = new ConsultaTurnosActivosStop() { data = new List<RespuestaCupoNoPropioStop>() };
                logger.Debug($"ObtenerMisTurnosActivosDeStop. Consultando para fechas {fechaDesde.ToString("yyyy-MM-dd")}/{fechaHasta.ToString("yyyy-MM-dd")}");
                var url = $"v{versionApi}/misturnosactivos/{fechaDesde.ToString("yyyy-MM-dd")}/{fechaHasta.ToString("yyyy-MM-dd")}";
                dynamic jObject;
                ResultadoStop respuesta;
                ExecuteStopRequest(url, HttpMethod.Get, null, out jObject, out respuesta);

                if (!respuesta.isError)
                {
                    ConsultaTurnosActivosStop model = JsonConvert.DeserializeObject<ConsultaTurnosActivosStop>(jObject.ToString());
                    listaCupos.data.AddRange(model.data);
                }
                else
                {
                    ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                    logger.Debug("error: " + error.ToJson() + " fechaDesde: " + fechaDesde.ToString() + " - fechaHasta: " + fechaHasta.ToString());
                }
                logger.Debug($"ObtenerMisTurnosActivosDeStop. Fin Consulta para fechas {fechaDesde.ToString("yyyy-MM-dd")}/{fechaHasta.ToString("yyyy-MM-dd")}");
                return listaCupos;
            }
            catch (Exception e)
            {
                logger.Error("ERROR ObtenerMisTurnosActivosDeStop" + " fechaDesde: " + fechaDesde.ToString() + " - fechaHasta: " + fechaHasta.ToString());
                logger.Error(e);
                throw;
            }
        }

        private void ExecuteStopRequest(string url, HttpMethod httpMethod, object data, out dynamic jObject, out ResultadoStop respuesta, TokenStop tokenNuevo = null, Configuracion datosConfiguracion = null)
        {
            if (datosConfiguracion == null)
                datosConfiguracion = repositorio.Obtener<Configuracion>(1);

            TokenStop token;
            if (tokenNuevo == null)
                token = ObtenerToken(datosConfiguracion.ClaveStop);
            else
                token = tokenNuevo;

            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri(urlStop)
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            var request = new HttpRequestMessage(httpMethod, url);

            if (data != null)
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = content;
            }

            client.DefaultRequestHeaders.Add("token", token.Data);
            HttpResponseMessage response = client.SendAsync(request).Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            jObject = JObject.Parse(res.ToString());
            respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
        }
        #endregion
    }
}
