using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ContratoKgPendientes;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class ContratoKgPendienteAgent : IContratoKgPendienteAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ContratoKgPendienteAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<ContratoKgPendiente> Consultar(List<ContratoKgPendiente> contratos)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                for (int i = 0; i < contratos.Count(); i++)
                {
                    contratos[i].KgPendiente = (i + 1) * 100000;
                }
                return contratos;
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

                        var rq = new ZMprfcIndicarKgPendientes()
                        {
                            ImContrato = contratos.Select(a => a.ContratoSAP).ToArray()
                        };
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        //logger.Debug(rq.ToXml());

                        var valor = agent.ZMprfcIndicarKgPendientes(rq);
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();

                        foreach (var contrato in contratos)
                        {
                            var contratoSAP = valor.ExContrato.Where(a => a.Contrato == contrato.ContratoSAP).SingleOrDefault();
                            contrato.KgPendiente = Decimal.ToInt32(contratoSAP == null ? 0 : contratoSAP.KilosCont);
                        }
                        return contratos;
                    }
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_INDICAR_KG_PENDIENTESClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_INDICAR_KG_PENDIENTES()
                        {
                            IM_CONTRATO = contratos.Select(a => a.ContratoSAP).ToArray()
                        };
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        //logger.Debug(rq.ToXml());

                        var valor = agent.SI_ZMPWS_DATAAGRO_INDICAR_KG_PENDIENTES(rq);
                        //logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();

                        foreach (var contrato in contratos)
                        {
                            var contratoSAP = valor.EX_CONTRATO.Where(a => a.CONTRATO == contrato.ContratoSAP).SingleOrDefault();
                            contrato.KgPendiente = Decimal.ToInt32(contratoSAP == null ? 0 : contratoSAP.KILOS_CONT);
                        }
                        return contratos;
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
