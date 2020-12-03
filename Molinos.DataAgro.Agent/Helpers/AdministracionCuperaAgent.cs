using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AdministracionCupera;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class AdministracionCuperaAgent : IAdministracionCuperaAgent
    {
        public AdministracionCuperaAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string AdministrarCupera(ConfiguracionCupoDto c)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_ADMIN_CUPOSClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var rq = new Z_MPRFC_ADMIN_CUPOS()
                    {
                        IM_CIERRE = c.CierreCupera.HasValue && c.CierreCupera.Value ? "X" : "",
                        IM_FECHA = c.Fecha.ToString("yyyy-MM-dd"),
                        IM_LIMITE_CUPOS = c.LimiteCupo.ToString(),
                        IM_LIMITE_CUPOS_ANT = c.LimiteCupoAnterior.ToString(),
                        IM_MATNR = c.Material,
                        IM_ZONA = c.ZonaCupo,
                        IM_WERKS = c.Centro,
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_ADMIN_CUPOS(rq);
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
