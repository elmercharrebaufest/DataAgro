using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AnularCupo;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EliminarCupoAgent : IEliminarCupoAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public EliminarCupoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string Eliminar(string cupoSap, string comercial)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                try
                {
                    if (repositorio.Existe<Cupo>(x => x.CupoSap == cupoSap))
                    {
                        return "OK";
                    }
                    else
                    {
                        return "";
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al eliminar cupo.", e);
                    throw;
                }
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_ANULAR_CUPOSClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_ANULAR_CUPOS
                    {
                        IM_CODIGO = cupoSap,
                        IM_COMERCIAL = comercial
                    };

                    logger.Debug(rq.ToXml());
                    var logId = repositorio.Agregar(new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    });
                    repositorio.GuardarCambios();

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_ANULAR_CUPOS(rq);
                    logger.Debug(devolucion.ToXml());

                    var log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    return devolucion.EX_MENSAJE;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al eliminar cupo.", e);
                    throw;
                }
            }
        }
    }
}
