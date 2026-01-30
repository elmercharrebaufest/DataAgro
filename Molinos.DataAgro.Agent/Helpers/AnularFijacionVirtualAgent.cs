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
    public class AnularFijacionVirtualAgent : IAnularFijacionVirtualAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public AnularFijacionVirtualAgent(IRepositorio repositorio, ILogger logger)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string AnularFijacionVirtual(FijacionDePrecioContrato fijacion, string comercial)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                var resp = "OK";
                return resp;
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcAnularFijVirCanjeRequest()
                    {
                        ZMprfcAnularFijVirCanje = new ZMprfcAnularFijVirCanje
                        {
                            ImContrnum = fijacion.ContratoSAP.Substring(3),
                            ImNroFij = fijacion.FijacionSAP.Substring(10),
                            ImUname = comercial.ToUpper()
                        }
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());
                    var devolucion = agent.ZMprfcAnularFijVirCanje(rq.ZMprfcAnularFijVirCanje);
                    logger.Debug(devolucion.ToXml());
                    return devolucion.ExMensaje;

                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al anular fijación virtual.");
                    throw;
                }
            }
        }
    }
}
