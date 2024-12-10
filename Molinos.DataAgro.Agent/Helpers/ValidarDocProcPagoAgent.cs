using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarDocProcPago;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ValidarDocProcPagoAgent : IValidarDocProcPagoAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidarDocProcPagoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ValidarEstado(string contratoSap, string fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "Ok";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_VALIDAR_DOC_PROC_PAGOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_VALIDAR_DOC_PROC_PAGO
                    {
                        IM_CON_PED = new ZMPES6290[] { new ZMPES6290 { CONTRATO = contratoSap, PEDIDO = fijacion } },

                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_VALIDAR_DOC_PROC_PAGO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return valor.EX_RESULTADO[0].MENSAJE;

                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
