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
        private readonly IDiasHabilesAgent diasHabilesAgent;
        public TipoDeCambioAgent(ILogger logger, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.diasHabilesAgent = diasHabilesAgent;
        }

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public decimal TraerTipoDeCambio(DateTime? fecha)
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
                    var agent = new SI_ZMPWS_DATAAGRO_TIPO_DE_CAMBIOClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_TIPO_DE_CAMBIO() { DATE = fecha.Value.Date.ToString("yyyy-MM-dd"), FOREIGN_AMOUNT = 1, FOREIGN_CURRENCY = "USDM ", LOCAL_CURRENCY = "ARP  " };
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

        public decimal TraerTipoDeCambioUltimoDiaHabil(DateTime? fecha)
        {
            if (fecha == null)
            {
                fecha = DateTime.Now.Date;
            }
            fecha = diasHabilesAgent.UltimoDiaHabil(fecha);
            var tipoDeCambio = TraerTipoDeCambio(fecha);
            return tipoDeCambio;
        }
    }
}
