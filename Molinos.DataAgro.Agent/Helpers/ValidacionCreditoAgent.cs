using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ValidacionCreditoAgent : IValidacionCreditoAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidacionCreditoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public ValidarCreditoDto ValidarCredito(string cuit)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new ValidarCreditoDto { Moneda = "ARP", Monto = 100000000 };
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcValidarCredito()
                    {
                        ImCuit = cuit,
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());
                    var valor = agent.ZMprfcValidarCredito(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    var credito = new ValidarCreditoDto
                    {
                        Cuit = valor.ExCuit,
                        Monto = valor.ExMonto,
                        Moneda = valor.ExMoneda
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
