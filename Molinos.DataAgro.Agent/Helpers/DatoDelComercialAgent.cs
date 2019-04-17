using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelComercial;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class DatoDelComercialAgent : IDatoDelComercialAgent
    {
        private readonly ILogger logger;
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatoDelComercialAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public DatosComercialAgentDto ObtenerDatosDeComercial(string Usuario)
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

                return new DatosComercialAgentDto {EX_GRUPO_COMPRAS=valor1.EX_GRUPO_COMPRAS,EX_ZONA=valor1.EX_ZONA };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

    }
}
