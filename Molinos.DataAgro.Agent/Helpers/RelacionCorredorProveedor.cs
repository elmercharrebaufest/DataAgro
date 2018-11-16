using Molinos.DataAgro.Agent.RiesgoComercial;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent
{
    public class RelacionCorredorProveedor
    {
        public RelacionCorredorProveedor(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly IRepositorio repositorio;
        public bool ObtenerRelacionCorredorProveedor(string cuitCorredor, string cuitProveedor)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return repositorio.Existe<CorredorProveedor>(x => x.Proveedor.CUIT == cuitProveedor && x.Corredor.CUIT == cuitCorredor);
            }
            else
            {

                SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient agent = new SI_ZMPWS_DATAAGRO_RIESGO_COMERCIALClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_RIESGO_COMERCIAL() { IM_CUIT = cuitProveedor };

                var valor = agent.SI_ZMPWS_DATAAGRO_RIESGO_COMERCIAL(rq);

                return true;
            }
        }

    }
}
