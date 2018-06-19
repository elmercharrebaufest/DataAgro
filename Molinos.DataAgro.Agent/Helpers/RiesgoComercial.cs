using Molinos.DataAgro.Agent.RiesgoComercial;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent
{
    public class RiesgoComerciales
    {

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public string ObtenerRiesgoComercial(string CUIT)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                CUIT = ConfigurationManager.AppSettings["SapPruebaCUIT"];
            }
           
            SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient agent = new SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient();

            agent.ClientCredentials.UserName.UserName = UserSap;

            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_RIESGO_COMERCIAL() { IM_CUIT =  CUIT };

            var valor = agent.SI_ZMPWS_DATAAGRO_RIESGO_COMERCIAL(rq);

            return valor.EX_RIESGO;
        }

    }
}
