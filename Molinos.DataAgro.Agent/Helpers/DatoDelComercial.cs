using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Agent.DatosDelComercial;
using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class DatoDelComercial
    {
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

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

                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_COMERCIALES(rq);

                return valor1;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
