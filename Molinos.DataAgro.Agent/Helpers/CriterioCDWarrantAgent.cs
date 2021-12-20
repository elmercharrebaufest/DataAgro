using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CriterioCDWarrant;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class CriterioCDWarrantAgent : ICriterioCDWarrantAgent
    {
        public CriterioCDWarrantAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public List<BasicoContrato> ConsultarContratoWarrant(DateTime desde, DateTime hasta)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var contrato = repositorio.Listar<Contrato, BasicoContrato>(x => new BasicoContrato {
                    ContratoSAP = x.ContratoSAP,                    
                }, x => DbFunctions.TruncateTime(x.Fecha) >= desde && DbFunctions.TruncateTime(x.Fecha) <= hasta);
                return contrato;
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_CD_WARRANTSClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    List<Material> materiales = repositorio.Listar<Material>();
                    var rq = new Z_MPRFC_CD_WARRANTS()
                    {
                        IM_FECHA_DESDE = desde.ToString("yyyy-MM-dd"),
                        IM_FECHA_HASTA = hasta.ToString("yyyy-MM-dd")
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    //logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_CD_WARRANTS(rq);
                    var listaResp = valor.EX_SALIDA.Select(x=> new BasicoContrato {
                        ContratoSAP = x.CONTRATO,
                        Material = materiales.FirstOrDefault(a => a.Codigo == x.MATERIAL) == null ? x.MATERIAL : materiales.FirstOrDefault(a => a.Codigo == x.MATERIAL).Descripcion,
                        Cantidad = (double)x.KILOS,
                        Fecha = DateTime.Parse(x.FECHA),
                        Moneda = x.MONEDA
                    }).ToList();

                    //logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return listaResp;

                }catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
