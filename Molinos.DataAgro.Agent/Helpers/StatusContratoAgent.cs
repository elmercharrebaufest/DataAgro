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

        public EstadoSAPDto ValidarEstado(string contratoSap)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new EstadoSAPDto { Status = "OK", NumeroSio = 0 };
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
                    long numsio = 0;
                    long.TryParse(valor.EX_NUM_SIO, out numsio);
                    logger.Debug("valor.EX_STATUS: ." + valor.EX_STATUS + ".");
                    logger.Debug("numsio: ." + numsio + ".");
                    var estado = new EstadoSAPDto()
                    {
                        NumeroSio = numsio,
                        Status = valor.EX_STATUS
                    };
                    return estado;
                    //if (string.IsNullOrEmpty(valor.EX_STATUS) && numsio == 0)
                    //{
                    //    return "";
                    //}
                    //else
                    //{
                    //    return "El contrato ya no se encuentra en slip o fue informado a SIO granos";
                    //}

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
