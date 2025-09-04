using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarLiquidacionComisiones;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ValidarLiquidacionComisionesAgent : IValidarLiquidacionComisionesAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidarLiquidacionComisionesAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ValidarLiquidacionComisiones(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new ZMprfcValidarLiqComisiones()
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
                        var devolucion = agent.ZMprfcValidarLiqComisiones(rq);
                        logger.Debug(devolucion.ToXml());

                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += devolucion.ToXml();
                        repositorio.GuardarCambios();
                        return devolucion.ExMensaje;

                    }
                    else
                    {
                        SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_COMISIONESClient agent = new SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_COMISIONESClient();

                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_VALIDAR_LIQ_COMISIONES()
                        {
                            IM_CONTRATO = fijacion.ContratoSAP,
                            IM_PEDIDO = fijacion.FijacionSAP
                        };
                        logger.Debug(rq.ToXml());

                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };

                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();

                        var devolucion = agent.SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_COMISIONES(rq);
                        logger.Debug(devolucion.ToXml());

                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += devolucion.ToXml();
                        repositorio.GuardarCambios();
                        return devolucion.EX_MENSAJE;
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al validar liquidación comisiones.", e);
                    throw;
                }
            }
        }

    }
}
