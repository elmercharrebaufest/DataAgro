using NLog;
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
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Molinos.DataAgro.Agent.Helpers
{
    class ClientePrimaryAPIAgent : IClientePrimaryAPIAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly ICache cache;
        private readonly string urlBase = ConfigurationManager.AppSettings["UrlBasePrimary"];
        private readonly string user = ConfigurationManager.AppSettings["UserPrimary"];
        private readonly string pass = ConfigurationManager.AppSettings["PassPrimary"];

        public ClientePrimaryAPIAgent(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager, ICache cache)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
            this.cache = cache;
        }

        private TokenPrimary ObtenerToken()
        {
            try
            {
                logger.Debug($"Gestionando Token...");
                var token = new TokenPrimary();
                var url = $"AuthToken/AuthToken?nombreUsuario={user}&password={pass}";

                HttpClient client = new HttpClient()
                {
                    BaseAddress = new Uri(urlBase),
                };
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsJsonAsync(url, new { }).Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsAsync<dynamic>().Result;
                var jObject = JObject.Parse(res.ToString());

                token = JsonConvert.DeserializeObject<TokenPrimary>(jObject.ToString());

                logger.Debug($"Token obtenido: {JsonConvert.SerializeObject(token)}");
                return token;
            }
            catch (Exception e)
            {
                logger.Error("Error ObtenerToken: ", e.Message);
                throw;
            }
        }

        public List<AgenteCompra> ObtenerNegocios(DateTime dia)
        {
            try
            {
                var fecha = dia.ToString("yyyyMMdd");
                var token = ReuseToken();
                if (token.Code != "200")
                {
                    throw new Exception(token.ErrorMessage + ", " + token.ErrorDescription);
                }

                logger.Debug($"Obteniendo negocios MAT...");
                TradeCaptureReportResult result = GetTradeCaptureReport(token, fecha, fecha);
                logger.Debug($"Resultado obtenido: {result.ToJson()}");

                if (result.Code == "200")
                {
                    //List<string> CFICodes = result.Value.Select(a => a.Instrument.First().CFICode).Distinct().ToList();
                    List<Instrument> instruments = new List<Instrument>();

                    var instrumentos = SecurityList(token);

                    if (instrumentos.Code == "200")
                    {
                        foreach (var item2 in instrumentos.Value)
                        {
                            foreach (var item3 in item2.SecListGrp)
                            {
                                instruments.AddRange(item3.Instrument);
                            }
                        }
                    }

                    List<AgenteCompra> listaAgenteCompra = new List<AgenteCompra>();
                    var operadores = repositorio.Listar<Operador>();
                    listaAgenteCompra = result.Value
                        .Where(a => a.TrdCapRptSideGrp.Any(b => b.Account == "97500" || b.Account == "281647") &&
                                                                a.TrdType == 61 &&
                                                                a.TrdRptStatus == "0")
                        .Select(a => new AgenteCompra
                        {
                            TipoNegocioId = (int)EnumTipoNegocio.AGENTE_DE_COMPRAS,
                            Cantidad = ObtenerCantidad(a),
                            Precio = a.LastPx ?? 0,
                            Fecha = DateTime.ParseExact(a.TransactTime, "s", null),
                            FechaOperacion = DateTime.ParseExact(a.TransactTime, "s", null).Date,
                            MonedaId = a.Currency == "USD" ? "USDM " : "ARP  ",
                            EstadoId = a.TrdRptStatus == "3" ? (int)EnumEstadoContrato.Rechazado : (int)EnumEstadoContrato.Confirmado, //TrdRptStatus 0: Definitiva. 3: Anulada. 4: Transitoria.
                            DestinoId = 1,
                            Observacion = "Código de contrato MAT: " + a.TradeID == null ? "" : a.TradeID.Value.ToString(),
                            MaterialId = ObtenerMaterial(instruments, a.Instrument[0]),
                            Posicion = ObtenerPosicion(instruments, a.Instrument[0]),
                            DolarExportador = EsDolarExportador(a.Instrument[0]),
                            CampanaId = ObtenerCampania(instruments, a.Instrument[0]),
                            Operador = ObtenerOperador(a.RootParties, operadores),
                            TipoAgenteCompraId = 1, //MAT
                            ComercialId = 44,
                            ComercialCreadorId = 44, //Id DataAgro en prod
                        })
                        .Where(a => !string.IsNullOrEmpty(a.Posicion))
                        .ToList();

                    var materiales = repositorio.Listar<Material>();

                    foreach (var item in listaAgenteCompra)
                    {
                        item.Material = materiales.Where(x => x.MaterialId == item.MaterialId).SingleOrDefault();
                        item.OperadorId = item.Operador.Id;
                        item.FechaDesde = new DateTime(int.Parse(item.Posicion.Substring(3, 4)), int.Parse(item.Posicion.Substring(0, 2)), 1);
                        item.FechaHasta = item.FechaDesde.AddMonths(1).AddDays(-1);
                    }
                    logger.Debug($"Lista negocios AgenteCompra API: {listaAgenteCompra.Select(a => new { a.MaterialId, a.Operador.Descripcion, a.Cantidad, a.Precio, a.MonedaId, a.DolarExportador, a.CampanaId }).ToJson()}");
                    return listaAgenteCompra;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Error("Error ObtenerNegocios MAT: ", e.Message);
                throw;
            }
        }

        private Operador ObtenerOperador(List<TradeCaptureReportRootParties> rootParties, List<Operador> operadores)
        {
            var operadorPrimary = rootParties.Where(x => x.RootPartyRole == "14").SingleOrDefault();
            return operadores.Where(x => x.CodigoPrimary == operadorPrimary.RootPartyID).SingleOrDefault();
        }

        private static double ObtenerCantidad(TradeCaptureReportValue a)
        {
            int value = 1;

            if (a.TrdCapRptSideGrp != null && a.TrdCapRptSideGrp.Count > 0)
            {
                value = a.TrdCapRptSideGrp.First().Side != "2" ? 1 : -1;
            }

            return value * decimal.ToDouble((a.LastQty ?? 0) * 1000);
        }

        private int ObtenerCampania(List<Instrument> instruments, TradeCaptureReportInstrument tradeCaptureReportInstrument)
        {
            var campanias = repositorio.Listar<Campaña, CampañaQry>(a => new CampañaQry { CampañaId = a.CampañaId, Descripcion = a.Descripcion });
            var item = instruments.Where(a => a.SecurityID == tradeCaptureReportInstrument.SecurityID).FirstOrDefault();
            if (item != null)
            {
                if (string.IsNullOrEmpty(item.MaturityMonthYear))
                {
                    logger.Error($"ObtenerCampania - No se puede grabar MATBA porque el instrumento con SecurityID {tradeCaptureReportInstrument.SecurityID} no tiene la posición (MaturityMonthYear).");
                    return 0;
                }
                var anio = int.Parse(item.MaturityMonthYear.Substring(2, 2)) - 1;
                var mes = int.Parse(item.MaturityMonthYear.Substring(4, 2));
                int materialId = ObtenerMaterial(instruments, tradeCaptureReportInstrument);
                string anioStr = anio.ToString();
                string descripcionBuscar = anioStr;
                // Caso especial SOJA y MAIZ
                if ((materialId == (int)EnumMateriales.SOJA ||
                     materialId == (int)EnumMateriales.MAIZ))
                {
                    bool esCampaniaAnterior = (mes == 1 || mes == 2);
                    descripcionBuscar = esCampaniaAnterior ? $"{anio - 2}-{anio - 1}" : anioStr;
                }
                else if (materialId == (int)EnumMateriales.TRIGO)
                {
                    // Regla especial trigo
                    if ((mes == 11 || mes == 12))
                    {
                        descripcionBuscar = $"{anio}-{anio + 1}";
                    }
                }

                int? campaña =  campanias
                    .Where(c => c.Descripcion.StartsWith(descripcionBuscar))
                    .Select(c => c.CampañaId)
                    .SingleOrDefault();

                return campaña ?? 0;
            }
            else
            {
                return 0;
            }
        }

        private string ObtenerPosicion(List<Instrument> instruments, TradeCaptureReportInstrument tradeCaptureReportInstrument)
        {
            var item = instruments.Where(a => a.SecurityID == tradeCaptureReportInstrument.SecurityID).FirstOrDefault();
            if (item != null)
            {
                if (string.IsNullOrEmpty(item.MaturityMonthYear))
                {
                    logger.Error($"ObtenerPosicion - No se puede grabar MATBA porque el instrumento con SecurityID {tradeCaptureReportInstrument.SecurityID} no tiene la posición (MaturityMonthYear).");
                    return "";
                }
                return item.MaturityMonthYear.Substring(4, 2) + "." + item.MaturityMonthYear.Substring(0, 4);
            }
            else
            {
                return "";
            }
        }

        private int ObtenerMaterial(List<Instrument> instruments, TradeCaptureReportInstrument tradeCaptureReportInstrument)
        {
            int materialId = 0;
            var item = instruments.Where(a => a.SecurityID == tradeCaptureReportInstrument.SecurityID).FirstOrDefault();
            if (item != null)
            {
                switch (item.SecurityGroup)
                {
                    case var s when item.SecurityGroup.Contains("MAI"):
                        materialId = (int)EnumMateriales.MAIZ;
                        break;
                    case var s when item.SecurityGroup.Contains("TRI"):
                        materialId = (int)EnumMateriales.TRIGO;
                        break;
                    case var s when item.SecurityGroup.Contains("SOJ"):
                        materialId = (int)EnumMateriales.SOJA;
                        break;
                    case var s when item.SecurityGroup.Contains("GIR"):
                        materialId = (int)EnumMateriales.GIRASOL;
                        break;
                    case var s when item.SecurityGroup.Contains("GIO"):
                        materialId = (int)EnumMateriales.GIRASOL_AO;
                        break;
                    default:
                        break;
                }
            }
            return materialId;
        }

        private bool EsDolarExportador(TradeCaptureReportInstrument tradeCaptureReportInstrument)
        {
            bool esExportador = tradeCaptureReportInstrument.SecurityID.StartsWith("MAI.EXP") || tradeCaptureReportInstrument.SecurityID.StartsWith("SOJ.EXP");
            return esExportador;
        }

        private TradeCaptureReportResult GetTradeCaptureReport(TokenPrimary token, string desde, string hasta)
        {
            var url = $"PosTrade/TradeCaptureReport?DateFrom={desde}&DateTo={hasta}&MarketID=&MarketSegmentID=&CFICode";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(urlBase) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token.Value);

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            var jObject = JObject.Parse(res.ToString());

            TradeCaptureReportResult result = JsonConvert.DeserializeObject<TradeCaptureReportResult>(jObject.ToString());
            return result;
        }

        private SecurityListResult SecurityList(TokenPrimary token/*, string cFICode*/)
        {
            //var url = $"/PreTrade/SecurityList?cFICode={cFICode}";
            //var url = $"/PreTrade/SecurityList?marketSegmentID=Agropecuario";
            var url = $"/PreTrade/SecurityList";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(urlBase) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token.Value);

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            var jObject = JObject.Parse(res.ToString());

            SecurityListResult result = JsonConvert.DeserializeObject<SecurityListResult>(jObject.ToString());
            return result;
        }

        public MarketDataResult ObtenerCotizacion(DateTime fecha)
        {
            try
            {
                var token = ReuseToken();
                if (token.Code != "200")
                {
                    throw new Exception(token.ErrorMessage + ", " + token.ErrorDescription);
                }

                logger.Debug($"Obteniendo cotizaciones...");
                MarketDataResult result = MarketData(token, fecha.ToString("yyyyMMdd"));
                if (result.Code == "200")
                {
                    return result;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Error("Error ObtenerCotizacion ", e.Message);
                throw;
            }
        }

        private MarketDataResult MarketData(TokenPrimary token, string fecha)
        {
            var url = $"PosTrade/MarketData?mdEntryType=5&ClearingBusinessDate={fecha}";
            HttpClient client = new HttpClient() { BaseAddress = new Uri(urlBase) };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token.Value);

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            var jObject = JObject.Parse(res.ToString());

            MarketDataResult result = JsonConvert.DeserializeObject<MarketDataResult>(jObject.ToString());
            return result;
        }

        public TokenPrimary ReuseToken()
        {
            var tokenPrimary = new TokenPrimary();
            if (cache.Existe("TokenPrimary"))
            {
                tokenPrimary = cache.Obtener<TokenPrimary>("TokenPrimary");
            }
            else
            {
                tokenPrimary = Retry.Do(ObtenerToken, TimeSpan.FromSeconds(1), 4);
                if (tokenPrimary.Status == HttpStatusCode.OK.ToString())
                    cache.Agregar("TokenPrimary", tokenPrimary, DateTime.Now.AddHours(23.5));
            }
            return tokenPrimary;
        }
    }
}