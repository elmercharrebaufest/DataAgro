using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CapacidadProductivaDisponible;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                        CAP_PROD= 2123,CAP_PROD_PORC= 100,COMPRAS_ACT= 12312312,COMPRAS_ANT= 23132,CUIT=cuit,MATERIAL="Soja",UNIDAD= "KG"
                    }
                };
            }
            else
            {
                try
                {
                    var resultado = new List<CapacidadProductivaPendienteDto>();
                    SI_ZMPWS_DATAAGRO_CAPAC_PRODUCTIVA_DISPOClient agent = new SI_ZMPWS_DATAAGRO_CAPAC_PRODUCTIVA_DISPOClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;

                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_CAPAC_PRODUCTIVA_DISPO()
                    {
                        IM_CUIT = cuit
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_CAPAC_PRODUCTIVA_DISPO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();
                    var materiales = repositorio.Listar<Material>().ToList();
                    if (valor != null && valor.EX_SALIDA != null && valor.EX_SALIDA.Count() > 0)
                    {
                        foreach (var a in valor.EX_SALIDA)
                        {
                            var item = new CapacidadProductivaPendienteDto
                            {
                                CAP_PROD = a.CAP_PROD,
                                CAP_PROD_PORC = a.CAP_PROD_PORC,
                                COMPRAS_ACT = a.COMPRAS_ACT,
                                COMPRAS_ANT = a.COMPRAS_ANT,
                                CUIT = a.CUIT,
                                MATERIAL = materiales.Where(x => x.Codigo == a.MATERIAL).Single().Descripcion,
                                UNIDAD = a.UNIDAD,
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
