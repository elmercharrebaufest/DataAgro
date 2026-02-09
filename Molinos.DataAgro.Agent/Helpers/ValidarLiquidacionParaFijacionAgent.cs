using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ValidarLiquidacionParaFijacionAgent : IValidarLiquidacionParaFijacionAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidarLiquidacionParaFijacionAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string Validar(string contratoSap, string fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "Ok";
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var rq = new ZMpfrcValLiqParFijacion
                    {
                        ImConPed = new Zmpes6290[] { new Zmpes6290 { Contrato = contratoSap, Pedido = fijacion } },
                    };
                    logger.Debug(rq.ToXml());
                    var valor = agent.ZMpfrcValLiqParFijacion(rq);
                    logger.Debug(valor.ToXml());
                    return valor.ExResultado[0].Mensaje;
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
