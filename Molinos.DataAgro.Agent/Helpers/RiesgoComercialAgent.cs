using Molinos.DataAgro.Agent.RiesgoComercial;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Autofac.Extras.NLog;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class RiesgoComercialAgent : IRiesgoComercialAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public RiesgoComercialAgent(ILogger logger)
        {
            this.logger = logger;
        }
        public string ObtenerRiesgoComercial(string CUIT)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                CUIT = ConfigurationManager.AppSettings["SapPruebaCUIT"];
            }

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                logger.Info("SAP sin PI - RFC ZMprfcRiesgoComercial");
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new ZMprfcRiesgoComercial() { ImCuit = CUIT };
                var valor = agent.ZMprfcRiesgoComercial(rq);
                logger.Info("SAP sin PI - RFC ZMprfcRiesgoComercial");
                return valor.ExRiesgo;
            }
            else
            {
                SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient agent = new SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_RIESGO_COMERCIAL() { IM_CUIT = CUIT };
                var valor = agent.SI_ZMPWS_DATAAGRO_RIESGO_COMERCIAL(rq);
                return valor.EX_RIESGO;
            }
        }

    }
}
