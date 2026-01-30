using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class RelacionCorredorProveedorAgent : IRelacionCorredorProveedorAgent
    {
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public RelacionCorredorProveedorAgent(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public bool ObtenerRelacionCorredorProveedor(string cuitCorredor, string cuitProveedor)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return repositorio.Existe<CorredorProveedor>(x => x.Proveedor.CUIT == cuitProveedor && x.Corredor.CUIT == cuitCorredor);
            }
            else
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new ZMprfcConsultarRp()
                {
                    ImCorredor = cuitCorredor,
                    ImProveedor = cuitProveedor
                };
                var valor = agent.ZMprfcConsultarRp(rq);
                var retorno = true;
                if (valor.ExMensaje != "X")
                {
                    retorno = false;
                }
                return retorno;

            }
        }
    }
}
