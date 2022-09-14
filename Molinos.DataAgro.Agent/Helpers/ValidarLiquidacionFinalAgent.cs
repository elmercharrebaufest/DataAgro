using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ValidarLiquidacionFinal;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ValidarLiquidacionFinalAgent : IValidarLiquidacionFinalAgent
    {
        private readonly IContratosParaFijacionAgent contratosParaFijacionAgent;
        private readonly ITipoDeCambioAgent tipoCambioAgent;

        public ValidarLiquidacionFinalAgent(ILogger logger, IRepositorio repositorio, IContratosParaFijacionAgent contratosParaFijacionAgent, ITipoDeCambioAgent tipoCambioAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratosParaFijacionAgent = contratosParaFijacionAgent;
            this.tipoCambioAgent = tipoCambioAgent;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
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
                    SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_FINALClient agent = new SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_FINALClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_VALIDAR_LIQ_FINAL()
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

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_VALIDAR_LIQ_FINAL(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    return devolucion.EX_MENSAJE;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
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
