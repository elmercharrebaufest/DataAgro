using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.EnviarCapacidadProductivaSAP;
using Molinos.DataAgro.Entities.Dto;
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
    public class EnviarCapacidadProductivaSAPAgent : IEnviarCapacidadProductivaSAPAgent
    {
        public EnviarCapacidadProductivaSAPAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string EnviarCapacidadProductivaSAP(List<EnviarCapacidadProductivaSAPDto> capProd)
        {
            logger.Debug("EnviarCapacidadProductivaSAP - Iniciando");
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "OK";
            }
            try
            {
                var agent = new SI_ZMPWS_DATAAGRO_ACTU_CAP_PRODUCTIVAClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var porComercialyProv = capProd.GroupBy(item => new { item.UsuarioSAP, item.Cuit });

                var devolucion = new Z_MPRFC_ACT_CAPACIDAD_PRODResponse();

                foreach (var item in porComercialyProv)
                {
                    var rq = new Z_MPRFC_ACT_CAPACIDAD_PROD()
                    {
                        IM_COMERCIAL = item.First().UsuarioSAP,
                        IM_CUIT = item.First().Cuit,
                        IM_DETALLE = item.Select(detalle => new ZMPES6970
                        {
                            CUIT = detalle.Cuit,
                            MATNR = detalle.Material,
                            COSECHA = detalle.Campania,
                            CANTIDAD = detalle.Cantidad,
                            UNIME = detalle.UnidadMedida,
                            PORC = detalle.Porcentaje,
                            FECHA_ACT = DateTime.Now.ToString("yyyy-MM-dd")
                        }).ToArray(),
                        IM_INTAD = DateTime.Now.ToString("yyyy-MM-dd"),
                        IM_TLFNS = item.OrderByDescending(x => x.Campania).First().Campania
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug("EnviarCapacidadProductivaSAP - Se envió: " + rq.ToXml());

                    devolucion = agent.SI_ZMPWS_DATAAGRO_ACTU_CAP_PRODUCTIVA(rq);
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    logger.Debug("EnviarCapacidadProductivaSAP - Respuesta: " + devolucion.ToXml());
                }

                return devolucion.EX_MENSAJE;
            }
            catch (Exception e)
            {
                logger.Error("EnviarCapacidadProductiva - Error comunicación SAP. ", e);
                throw e;
            }

        }
    }
}