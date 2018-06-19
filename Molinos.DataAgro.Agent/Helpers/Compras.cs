using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Agent
{
    public class ComprasAgent
    {


        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];


        public List<ZMPES5130> Comprar(string CUIT, string UsuarioComercial)
        {

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            { 
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient agent = new Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS() { IM_CUIT = new List<String>() { CUIT }.ToArray(), IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS(rq);

            return devolucion.EX_COMPRAS.ToList();

        }

        public List<ZMPES5130> ComprarIniciales(List<String> CUIT, string UsuarioComercial)
        {

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient agent = new Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS() { IM_CUIT =  CUIT.ToArray() , IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS(rq);

            return devolucion.EX_COMPRAS.ToList();

        }
    }
}
