using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
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
                    logger.Error(e, "Error comunicacion SAP al eliminar cupo.");
                    throw;
                }
            }
            else
            {
                try
                {

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var rq = new ZMprfcAnularCupos
                    {
                        ImCodigo = cupoSap,
                        ImComercial = comercial
                    };

                    logger.Debug(rq.ToXml());
                    var logId = repositorio.Agregar(new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    });
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcAnularCupos(rq);
                    logger.Debug(devolucion.ToXml());

                    var log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    return devolucion.ExMensaje;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al eliminar cupo.");
                    throw;
                }
            }
        }
    }
}
