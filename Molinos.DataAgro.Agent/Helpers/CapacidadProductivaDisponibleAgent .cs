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

namespace Molinos.DataAgro.Agent
{
    public class CapacidadProductivaDisponibleAgent : ICapacidadProductivaDisponibleAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CapacidadProductivaDisponibleAgent(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }

        public List<CapacidadProductivaPendienteDto> ObtenerCapacidadProductivaPendiente(string cuit)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<CapacidadProductivaPendienteDto> {
                    new CapacidadProductivaPendienteDto{
                        CAP_PROD = 2123,
                        CAP_PROD_PORC = 100,
                        COMPRAS_ACT = 12312312,
                        COMPRAS_ANT = 23132,
                        CUIT = cuit,
                        MATERIAL = "Soja",
                        UNIDAD = "KG"
                    }
                };
            }
            else
            {
                try
                {

                    var resultado = new List<CapacidadProductivaPendienteDto>();

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcCapacProductivaDispo()
                    {
                        ImCuit = cuit
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.ZMprfcCapacProductivaDispo(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    var materiales = repositorio.Listar<Material>().ToList();
                    if (valor != null && valor.ExSalida != null && valor.ExSalida.Count() > 0)
                    {
                        foreach (var a in valor.ExSalida)
                        {
                            var item = new CapacidadProductivaPendienteDto
                            {
                                CAP_PROD = a.CapProd,
                                CAP_PROD_PORC = a.CapProdPorc,
                                COMPRAS_ACT = a.ComprasAct,
                                COMPRAS_ANT = a.ComprasAnt,
                                CUIT = a.Cuit,
                                MATERIAL = materiales.Where(x => x.Codigo == a.Material).Single().Descripcion,
                                UNIDAD = a.Unidad,
                            };
                            resultado.Add(item);
                        }
                    }
                    return resultado;

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
