using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ListaCBUProveedor;
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
    public class ListaCBUProveedorAgent : IListaCBUProveedorAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ListaCBUProveedorAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<PagoCBUDto> ListarCBU(string cuit, string filtro)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<PagoCBUDto>() { new PagoCBUDto() {
                    Cbu = "1234567898",
                    Koinh = "456",
                    Pago = "456-1234567898 AFIP Santander",
                } };
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

                        var rq = new ZMprfcListaCbuProveedor
                        {
                            ImCuit = new Zmpes6280[] { new Zmpes6280 { Cuit = cuit } }
                        };
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        logger.Debug(rq.ToXml());

                        var valor = agent.ZMprfcListaCbuProveedor(rq);
                        var listaCbus = new List<PagoCBUDto>();

                        foreach (var item in valor.ExCbu)
                        {
                            var cbus = new PagoCBUDto()
                            {
                                Cbu = item.Bankn,
                                Cuit = item.Cuit,
                                Koinh = item.Koinh,
                                Pago = item.Koinh + "-" + item.Bankn + " " + item.Bvtyp + " " + item.Banka,
                                NombreBanco = item.Bvtyp
                            };
                            listaCbus.Add(cbus);
                        }

                        logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();
                        return listaCbus.Where(x => x.Pago.Contains(filtro)).OrderBy(x => x.Cbu).ToList();
                    }
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_LISTA_CBU_PROVEEDORClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_LISTA_CBU_PROVEEDOR
                        {
                            IM_CUIT = new ZMPES6280[] { new ZMPES6280 { CUIT = cuit } }
                        };
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                        logger.Debug(rq.ToXml());

                        var valor = agent.SI_ZMPWS_DATAAGRO_LISTA_CBU_PROVEEDOR(rq);
                        var listaCbus = new List<PagoCBUDto>();

                        foreach (var item in valor.EX_CBU)
                        {
                            var cbus = new PagoCBUDto()
                            {
                                Cbu = item.BANKN,
                                Cuit = item.CUIT,
                                Koinh = item.KOINH,
                                Pago = item.KOINH + "-" + item.BANKN + " " + item.BVTYP + " " + item.BANKA,
                                NombreBanco = item.BVTYP
                            };
                            listaCbus.Add(cbus);
                        }

                        logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();
                        return listaCbus.Where(x => x.Pago.Contains(filtro)).OrderBy(x => x.Cbu).ToList();
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
