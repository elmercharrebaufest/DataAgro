using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.StatusContrato;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class StatusContratoAgent : IStatusContratoAgent
    {
        public StatusContratoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string ValidarEstado(string contratoSap)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_STATUS_DE_CONTRATO()
                    {
                        IM_CONTRATO = contratoSap
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    if (!string.IsNullOrEmpty(valor.EX_STATUS) || !string.IsNullOrEmpty(valor.EX_NUM_SIO))
                    {
                    return "No se puede modificar contrato ya que no se encuentra en slip.";
                    }
                    return "";
                }catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
