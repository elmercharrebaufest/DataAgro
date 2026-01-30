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
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class CriterioCDWarrantAgent : ICriterioCDWarrantAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CriterioCDWarrantAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<BasicoContrato> ConsultarContratoWarrant(DateTime desde, DateTime hasta)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var contrato = repositorio.Listar<Contrato, BasicoContrato>(x => new BasicoContrato
                {
                    ContratoSAP = x.ContratoSAP,
                }, x => DbFunctions.TruncateTime(x.Fecha) >= desde && DbFunctions.TruncateTime(x.Fecha) <= hasta);
                return contrato;
            }
            else
            {
                try
                {

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    List<Material> materiales = repositorio.Listar<Material>();
                    var rq = new ZMprfcCdWarrants()
                    {
                        ImFechaDesde = desde.ToString("yyyy-MM-dd"),
                        ImFechaHasta = hasta.ToString("yyyy-MM-dd")
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    //logger.Debug(rq.ToXml());

                    var valor = agent.ZMprfcCdWarrants(rq);
                    var listaResp = valor.ExSalida.Select(x => new BasicoContrato
                    {
                        ContratoSAP = x.Contrato,
                        Material = materiales.FirstOrDefault(a => a.Codigo == x.Material) == null ? x.Material : materiales.FirstOrDefault(a => a.Codigo == x.Material).Descripcion,
                        Cantidad = (double)x.Kilos,
                        Fecha = DateTime.Parse(x.Fecha),
                        Moneda = x.Moneda
                    }).ToList();

                    //logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return listaResp;
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
