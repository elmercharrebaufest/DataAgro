using Molinos.DataAgro.Agent.RelacionCorredorProveedor;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class RelacionCorredorProveedorAgent : IRelacionCorredorProveedorAgent
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public RelacionCorredorProveedorAgent(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public bool ObtenerRelacionCorredorProveedor(string cuitCorredor, string cuitProveedor)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return repositorio.Existe<CorredorProveedor>(x => x.Proveedor.CUIT == cuitProveedor && x.Corredor.CUIT == cuitCorredor);
            }
            else
            {

                SI_ZMPWS_DATAAGRO_CONSULTAR_RPClient agent = new SI_ZMPWS_DATAAGRO_CONSULTAR_RPClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_CONSULTAR_RP()
                {
                    IM_CORREDOR = cuitCorredor,
                    IM_PROVEEDOR = cuitProveedor
                };

                var valor = agent.SI_ZMPWS_DATAAGRO_CONSULTAR_RP(rq);
                var retorno = true;
                if (valor.EX_MENSAJE != "X")
                {
                    retorno = false;
                }
                return retorno;
            }
        }

    }
}
