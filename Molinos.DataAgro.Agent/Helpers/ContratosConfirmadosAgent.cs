using NLog;
using Molinos.DataAgro.Agent.ContratosConfirmados;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ContratosConfirmadosAgent : IContratosConfirmadosAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ContratosConfirmadosAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<Contrato> ConfirmarContrato(Contrato contrato)
        {
            logger.Debug("Eviando Contrato Nro: " + contrato.Id);
            var contratos = new List<Contrato>();
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                contratos.Add(new Contrato { ContratoSAP = "1234560", Fecha = DateTime.Now });
                return contratos;
            }
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    logger.Debug("Cargando contrato");
                    var rq = new ZMprfcContratosConfirmados()
                    {
                        ImFecha = new string[] { contrato.Fecha.ToString("yyyy-MM-dd") }
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcContratosConfirmados(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    foreach (var item in devolucion.ExSalida)
                    {
                        Contrato contratoTemp = new Contrato
                        {
                            ContratoSAP = item.Contrnum,
                            Fecha = String.IsNullOrEmpty(item.FechaConfir) ? DateTime.Now : Convert.ToDateTime(item.FechaConfir)
                        };
                    }
                    return contratos;
                }
                else
                {
                    SI_ZMPWS_DATAAGRO_CONTRATOS_CONFIRMADOSClient agent = new SI_ZMPWS_DATAAGRO_CONTRATOS_CONFIRMADOSClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    logger.Debug("Cargando contrato");
                    var rq = new Z_MPRFC_CONTRATOS_CONFIRMADOS()
                    {
                        IM_FECHA = new string[] { contrato.Fecha.ToString("yyyy-MM-dd") }
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_CONTRATOS_CONFIRMADOS(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    foreach (var item in devolucion.EX_SALIDA)
                    {
                        Contrato contratoTemp = new Contrato
                        {
                            ContratoSAP = item.CONTRNUM,
                            Fecha = String.IsNullOrEmpty(item.FECHA_CONFIR) ? DateTime.Now : Convert.ToDateTime(item.FECHA_CONFIR)
                        };
                    }
                    return contratos;
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Error comunicacion SAP al confirmar contrato.");
                throw;
            }
        }
    }
}
