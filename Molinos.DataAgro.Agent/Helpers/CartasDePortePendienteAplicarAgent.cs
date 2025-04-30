using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CartasDePortePendienteAplicar;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
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
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CartasDePortePendienteAplicarAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        private string ObtenerCodigoProveedor(string cuit, bool esCorredor = false)
        {
            var prefix = esCorredor ? "C" : "00";

            return prefix + cuit.Remove(cuit.Length - 1).Remove(0, 2);
        }

        public List<CcPpPendienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPendienteAplicarDto req)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                if (req.Proveedor == "20043159381")
                {
                    return new List<CcPpPendienteAplicarDto>();
                }
                return new List<CcPpPendienteAplicarDto>() { new CcPpPendienteAplicarDto() {
                    Contrato = "",
                    AgenteCompra = "Agente compra",
                    Cantidad = 500,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngresoString = "08-08-2020",
                    FechaNetoString = "08-08-2020",
                    FechaIngresoDate = DateTime.ParseExact("2020-08-08", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoDate = DateTime.ParseExact("2020-08-08", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                },
                new CcPpPendienteAplicarDto() {
                    Contrato = "",
                    AgenteCompra = "Agente compra",
                    Cantidad = 30000,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngresoString = "06-06-2020",
                    FechaNetoString = "06-06-2020",
                    FechaIngresoDate = DateTime.ParseExact("2020-06-06", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoDate = DateTime.ParseExact("2020-06-06", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Warrant = true,
                    Sustentable = false
                },
                new CcPpPendienteAplicarDto() {
                    Contrato = "",
                    AgenteCompra = "Agente compra",
                    Cantidad = 30000,
                    CartasPorte = "000585221852",
                    Centro = "centro",
                    Corredor = "corredor",
                    FechaIngresoString = "07-07-2020",
                    FechaNetoString = "07-07-2020",
                    FechaIngresoDate = DateTime.ParseExact("2020-07-07", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    FechaNetoDate = DateTime.ParseExact("2020-07-07", "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Sustentable = true,
                    Canje = true
                } }.OrderBy(a => a.FechaIngresoDate).ToList();
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

                        var centro = repositorio.Obtener<Centro>(x => x.Id.ToString() == req.Centro || x.CodigoSap == req.Centro);
                        var material = repositorio.Obtener<Material>(x => x.MaterialId.ToString() == req.Material || x.Codigo == req.Material);

                        var rq = new ZMprfcCcppPendienteAplicar
                        {
                            ImAgenteCompra = req.AgenteCompra == "1" ? "9952569841" : string.Empty,
                            ImCentro = centro.CodigoSap,
                            ImCorredor = req.Corredor != null ? ObtenerCodigoProveedor(req.Corredor, true) : string.Empty,
                            ImMaterial = material.Codigo,
                            ImProveedor = req.Proveedor != null ? ObtenerCodigoProveedor(req.Proveedor) : string.Empty
                        };

                        var valor = agent.ZMprfcCcppPendienteAplicar(rq);
                        var listaccpp = new List<CcPpPendienteAplicarDto>();
                        if (valor.ExSalida != null)
                        {
                            listaccpp = valor.ExSalida.Select(item =>
                                new CcPpPendienteAplicarDto()
                                {
                                    AgenteCompra = item.AgenteCompra,
                                    Cantidad = item.Cantidad,
                                    CartasPorte = item.Ccpp,
                                    Centro = centro.Descripcion,
                                    Corredor = item.Corredor,
                                    FechaIngresoString = item.FechaIngreso == "0000-00-00" ? "00-00-0000" : DateTime.ParseExact(item.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                                    FechaNetoString = item.FechaNeto == "0000-00-00" ? "00-00-0000" : DateTime.ParseExact(item.FechaNeto, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                                    Material = material.Descripcion,
                                    Proveedor = item.Proveedor,
                                    FechaIngresoDate = item.FechaIngreso == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FechaIngreso, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                    FechaNetoDate = item.FechaNeto == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FechaNeto, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                    Almacen = item.Almacen,
                                    Canje = item.Canje == "X",
                                    CD = item.CdCg == "X",
                                    Warrant = item.Warrant == "X",
                                    Sustentable = item.Sustentable == "X",
                                    Region = item.Region,
                                    Contrato = item.Contrato ?? "",
                                    KgContrato = item.KilosCont,
                                    EPA = item.Epa == "X",
                                    EUDR = item.Eudr == "X"
                                }).OrderBy(a => a.FechaIngresoDate).ToList();
                        }
                        return listaccpp;
                    }
                    else
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
                        var listaccpp = new List<CcPpPendienteAplicarDto>();
                        if (valor.EX_SALIDA != null)
                        {
                            listaccpp = valor.EX_SALIDA.Select(item =>
                                new CcPpPendienteAplicarDto()
                                {
                                    AgenteCompra = item.AGENTE_COMPRA,
                                    Cantidad = item.CANTIDAD,
                                    CartasPorte = item.CCPP,
                                    Centro = centro.Descripcion,
                                    Corredor = item.CORREDOR,
                                    FechaIngresoString = item.FECHA_INGRESO == "0000-00-00" ? "00-00-0000" : DateTime.ParseExact(item.FECHA_INGRESO, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                                    FechaNetoString = item.FECHA_NETO == "0000-00-00" ? "00-00-0000" : DateTime.ParseExact(item.FECHA_NETO, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy"),
                                    Material = material.Descripcion,
                                    Proveedor = item.PROVEEDOR,
                                    FechaIngresoDate = item.FECHA_INGRESO == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FECHA_INGRESO, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                    FechaNetoDate = item.FECHA_NETO == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(item.FECHA_NETO, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                                    Almacen = item.ALMACEN,
                                    Canje = item.CANJE == "X",
                                    CD = item.CD_CG == "X",
                                    Warrant = item.WARRANT == "X",
                                    Sustentable = item.SUSTENTABLE == "X",
                                    Region = item.REGION,
                                    Contrato = item.CONTRATO ?? "",
                                    KgContrato = item.KILOS_CONT,
                                    EPA = item.EPA == "X",
                                    EUDR = item.EUDR == "X"
                                }).OrderBy(a => a.FechaIngresoDate).ToList();
                        }
                        return listaccpp;
                    }
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