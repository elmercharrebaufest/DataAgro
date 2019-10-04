using Molinos.DataAgro.Agent.CapacidadProductiva;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent
{
    public class CapacidadProductivaAgent : ICapacidadProductivaAgent
    {
        public CapacidadProductivaAgent(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly IRepositorio repositorio;
        public string ObtenerCapacidadProductiva(string cuit, decimal cantidad, string centro, string cosecha, string material )
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {

                SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVAClient agent = new SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVAClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_CAPACIDAD_PRODUCTIVA()
                {
                    IM_CUIT = cuit,
                    IM_CANTIDAD = cantidad,
                    IM_CENTRO = centro,
                    IM_COSECHA = cosecha,
                    IM_MATERIAL = material
                
                };

                var valor = agent.SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVA(rq);
                
                return valor.EX_MENSAJE;
            }
        }

    }
}
