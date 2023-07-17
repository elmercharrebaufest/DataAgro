using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarCantidadAcuerdo;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ValidarCantidadAcuerdoAgent : IValidarCantidadAcuerdoAgent
    {
        public ValidarCantidadAcuerdoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string ValidarCantidad(double cantidad, string acuerdo)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_VALIDAR_CANT_ACUERDOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_VALIDAR_CANT_ACUERDO()
                    {
                     IM_CANTIDAD = (decimal)cantidad,
                     IM_NROACUERDO = acuerdo
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_VALIDAR_CANT_ACUERDO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    
                    return valor.EX_MENSAJE;
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