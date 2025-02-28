using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AnulacionContratos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
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
                    var agent = new SI_ZMPWS_DATAAGRO_ANULACION_CONTRATOClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var rq = new SI_ZMPWS_DATAAGRO_ANULACION_CONTRATORequest()
                    {
                        Z_MPRFC_ANULACION_CONTRATO = new Z_MPRFC_ANULACION_CONTRATO
                        {
                            IM_CONTRATO = contrato.ContratoSAP
                        }
                    };

                    logger.Debug(rq.ToXml());
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_ANULACION_CONTRATO(rq.Z_MPRFC_ANULACION_CONTRATO);
                    logger.Debug(devolucion.ToXml());

                    return devolucion.EX_MENSAJE;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al eliminar contrato.", e);
                    throw;
                }
            }
        }
    }
}
