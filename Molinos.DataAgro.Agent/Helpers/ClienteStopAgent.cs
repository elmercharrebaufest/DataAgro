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
using System.Web;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteStopAgent : IClienteStopAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        readonly String urlStop = ConfigurationManager.AppSettings["UrlBaseSTOP"];
        private readonly Func<ICupoManager> cupoManagerInj;

        public ClienteStopAgent(ILogger logger, IRepositorio repositorio, Func<ICupoManager> cupoManagerInj,
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
                //logger.Debug($"Gestionando Token...");
                var token = CreateTokenAsync(clave);
                //logger.Debug($"Token obtenido: {token.Data}");
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlStop);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                HttpResponseMessage response = client.PostAsJsonAsync(
                    $"v1.1.0/auths/{clave}", new { }).Result;
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
                    //logger.Debug(error.ToJson());
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlStop);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            try
            {
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var listaCupos = repositorio.Listar<Cupo>(x => cupos.Contains(x.CupoSap));
                var token = ObtenerToken(datosConfiguracion.ClaveStop);
                foreach (var cupo in listaCupos)
                {
                    if (cupo.CupoStop != null)
                    {
                        cupo.EstadoCupoId = 1;
                    }
                    else
                    {
                        var codigoCupo = cupo.CupoStop != null ? cupo.CupoStop.ToString() : cupo.CupoSap;

                        var cupoStop = new CupoStop
                        {
                            token = token.Data,
                            cuitDestinatario = cupo.Destinatario,
                            cuitDestino = datosConfiguracion.CuitDestinoStop,
                            cuitCorredorC = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            idCupoTerminal = cupo.CupoSap,
                            idTerminal = datosConfiguracion.TerminalStopId,
                            fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaGeneracion.ToString("HH:mm:ss"),
                            codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                            desvio = "N",
                            codGrano = cupo.Material.CodigoEspecie.Value
                        };
                        HttpResponseMessage response = client.PostAsJsonAsync(
                                "v1.1.0/turnos/", cupoStop).Result;
                        response.EnsureSuccessStatusCode();
                        var res = response.Content.ReadAsAsync<dynamic>().Result;
                        var jObject = JObject.Parse(res.ToString());

                        ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                        if (!respuesta.isError)
                        {
                            RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                            cupo.CupoStop = model.idCupo;
                            cupo.CreacionStop = model.creado;
                            cupo.EstadoCupoId = model.idCupoEstado;
                            //logger.Debug(model.ToJson());
                        }
                        else
                        {
                            ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                            cupo.ErrorStop = error.userMessage;
                            cupo.EstadoCupoId = 7;
                            //logger.Debug(error.ToJson());
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
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(urlStop);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var hoy = DateTime.Now.Date;
                    var listaCupos = repositorio.Listar<Cupo>(x => x.EstadoCupoId == 6 || x.EstadoCupoId == 7 &&
                    !x.Centro.Acopio &&
                    x.FechaIngreso >= hoy, 100);

                    var token = ObtenerToken(datosConfiguracion.ClaveStop);
                    foreach (var cupo in listaCupos)
                    {
                        var cupoStop = new CupoStop
                        {
                            token = token.Data,
                            cuitDestinatario = cupo.Destinatario,
                            cuitDestino = datosConfiguracion.CuitDestinoStop,
                            cuitCorredorC = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                            idCupoTerminal = cupo.CupoSap,
                            idTerminal = datosConfiguracion.TerminalStopId,
                            fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaGeneracion.ToString("HH:mm:ss"),
                            codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                            desvio = "N",
                            codGrano = cupo.Material.CodigoEspecie.Value
                        };
                        HttpResponseMessage response = client.PostAsJsonAsync(
                                "v1.1.0/turnos/", cupoStop).Result;
                        response.EnsureSuccessStatusCode();
                        var res = response.Content.ReadAsAsync<dynamic>().Result;
                        var jObject = JObject.Parse(res.ToString());

                        ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                        if (!respuesta.isError)
                        {
                            RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                            cupo.CupoStop = model.idCupo;
                            cupo.CreacionStop = model.creado;
                            cupo.EstadoCupoId = model.idCupoEstado;
                            //logger.Debug(model.ToJson());
                        }
                        else
                        {
                            ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                            cupo.ErrorStop = error.userMessage;
                            cupo.EstadoCupoId = 7;
                            //logger.Debug(error.ToJson());
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
        public int ConsultarCupo(string cupo, int terminalId, string token)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlStop);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(
                            $"v1.1.0/turnos/{token}/{terminalId}/{cupo}").Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            var jObject = JObject.Parse(res.ToString());
            ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
            //logger.Debug("jObject.ToString(): " + jObject.ToString());
            if (!respuesta.isError)
            {
                RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                //logger.Debug(model.ToJson());

                return model.idCupoEstado;
            }
            else
            {
                ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                logger.Debug(error.ToJson());
                throw new Exception(error.userMessage);
            }
        }

        public Resultado EliminarCupo(Cupo cupo)
        {
            var resultado = new Resultado();
            try
            {
                logger.Debug("Eliminar Cupo en STOP  inicio: " + cupo.CupoSap ?? "");
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var token = ObtenerToken(datosConfiguracion.ClaveStop);

                var codigoCupo = cupo.CupoStop != null ? cupo.CupoStop.ToString() : cupo.CupoSap;
                var estado = ConsultarCupo(codigoCupo, datosConfiguracion.TerminalStopId, token.Data);
                if (estado == 1)
                {
                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri($"{urlStop}v1.1.0/turnos/{token.Data}/{datosConfiguracion.TerminalStopId}/{cupo.CupoStop.Value}")
                    };

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();
                    var res = response.Content.ReadAsAsync<dynamic>().Result;
                    var jObject = JObject.Parse(res.ToString());

                    ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                    if (!respuesta.isError)
                    {
                        var model = JsonConvert.DeserializeObject<dynamic>(jObject["data"].ToString());
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
                        resultado.Error(error.errorCode, error.userMessage);
                        return resultado;
                    }
                }
                else
                {
                    logger.Debug("Error al Modificar en STOP linea 323" + cupo.CupoSap);
                    cupo.EstadoCupoId = estado;
                    repositorio.GuardarCambios();
                    resultado.Error("", "Cupo con CTG");
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
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(urlStop);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var fechas = repositorio.Listar<Cupo, DateTime>(x => x.FechaIngreso, x => x.EstadoCupoId != 4 && x.EstadoCupoId != 5 && x.EstadoCupoId != 8 && !x.Centro.Acopio);

                    DateTime tresDiasAtras = DateTime.Now.AddDays(-3).Date;
                    fechas = fechas.Where(x => x >= tresDiasAtras).ToList();

                    ConsultaCuposStop listaCuposStop;

                    listaCuposStop = ObtenerDatosDeStop(datosConfiguracion, client, fechas);

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
                    var cuposModificados = repositorio.Listar<Cupo, CupoDto>(x => new CupoDto { Id = x.Id, EstadoCupoId = x.EstadoCupoId, CupoSap = x.CupoSap }, x => cuposSapStop.Contains(x.CupoSap));


                    IEnumerable<int> query = from cm in cuposModificados
                                             join ac in actualizarCupos on cm.CupoSap equals ac.CupoSap
                                             where cm.EstadoCupoId != ac.EstadoCupoId
                                             select cm.Id;

                    repositorio.ActualizarTodos(actualizarCupos, columnas, "CupoSap", where);

                    repositorio.GuardarCambios();

                    //var cupoManager = cupoManagerInj();
                    //var cupos = cupoManager.ObtenerCupos(query.ToList(), null);
                    //var logs = new List<LogDataAgro>();
                    //foreach (var cupo in cupos)
                    //{
                    //    var resolver = new IgnorePropertiesResolver(new[] { "EstadoOrden" });
                    //    string descripcion = string.IsNullOrEmpty(cupo.CupoSap) ? cupo.Id.ToString() : cupo.CupoSap;

                    //    string jsonObjeto = JsonConvert.SerializeObject(cupo, new JsonSerializerSettings()
                    //    {
                    //        ContractResolver = resolver,
                    //        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    //        PreserveReferencesHandling = PreserveReferencesHandling.None,
                    //        Formatting = Formatting.Indented,
                    //    });
                    //    var logAgregado = new LogDataAgro
                    //    {
                    //        Usuario = "STOP",
                    //        Fecha = DateTime.Now,
                    //        DatoModificado = jsonObjeto,
                    //        Clase = "Cupo",
                    //        Tipo = cupo.GetType().Name,
                    //        AccionRealizada = TipoAccionLogDataAgro.Modificar.ToString(),
                    //        ClaseId = cupo.Id,
                    //        Descripcion = descripcion,
                    //    };
                    //    logs.Add(logAgregado);
                    //}
                    //repositorio.AgregarTodos(logs);
                    //repositorio.GuardarCambios();
                    logger.Debug("Fin consulta ConsultarCuposDiarios. Fechas" + string.Join(", ", fechas.Select(a => a.ToString("yyyy/MM/dd")).ToList()));

                    return listaCuposStop.results;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
            else
            {
                return new List<RespuestaCupoStop>();
            }
        }

        private ConsultaCuposStop ObtenerDatosDeStop(Configuracion datosConfiguracion, HttpClient client, List<DateTime> fechas)
        {
            CultureInfo provider;
            var token = ObtenerToken(datosConfiguracion.ClaveStop);
            var listaCupos = new ConsultaCuposStop() { results = new List<RespuestaCupoStop>() };
            //logger.Debug("Token obtenido. Consultando para fechas " + string.Join(", ", fechas));
            provider = CultureInfo.InvariantCulture;
            foreach (var fecha in fechas)
            {
                HttpResponseMessage response = client.PostAsJsonAsync(
                       $"v1.1.0/t/{token.Data}/1/f/{fecha.ToString("yyyy-MM-dd")}", new { }).Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsAsync<dynamic>().Result;
                var jObject = JObject.Parse(res.ToString());
                ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                //logger.Debug(fecha.ToShortDateString() + " " + respuesta.isError.ToString());
                if (!respuesta.isError)
                {
                    ConsultaCuposStop model = JsonConvert.DeserializeObject<ConsultaCuposStop>(jObject["data"].ToString());
                    listaCupos.results.AddRange(model.results);
                    //logger.Debug(model.results.Count);
                    //if (model.results.Count > 0)
                    //logger.Debug(String.Join(",", model.results.Select(a => a.idCupoTerminal)));
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlStop);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            var token = ObtenerToken(datosConfiguracion.ClaveStop);

            var codigoCupo = cupo.CupoStop != null ? cupo.CupoStop.ToString() : cupo.CupoSap;
            var estado = ConsultarCupo(codigoCupo, datosConfiguracion.TerminalStopId, token.Data);
            if (estado == 1)
            {
                try
                {
                    var cupoStop = new CupoStop
                    {
                        token = token.Data,
                        cuitDestinatario = cupo.Destinatario,
                        cuitDestino = datosConfiguracion.CuitDestinoStop,
                        cuitCorredorC = cupo.Proveedor.Segmentacion.Grupo == "Corredores" ? cupo.Proveedor.CUIT : null,
                        idCupoTerminal = cupo.CupoSap,
                        idTerminal = datosConfiguracion.TerminalStopId,
                        fecha = cupo.FechaIngreso.ToString("yyyy-MM-dd") + "T" + cupo.FechaIngreso.ToString("HH:mm:ss"),
                        codLocalidadDestino = datosConfiguracion.CodigoLocalidadStop,
                        desvio = "N",
                        codGrano = cupo.Material.CodigoEspecie.Value
                    };
                    HttpResponseMessage response = client.PostAsJsonAsync(
                            $"v1.1.0/turnos/{cupo.CupoStop}", cupoStop).Result;
                    response.EnsureSuccessStatusCode();
                    var res = response.Content.ReadAsAsync<dynamic>().Result;
                    var jObject = JObject.Parse(res.ToString());

                    ResultadoStop respuesta = JsonConvert.DeserializeObject<ResultadoStop>(jObject.ToString());
                    if (!respuesta.isError)
                    {
                        RespuestaCupoStop model = JsonConvert.DeserializeObject<RespuestaCupoStop>(jObject["data"].ToString());
                        //logger.Debug(model.ToJson());
                    }
                    else
                    {
                        ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                        cupo.ErrorStop = error.userMessage;
                        cupo.EstadoCupoId = 7;
                        logger.Debug(error.ToJson());
                        throw new Exception("Cupo Grabado en SAP, error Stop: " + error.userMessage);
                    }

                    repositorio.GuardarCambios();

                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }
    }
}
