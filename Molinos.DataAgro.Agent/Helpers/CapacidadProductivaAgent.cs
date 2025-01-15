using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CapacidadProductiva;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class CapacidadProductivaAgent : ICapacidadProductivaAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CapacidadProductivaAgent(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public string ObtenerCapacidadProductiva(string cuit, decimal cantidad, string centro, string cosecha, string material)
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

                        var rq = new ZMprfcCapacidadProductiva()
                        {
                            ImCuit = cuit,
                            ImCantidad = cantidad,
                            ImCentro = centro,
                            ImCosecha = cosecha,
                            ImMaterial = material
                        };

                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        logger.Debug(rq.ToXml());

                        var valor = agent.ZMprfcCapacidadProductiva(rq);
                        logger.Info("SAP sin PI - RFC ZMprfcCapacidadProductiva");
                        logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();

                        return valor.ExMensaje;
                    }
                    else
                    {
                        SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVAClient agent = new SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVAClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_CAPACIDAD_PRODUCTIVA()
                        {
                            IM_CUIT = cuit,
                            IM_CANTIDAD = cantidad,
                            IM_CENTRO = centro,
                            IM_COSECHA = cosecha,
                            IM_MATERIAL = material
                        };

                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        logger.Debug(rq.ToXml());

                        var valor = agent.SI_ZMPWS_DATAAGRO_CAPACIDAD_PRODUCTIVA(rq);
                        logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();

                        return valor.EX_MENSAJE;
                    }
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
