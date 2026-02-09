using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EliminarContratoAgent : IEliminarContratoAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public EliminarContratoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string Eliminar(Contrato contrato)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                string resp;
                if (contrato.NoInformaSio.HasValue && contrato.NoInformaSio.Value)
                {
                    resp = "Error al borrar contrato" + contrato.ContratoSAP + " - Campos NUM_SIO y/o STATUS con datos";
                }
                else
                {
                    resp = "Contrato " + contrato.ContratoSAP + " Ha sido borrado";
                }
                return resp;
            }
            else
            {
                try
                {

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcAnulacionContratoRequest()
                    {
                        ZMprfcAnulacionContrato = new ZMprfcAnulacionContrato
                        {
                            ImContrato = contrato.ContratoSAP
                        }
                    };

                    logger.Debug(rq.ToXml());
                    var devolucion = agent.ZMprfcAnulacionContrato(rq.ZMprfcAnulacionContrato);
                    logger.Debug(devolucion.ToXml());
                    return devolucion.ExMensaje;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al eliminar contrato.");
                    throw;
                }
            }
        }
    }
}
