using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class TipoDeCambioAgent : ITipoDeCambioAgent
    {
        private readonly ILogger logger;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public TipoDeCambioAgent(ILogger logger, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.diasHabilesAgent = diasHabilesAgent;
        }

        public decimal TraerTipoDeCambio(DateTime? fecha, string typeOfRate)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return 94;
            }
            else
            {
                try
                {
                    if (fecha == null)
                    {
                        fecha = DateTime.Now.Date;
                    }

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcTipoDeCambio()
                    {
                        Date = fecha.Value.Date.ToString("yyyy-MM-dd"),
                        ForeignAmount = 1,
                        ForeignCurrency = "USDM ",
                        LocalCurrency = "ARP  ",
                        TypeOfRate = typeOfRate
                    };
                    logger.Debug(rq.ToXml());

                    var devolucion = agent.ZMprfcTipoDeCambio(rq);
                    logger.Debug(devolucion.ToXml());
                    return devolucion.ExchangeRate;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return 1;
                }
            }
        }

        public decimal TraerTipoDeCambioMoneda(DateTime? fecha, string moneda, string typeOfRate)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return 94;
            }
            else
            {
                try
                {
                    if (fecha == null)
                    {
                        fecha = DateTime.Now.Date;
                    }

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcTipoDeCambio()
                    {
                        Date = fecha.Value.Date.ToString("yyyy-MM-dd"),
                        ForeignAmount = 1,
                        ForeignCurrency = moneda,
                        LocalCurrency = "ARP  ",
                        TypeOfRate = typeOfRate
                    };
                    logger.Debug(rq.ToXml());
                    var devolucion = agent.ZMprfcTipoDeCambio(rq);
                    logger.Debug(devolucion.ToXml());
                    return devolucion.ExchangeRate;

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return 1;
                }
            }
        }

        public decimal TraerTipoDeCambioUltimoDiaHabil(DateTime? fecha, string typeOfRate)
        {
            if (fecha == null)
            {
                fecha = DateTime.Now.Date;
            }
            fecha = diasHabilesAgent.UltimoDiaHabil(fecha);
            var tipoDeCambio = TraerTipoDeCambio(fecha, typeOfRate);
            return tipoDeCambio;
        }
    }
}
