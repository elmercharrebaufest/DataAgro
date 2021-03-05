using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CartasDePortePendienteAplicar;
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
    public class CartasDePortePendienteAplicarAgent : ICartasDePortePendienteAplicarAgent
    {
        public CartasDePortePendienteAplicarAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        private string ObtenerCodigoProveedor(string cuit, bool esCorredor = false)
        {
            var prefix = esCorredor ? "C" : "00";
            
            return prefix + cuit.Remove(cuit.Length - 1).Remove(0, 2);
        }
        public List<CcPpPerndienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<CcPpPerndienteAplicarDto>() { new CcPpPerndienteAplicarDto() {
                    AgenteCompra = "Agente compra",
                    Cantidad = 10,
                    CartasPorte = "Carta de porte",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngreso = "10/02/2021",
                    FechaNeto = "12/01/2021"
                } };
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_CCPP_PEND_APLICARClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var centro = repositorio.Obtener<Centro>(x => x.Id.ToString() == req.Centro);
                    var material = repositorio.Obtener<Material>(x => x.MaterialId.ToString() == req.Material);

                    var rq = new Z_MPRFC_CCPP_PENDIENTE_APLICAR
                    {
                        IM_AGENTE_COMPRA = req.AgenteCompra == "1" ? "9952569841" : string.Empty,
                        IM_CENTRO = centro.CodigoSap,
                        IM_CORREDOR = req.Corredor != null ? ObtenerCodigoProveedor(req.Corredor, true) : string.Empty,
                        IM_MATERIAL = material.Codigo,
                        IM_PROVEEDOR = req.Proveedor != null ? ObtenerCodigoProveedor(req.Proveedor) : string.Empty
                    };

                    var valor = agent.SI_ZMPWS_DATAAGRO_CCPP_PEND_APLICAR(rq);
                    var listaccpp = valor.EX_SALIDA.Select(item =>
                        new CcPpPerndienteAplicarDto()
                        {
                            AgenteCompra = item.AGENTE_COMPRA,
                            Cantidad = item.CANTIDAD,
                            CartasPorte = item.CCPP,
                            Centro = item.CENTRO,
                            Corredor = item.CORREDOR,
                            FechaIngreso = item.FECHA_INGRESO,
                            FechaNeto = item.FECHA_NETO
                        }).ToList();

                    return listaccpp;
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
