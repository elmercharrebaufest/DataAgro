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
                    var cupoEnDto = cupoManager.ObtenerCupo(cupoConId.Id);
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
                        logDataAgroManager.LogCambiosDataAgro(cupoManager.ObtenerCupo(cupoConId.Id), TipoAccionLogDataAgro.Modificar);
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
            try
            {
                var resultado = new Resultado();
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
                            return resultado;
                        }
                        else
                        {
                            resultado.Error("", "Error al Modificar en STOP");
                            return resultado;
                        }
                    }
                    else
                    {
                        ErrorStop error = JsonConvert.DeserializeObject<ErrorStop>(jObject["data"].ToString());
                        logger.Debug(error.ToJson());
                        resultado.Error(error.errorCode, error.userMessage);
                        return resultado;
                    }
                }
                else
                {
                    cupo.EstadoCupoId = estado;
                    repositorio.GuardarCambios();
                    resultado.Error("", "Cupo con CTG");
                    return resultado;
                }
            }
            catch (Exception e)
            {

                logger.Error(e.Message);
                throw e;
            }
        }
        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionConsultaStop.HasValue && datosConfiguracion.ConexionConsultaStop.Value)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(urlStop);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //logger.Debug("Iniciando consulta");
                    var fechas = repositorio.Listar<Cupo, DateTime>(x => x.FechaIngreso, x => x.EstadoCupoId != 4 && x.EstadoCupoId != 5 && x.EstadoCupoId != 8 && !x.Centro.Acopio);
                    var token = ObtenerToken(datosConfiguracion.ClaveStop);
                    var listaCupos = new ConsultaCuposStop() { results = new List<RespuestaCupoStop>() };
                    //logger.Debug("Token obtenido. Consultando para fechas " + string.Join(", ", fechas));
                    CultureInfo provider = CultureInfo.InvariantCulture;

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
                            logger.Debug(error.ToJson());
                        }
                    }

                    listaCupos.results = listaCupos.results.ToList();
                    IList<Cupo> actualizarCupos = null;
                    List<int> cuposCambioEstado = new List<int>();
                    while (listaCupos.results.Count > 0)
                    {
                        var index = listaCupos.results.Count >= 50 ? 50 : listaCupos.results.Count;
                        var lista = listaCupos.results.Take(index).ToList();
                        listaCupos.results.RemoveRange(0, index);
                        var cuposActualizados = lista.ToDictionary(x => x.idCupoTerminal);
                        actualizarCupos = repositorio.Listar<Cupo>(x => cuposActualizados.Keys.Contains(x.CupoSap));
                        foreach (var cupo in actualizarCupos)
                        {
                            if (cupo.CupoStop == null)
                            {
                                cupo.CupoStop = cuposActualizados[cupo.CupoSap].idCupo;
                                cupo.CreacionStop = cuposActualizados[cupo.CupoSap].creado;

                            }
                            if (cupo.EstadoCupoId != cuposActualizados[cupo.CupoSap].idCupoEstado ||
                                cupo.EstadoPlanta != cuposActualizados[cupo.CupoSap].estadoEnPlanta ||
                            (cupo.CTGFechaDesde != (!String.IsNullOrEmpty(cuposActualizados[cupo.CupoSap].fechaCTG_Desde) ? DateTime.ParseExact(cuposActualizados[cupo.CupoSap].fechaCTG_Desde, "yyyy-MM-ddTHH:mm:ss", provider) : (DateTime?)null))||
                            (cupo.CTGFechaHasta != (!String.IsNullOrEmpty(cuposActualizados[cupo.CupoSap].fechaCTG_Hasta) ? DateTime.ParseExact(cuposActualizados[cupo.CupoSap].fechaCTG_Hasta, "yyyy-MM-ddTHH:mm:ss", provider) : (DateTime?)null)) ||
                            cupo.RemitenteComercial != cuposActualizados[cupo.CupoSap].cuitRemComercial||
                            cupo.CorredorComprador != cuposActualizados[cupo.CupoSap].cuitCorredorCAfip||
                            cupo.CorredorVendedor != cuposActualizados[cupo.CupoSap].cuitCorredorVAfip||
                            cupo.MercadoATermino != cuposActualizados[cupo.CupoSap].cuitMercadoATerminoAfip||
                            cupo.Cosecha != cuposActualizados[cupo.CupoSap].cosecha||
                            cupo.IntermediarioFlete != cuposActualizados[cupo.CupoSap].cuitIntermediarioFleteAfip||
                            cupo.Transportista != cuposActualizados[cupo.CupoSap].cuitTransportistaAfip||
                            cupo.Chofer != cuposActualizados[cupo.CupoSap].cuitChoferAfip||
                            cupo.Km != cuposActualizados[cupo.CupoSap].kmRecorrer||
                            cupo.Peso != cuposActualizados[cupo.CupoSap].pesoNetoEstimado||
                            cupo.CartaPorte != cuposActualizados[cupo.CupoSap].cartaPorte||
                            cupo.CTG != cuposActualizados[cupo.CupoSap].ctg||
                            cupo.CuitOrigen != cuposActualizados[cupo.CupoSap].cuitOrigen||
                            cupo.CuitOrigenAfip != cuposActualizados[cupo.CupoSap].cuitOrigenAfip||
                            cupo.CodLocalidadOrigen != cuposActualizados[cupo.CupoSap].codLocalidadOrigen||
                            cupo.NroEstablecimientoOrigen != cuposActualizados[cupo.CupoSap].nroEstablecimientoOrigen
                                )
                            {
                                cuposCambioEstado.Add(cupo.Id);
                            }
                            cupo.EstadoPlanta = cuposActualizados[cupo.CupoSap].estadoEnPlanta;
                            //logger.Debug("CTGFechaDesde: ." + cuposActualizados[cupo.CupoSap].fechaCTG_Desde);
                            //logger.Debug("CTGFechaHasta" + cuposActualizados[cupo.CupoSap].fechaCTG_Hasta);
                            cupo.CTGFechaDesde = !String.IsNullOrEmpty(cuposActualizados[cupo.CupoSap].fechaCTG_Desde) ?
                                DateTime.ParseExact(cuposActualizados[cupo.CupoSap].fechaCTG_Desde, "yyyy-MM-ddTHH:mm:ss", provider) : (DateTime?)null;
                            cupo.CTGFechaHasta = !String.IsNullOrEmpty(cuposActualizados[cupo.CupoSap].fechaCTG_Hasta) ?
                                DateTime.ParseExact(cuposActualizados[cupo.CupoSap].fechaCTG_Hasta, "yyyy-MM-ddTHH:mm:ss", provider) : (DateTime?)null;
                            cupo.RemitenteComercial = cuposActualizados[cupo.CupoSap].cuitRemComercial;
                            cupo.CorredorComprador = cuposActualizados[cupo.CupoSap].cuitCorredorCAfip;
                            cupo.CorredorVendedor = cuposActualizados[cupo.CupoSap].cuitCorredorVAfip;
                            cupo.MercadoATermino = cuposActualizados[cupo.CupoSap].cuitMercadoATerminoAfip;
                            cupo.Cosecha = cuposActualizados[cupo.CupoSap].cosecha;
                            cupo.IntermediarioFlete = cuposActualizados[cupo.CupoSap].cuitIntermediarioFleteAfip;
                            cupo.Transportista = cuposActualizados[cupo.CupoSap].cuitTransportistaAfip;
                            cupo.Chofer = cuposActualizados[cupo.CupoSap].cuitChoferAfip;
                            cupo.Km = cuposActualizados[cupo.CupoSap].kmRecorrer;
                            cupo.Peso = cuposActualizados[cupo.CupoSap].pesoNetoEstimado;
                            cupo.CartaPorte = cuposActualizados[cupo.CupoSap].cartaPorte;
                            cupo.CTG = cuposActualizados[cupo.CupoSap].ctg;
                            cupo.CuitOrigen = cuposActualizados[cupo.CupoSap].cuitOrigen;
                            cupo.CuitOrigenAfip = cuposActualizados[cupo.CupoSap].cuitOrigenAfip;
                            cupo.CodLocalidadOrigen = cuposActualizados[cupo.CupoSap].codLocalidadOrigen;
                            cupo.NroEstablecimientoOrigen = cuposActualizados[cupo.CupoSap].nroEstablecimientoOrigen;
                            cupo.EstadoCupoId = cuposActualizados[cupo.CupoSap].idCupoEstado;
                        }
                    }
                    repositorio.GuardarCambios();
                    if (actualizarCupos != null)
                    {
                        var cupoManager = cupoManagerInj();
                        foreach (var cupoId in cuposCambioEstado)
                        {
                            logDataAgroManager.LogCambiosDataAgro(cupoManager.ObtenerCupo(cupoId), TipoAccionLogDataAgro.Modificar);
                        }
                    }
                    return listaCupos.results;
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
