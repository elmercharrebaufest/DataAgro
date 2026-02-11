using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EnviarCapacidadProductivaSAPAgent : IEnviarCapacidadProductivaSAPAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public EnviarCapacidadProductivaSAPAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string EnviarCapacidadProductivaSAP(List<EnviarCapacidadProductivaSAPDto> capProd)
        {
            logger.Debug("EnviarCapacidadProductivaSAP - Iniciando");
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "OK";
            }
            try
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var porComercialyProv = capProd.GroupBy(item => new { item.UsuarioSAP, item.Cuit });

                var devolucion = new ZMprfcActCapacidadProdResponse();

                foreach (var item in porComercialyProv)
                {
                    var rq = new ZMprfcActCapacidadProd()
                    {
                        ImComercial = item.First().UsuarioSAP,
                        ImCuit = item.First().Cuit,
                        ImDetalle = item.Select(detalle => new Zmpes6970
                        {
                            Cuit = ObtenerCodigoProveedor(detalle.Cuit,false),
                            Matnr = detalle.Material,
                            Cosecha = detalle.Campania,
                            Cantidad = detalle.Cantidad,
                            Unime = detalle.UnidadMedida,
                            Porc = detalle.Porcentaje,
                            FechaAct = DateTime.Now.ToString("yyyy-MM-dd")
                        }).ToArray(),
                        ImIntad = DateTime.Now.ToString("yyyy-MM-dd"),
                        ImTlfns = item.OrderByDescending(x => x.Campania).First().Campania
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug("EnviarCapacidadProductivaSAP - Se envió: " + rq.ToXml());

                    devolucion = agent.ZMprfcActCapacidadProd(rq);
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    logger.Debug("EnviarCapacidadProductivaSAP - Respuesta: " + devolucion.ToXml());
                }

                return devolucion.ExMensaje;

            }
            catch (Exception e)
            {
                logger.Error(e, "EnviarCapacidadProductiva - Error comunicación SAP. ");
                throw;
            }
        }

        private string ObtenerCodigoProveedor(string cuit, bool esCorredor = false)
        {
            var prefix = esCorredor ? "C" : "00";

            return prefix + cuit.Remove(cuit.Length - 1).Remove(0, 2);
        }
    }
}