using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidacionCredito;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ValidacionCreditoAgent : IValidacionCreditoAgent
    {
        public ValidacionCreditoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public ValidarCreditoDto ValidarCredito(string cuit)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new ValidarCreditoDto { Moneda = "ARP", Monto = 100000000};
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_VALIDAR_CREDITOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_VALIDAR_CREDITO()
                    {
                        IM_CUIT = cuit,                         
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_VALIDAR_CREDITO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    var credito = new ValidarCreditoDto
                    {
                       Cuit = valor.EX_CUIT,
                       Monto = valor.EX_MONTO,
                       Moneda = valor.EX_MONEDA
                    };             
                    return credito;
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
