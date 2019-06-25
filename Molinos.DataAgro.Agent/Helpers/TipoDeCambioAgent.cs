using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.TipoDeCambio;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class TipoDeCambioAgent : ITipoDeCambioAgent
    {
        private readonly ILogger logger;
        public TipoDeCambioAgent(ILogger logger)
        {
            this.logger = logger;
        }

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public decimal TraerTipoDeCambio()
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return 45;
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIOClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_TIPO_DE_CAMBIO() { DATE = DateTime.Now.Date.ToString("yyyy-MM-dd"), FOREIGN_AMOUNT = 1, FOREIGN_CURRENCY = "USDM ", LOCAL_CURRENCY = "ARP  " };
                    logger.Debug(rq.ToXml());

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIO(rq);
                    logger.Debug(devolucion.ToXml());
                    return devolucion.EXCHANGE_RATE;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return 1;
                }
            }
        }
    }
}
