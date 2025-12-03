using NLog;
using Molinos.DataAgro.Agent.TipoDeCambio;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
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

                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
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
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_TIPO_DE_CAMBIO()
                        {
                            DATE = fecha.Value.Date.ToString("yyyy-MM-dd"),
                            FOREIGN_AMOUNT = 1,
                            FOREIGN_CURRENCY = "USDM ",
                            LOCAL_CURRENCY = "ARP  ",
                            TYPE_OF_RATE = typeOfRate
                        };
                        logger.Debug(rq.ToXml());

                        var devolucion = agent.SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIO(rq);
                        logger.Debug(devolucion.ToXml());
                        return devolucion.EXCHANGE_RATE;
                    }
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

                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
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
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;
                        var rq = new Z_MPRFC_TIPO_DE_CAMBIO()
                        {
                            DATE = fecha.Value.Date.ToString("yyyy-MM-dd"),
                            FOREIGN_AMOUNT = 1,
                            FOREIGN_CURRENCY = moneda,
                            LOCAL_CURRENCY = "ARP  ",
                            TYPE_OF_RATE = typeOfRate
                        };
                        logger.Debug(rq.ToXml());
                        var devolucion = agent.SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIO(rq);
                        logger.Debug(devolucion.ToXml());
                        return devolucion.EXCHANGE_RATE;
                    }
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
