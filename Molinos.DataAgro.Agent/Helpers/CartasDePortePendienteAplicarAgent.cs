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
using System.Globalization;
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
                if (req.Proveedor == "20043159381")
                {
                    return new List<CcPpPerndienteAplicarDto>();
                }
                return new List<CcPpPerndienteAplicarDto>() { new CcPpPerndienteAplicarDto() {
                    AgenteCompra = "Agente compra",
                    Cantidad = 10,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngreso = "2020-08-08",
                    FechaNeto = "2020-08-08",
                    FechaIngresoFecha = DateTime.ParseExact("2020-08-08", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoFecha = DateTime.ParseExact("2020-08-08", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                },
                new CcPpPerndienteAplicarDto() {
                    AgenteCompra = "Agente compra",
                    Cantidad = 100000,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngreso = "2020-06-06",
                    FechaNeto = "2020-06-06",
                    FechaIngresoFecha = DateTime.ParseExact("2020-06-06", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoFecha = DateTime.ParseExact("2020-06-06", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Warrant = true,
                    Sustentable = false
                },
                new CcPpPerndienteAplicarDto() {
                    AgenteCompra = "Agente compra",
                    Cantidad = 100000,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngreso = "2020-07-07",
                    FechaNeto = "2020-07-07",
                    FechaIngresoFecha = DateTime.ParseExact("2020-07-07", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoFecha = DateTime.ParseExact("2020-07-07", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Sustentable = true,
                    Canje = true
                } }.OrderBy(a => a.FechaIngresoFecha).ToList();
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_CCPP_PEND_APLICARClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var centro = repositorio.Obtener<Centro>(x => x.Id.ToString() == req.Centro || x.CodigoSap == req.Centro);
                    var material = repositorio.Obtener<Material>(x => x.MaterialId.ToString() == req.Material || x.Codigo == req.Material);

                    var rq = new Z_MPRFC_CCPP_PENDIENTE_APLICAR
                    {
                        IM_AGENTE_COMPRA = req.AgenteCompra == "1" ? "9952569841" : string.Empty,
                        IM_CENTRO = centro.CodigoSap,
                        IM_CORREDOR = req.Corredor != null ? ObtenerCodigoProveedor(req.Corredor, true) : string.Empty,
                        IM_MATERIAL = material.Codigo,
                        IM_PROVEEDOR = req.Proveedor != null ? ObtenerCodigoProveedor(req.Proveedor) : string.Empty
                    };

                    var valor = agent.SI_ZMPWS_DATAAGRO_CCPP_PEND_APLICAR(rq);
                    var listaccpp = new List<CcPpPerndienteAplicarDto>();
                    if (valor.EX_SALIDA != null)
                    {
                        listaccpp = valor.EX_SALIDA.Select(item =>
                            new CcPpPerndienteAplicarDto()
                            {
                                AgenteCompra = item.AGENTE_COMPRA,
                                Cantidad = item.CANTIDAD,
                                CartasPorte = item.CCPP,
                                Centro = centro.Descripcion,
                                Corredor = item.CORREDOR,
                                FechaIngreso = item.FECHA_INGRESO,
                                FechaNeto = item.FECHA_NETO,
                                Material = material.Descripcion,
                                Proveedor = item.PROVEEDOR,
                                FechaIngresoFecha = item.FECHA_INGRESO == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FECHA_INGRESO, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                FechaNetoFecha = item.FECHA_NETO == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FECHA_NETO, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                Almacen = item.ALMACEN,
                                Canje = item.CANJE == "X",
                                CD = item.CD_CG == "X",
                                Warrant = item.WARRANT == "X",
                                Sustentable = item.SUSTENTABLE == "X",
                                Region = item.REGION
                            }).OrderBy(a => a.FechaIngresoFecha).ToList();
                    }
                    return listaccpp;
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    throw;
                }
            }
        }

    }
}
