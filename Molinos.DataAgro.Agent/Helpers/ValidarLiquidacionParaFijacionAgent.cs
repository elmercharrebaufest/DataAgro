using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarLiquidacionParaFijacion;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ValidarLiquidacionParaFijacionAgent : IValidarLiquidacionParaFijacionAgent
    {
        public ValidarLiquidacionParaFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string Validar(string contratoSap, string fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "Ok";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_VAL_LIQ_PAR_FIJACIONClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPFRC_VAL_LIQ_PAR_FIJACION
                    {
                        IM_CON_PED = new ZMPES6290[] { new ZMPES6290 { CONTRATO = contratoSap, PEDIDO = fijacion } },
                    };
                    logger.Debug(rq.ToXml());
                    
                    var valor = agent.SI_ZMPWS_DATAAGRO_VAL_LIQ_PAR_FIJACION(rq);
                    logger.Debug(valor.ToXml());

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
