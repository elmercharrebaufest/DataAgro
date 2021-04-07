using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    class ClientePrimariAPIAgent : IClientePrimariAPIAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        readonly String urlBase = ConfigurationManager.AppSettings["UrlBasePrimary"];
        readonly String user = ConfigurationManager.AppSettings["UserPrimary"];
        readonly String pass = ConfigurationManager.AppSettings["PassPrimary"];

        public ClientePrimariAPIAgent(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
        }
        public TokenPrimary ObtenerToken()
        {
            try
            {
                // Create a new token
                logger.Debug($"Gestionando Token...");
                var token = new TokenPrimary();
                var url = $"AuthToken/AuthToken?nombreUsuario={user}&password={pass}";


                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(urlBase);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.PostAsJsonAsync(
                    url, new { }).Result;
                response.EnsureSuccessStatusCode();
                var res = response.Content.ReadAsAsync<dynamic>().Result;
                var jObject = JObject.Parse(res.ToString());

                token = JsonConvert.DeserializeObject<TokenPrimary>(jObject.ToString());

                logger.Debug($"Token obtenido: {JsonConvert.SerializeObject(token)}");
                return token;
            }
            catch (Exception e)
            {
                logger.Debug($"Error obtener token");
                logger.Error(e.Message);
                throw;
            }
        }

        public TradeCaptureReportResult ObtenerNegocios()
        {
            try
            {
                var hasta = DateTime.Now.ToString("yyyyMMdd");
                var desde = DateTime.Now.AddMonths(-1).ToString("yyyyMMdd");
                //pruebas
                hasta = "20210127";
                desde = "20210127";
                var token = ObtenerToken();
                //List<MaterialDto> materiales = repositorio.Listar<Material, MaterialDto>(a => new MaterialDto { MaterialId = a.MaterialId, Descripcion = a.Descripcion, CampañaId = a.CampañaId }, null, 0, null, Entities.Helpers.DirOrden.Asc);
                if (token.Code != "200")
                {
                    throw new Exception(token.ErrorMessage + ", " + token.ErrorDescription);
                }
                // Create a new token
                logger.Debug($"Obteniendo negocios...");
                TradeCaptureReportResult result = GetTradeCaptureReport(token, desde, hasta);
                if (result.Code == "200")
                {
                    List<string> CFICodes = result.Value.Select(a => a.Instrument.First().CFICode).Distinct().ToList();
                    List<Instrument> instruments = new List<Instrument>();
                    //foreach (var item in CFICodes)
                    //{
                    var instrumentos = SecurityList(token /*, item*/);
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
                    //}
                    List<AgenteCompra> lista = new List<AgenteCompra>();

                    lista = result.Value.Select(a => new AgenteCompra
                    {

                        TipoNegocioId = 5,
                        Cantidad = decimal.ToDouble((a.LastQty ?? 0) * 1000),
                        Precio = a.LastPx ?? 0,
                        FechaDesde = DateTime.ParseExact(a.TransactTime, "s", null).Date,
                        FechaHasta = DateTime.ParseExact(a.TransactTime, "s", null).Date.AddMonths(1),
                        FechaOperacion = DateTime.ParseExact(a.TransactTime, "s", null),
                        MonedaId = a.Currency == "USD" ? "USDM " : "ARP  ",
                        Fecha = DateTime.ParseExact(a.TransactTime, "s", null),
                        EstadoId = a.TrdRptStatus == "3" ? 6 : 2,
                        DestinoId = 1,
                        //tendriamos que tener un nuevo campo para guardar el nro de contrato del MAT
                        Observacion = a.TradeID == null ? "" : a.TradeID.Value.ToString(),//codigo contrato MAT
                                                                                          //faltan
                        MaterialId = ObtenerMaterial(instruments, a.Instrument[0]),
                        Posicion = ObtenerPosicion(instruments, a.Instrument[0]),
                        CampanaId = ObtenerCampania(instruments, a.Instrument[0]),
                        OperadorId = 1,
                        TipoAgenteCompraId = 1,
                        ComercialId = 1,
                        ComercialCreadorId = 1,


                    }).ToList();

                    var  ppp = lista.ToJson();
                    return result;
                }

                return null;
            }
            catch (Exception e)
            {
                logger.Debug($"Error obtener token");
                logger.Error(e.Message);
                throw;
            }
        }

        private int ObtenerCampania(List<Instrument> instruments, TradeCaptureReportInstrument tradeCaptureReportInstrument)
        {
            var campanias = repositorio.Listar<Campaña, CampañaQry>(a => new CampañaQry { CampañaId = a.CampañaId, Descripcion = a.Descripcion });
            var item = instruments.Where(a => a.SecurityID == tradeCaptureReportInstrument.SecurityID).FirstOrDefault();
            if (item != null)
            {
                var anio = (int.Parse(item.MaturityMonthYear.Substring(2, 2)) -1).ToString();
                int? campaña = campanias.Where(a => a.Descripcion.StartsWith(anio)).Select(a => a.CampañaId).SingleOrDefault();
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
                        materialId = 1; break;
                    case var s when item.SecurityGroup.Contains("TRI"):
                        materialId = 2; break;
                    case var s when item.SecurityGroup.Contains("SOJ"):
                        materialId = 3; break;
                    case var s when item.SecurityGroup.Contains("GIR"):
                        materialId = 4; break;
                    case var s when item.SecurityGroup.Contains("GIO"):
                        materialId = 5; break;
                    default:
                        break;
                }
            }
            return materialId;

        }

        private TradeCaptureReportResult GetTradeCaptureReport(TokenPrimary token, string desde, string hasta)
        {
            var url = $"PosTrade/TradeCaptureReport?DateFrom={desde}&DateTo={hasta}&MarketID=&MarketSegmentID=&CFICode";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
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
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
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

                var token = ObtenerToken();
                //List<MaterialDto> materiales = repositorio.Listar<Material, MaterialDto>(a => new MaterialDto { MaterialId = a.MaterialId, Descripcion = a.Descripcion, CampañaId = a.CampañaId }, null, 0, null, Entities.Helpers.DirOrden.Asc);
                if (token.Code != "200")
                {
                    throw new Exception(token.ErrorMessage + ", " + token.ErrorDescription);
                }
                // Create a new token
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
                logger.Debug($"Error obtener token");
                logger.Error(e.Message);
                throw;
            }
        }

        private MarketDataResult MarketData(TokenPrimary token, string fecha)
        {
            var url = $"PosTrade/MarketData?mdEntryType=5&ClearingBusinessDate={fecha}";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(urlBase);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token.Value);

            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            var res = response.Content.ReadAsAsync<dynamic>().Result;
            var jObject = JObject.Parse(res.ToString());

            MarketDataResult result = JsonConvert.DeserializeObject<MarketDataResult>(jObject.ToString());
            return result;
        }
    }
}
