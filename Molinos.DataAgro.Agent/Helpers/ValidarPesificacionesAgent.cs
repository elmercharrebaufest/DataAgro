using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarPesificaciones;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ValidarPesificacionesAgent : IValidarPesificacionAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ValidarPesificacionesAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string ValidarPesificacion(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                try
                {
                    SI_ZMPWS_DATAAGRO_VALIDAR_PESIFICACIONESClient agent = new SI_ZMPWS_DATAAGRO_VALIDAR_PESIFICACIONESClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_VALIDAR_PESIFICACIONES()
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

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_VALIDAR_PESIFICACIONES(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    return devolucion.EX_MENSAJE;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al validar pesificación.", e);
                    throw;
                }
            }
        }
    }
}
