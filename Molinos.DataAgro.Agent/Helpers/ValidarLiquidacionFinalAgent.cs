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
    public class ValidarLiquidacionFinalAgent : IValidarLiquidacionFinalAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidarLiquidacionFinalAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ValidarLiquidacionFinal(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcValidarLiqFinal()
                    {
                        ImContrato = fijacion.ContratoSAP,
                        ImPedido = fijacion.FijacionSAP
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcValidarLiqFinal(rq);
                    logger.Debug(devolucion.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    return devolucion.ExMensaje;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al validar liquidación final.");
                    throw;
                }
            }
        }

        public string ValidarLiquidacionParcial(FijacionDePrecioContrato fijacion)
        {
            throw new NotImplementedException();
        }

        public string ValidarPesificacion(FijacionDePrecioContrato fijacion)
        {
            throw new NotImplementedException();
        }

        public string ValidarLiquidacionComisiones(FijacionDePrecioContrato fijacion)
        {
            throw new NotImplementedException();
        }
    }
}
