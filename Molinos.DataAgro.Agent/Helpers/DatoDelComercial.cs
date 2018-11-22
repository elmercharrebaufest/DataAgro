using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelComercial;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Helpers;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class DatoDelComercial
    {
        private readonly ILogger logger;
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatoDelComercial(ILogger logger)
        {
            this.logger = logger;
        }

        public Z_MPRFC_DATOS_COMERCIALResponse ObtenerDatosDeComercial(string Usuario)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                Usuario = ConfigurationManager.AppSettings["SapPruebaUser"];
            }

            try
            {
                SI_ZMPWS_DATAAGRO_DATOS_COMERCIALESClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMERCIALESClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_DATOS_COMERCIAL() { IM_USUARIO = Usuario };
                logger.Debug(rq.ToXml());
                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_COMERCIALES(rq);

                return valor1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

    }
}
