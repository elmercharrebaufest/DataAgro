using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ListaCBUProveedor;
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
        public ListaCBUProveedorAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

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
                            Pago = item.KOINH+"-"+item.BANKN + " " + item.BVTYP + " " + item.BANKA,
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
