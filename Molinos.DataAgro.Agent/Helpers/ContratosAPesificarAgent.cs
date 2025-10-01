using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ContratosAPesificar;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
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
    public class ContratosAPesificarAgent : IContratosAPesificarAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ContratosAPesificarAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }


        public List<PesificarAgentDto> ConsultarProveedores(List<string> cuits, bool esBuscarTodos)
        {
            string lastBeforeTheError = string.Empty;
            try
            {
                var pesificado = new List<PesificarAgentDto>();
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcListaProveedores { ImProveedores = cuits.ToArray() };

                    if (!esBuscarTodos)
                    {
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                    }

                    lastBeforeTheError = rq.ToXml();

                    var devolucion = agent.ZMprfcListaProveedores(rq);
                    if (devolucion.ExSalida != null)
                    {
                        foreach (var dev in devolucion.ExSalida)
                        {
                            pesificado.Add(ConvertirADtoSinPI(dev));
                        }
                    }
                    return pesificado;
                }
                else
                {
                    var agent = new SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORESClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var rq = new Z_MPRFC_LISTA_PROVEEDORES { IM_PROVEEDORES = cuits.ToArray() };

                    if (!esBuscarTodos)
                    {
                        var log = new Log
                        {
                            Fecha = DateTime.Now,
                            Xml = rq.ToXml()
                        };
                        var logId = repositorio.Agregar(log);
                        repositorio.GuardarCambios();
                    }

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORES(rq);
                    if (devolucion.EX_SALIDA != null)
                    {
                        foreach (var dev in devolucion.EX_SALIDA)
                        {
                            pesificado.Add(ConvertirADto(dev));
                        }
                    }

                    return pesificado;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Error en ContratosAPesificarAgent, ConsultarProveedores(). Últimos CUITS antes del error: {lastBeforeTheError}", ex);
                throw;
            }
        }

        public List<PesificarAgentDto> ConsultarPorUnProveedor(string cuit)
        {
            try
            {
                var pesificado = new List<PesificarAgentDto>();
                var prov = new List<string> { cuit };
                pesificado = ConsultarProveedores(prov, false);
                return pesificado;

                /*
                var agent = new SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORESClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                var prov = new List<string> { cuit };
                var rq = new Z_MPRFC_LISTA_PROVEEDORES { IM_PROVEEDORES = prov.ToArray() };
                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORES(rq);
                if (devolucion.EX_SALIDA != null)
                {
                    foreach (var dev in devolucion.EX_SALIDA)
                    {
                        pesificado.Add(ConvertirADto(dev));
                    }
                }

                return pesificado;
                */
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        public List<PesificarAgentDto> ConsultarTodo(List<string> cuits)
        {
            try
            {

                logger.Debug("Cuits pesificados " + cuits.ToXml());
                var pesificado = new List<PesificarAgentDto>();
                pesificado = ConsultarProveedores(cuits, true);
                logger.Debug("Pesificado total " + pesificado.Count());
                return pesificado;

                /*
                var agent = new SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORESClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                logger.Debug("Cuits pesificados " + cuits.ToXml());

                var rq = new Z_MPRFC_LISTA_PROVEEDORES {
                    IM_PROVEEDORES = cuits.ToArray()
                };

                var devolucion = agent.SI_ZMPWS_DATAAGRO_LISTA_PROVEEDORES(rq);
                var pesificado = new List<PesificarAgentDto>();
                if (devolucion.EX_SALIDA != null)
                {
                    foreach (var dev in devolucion.EX_SALIDA)
                    {
                        pesificado.Add(ConvertirADto(dev));
                    }
                }

                logger.Debug("Pesificado total " + pesificado.Count());
                return pesificado;
                */
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        private PesificarAgentDto ConvertirADto(ZMPES6360 dev)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var pesificado = new PesificarAgentDto
            {
                Material = dev.MATERIAL,
                Contrato = dev.CONTRATO,
                CantidadPendiente = (int)dev.CANT_PENDIENTE,
                Comercial = dev.COMERCIAL,
                Fijacion = dev.FIJACION,
                FechaFijacion = dev.FECHA_FIJACION == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FECHA_FIJACION, "yyyy-MM-dd", provider),
                FechaHastaDolarizado = dev.FECHA_HASTA_DOL == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FECHA_HASTA_DOL, "yyyy-MM-dd", provider),
                FechaUltimaAplicacion = dev.FECHA_ULT_APLI == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FECHA_ULT_APLI, "yyyy-MM-dd", provider),
                DolarizadoNoProductor = dev.DOLARIZADO_NO_PROD != "NO",
                CuitCorredor = dev.CUIT_CORREDOR,
                CuitVendedor = dev.CUIT_VENDEDOR,
                DolarizadoExpress = dev.DOLARIZADO_EXPRESS != "NO",
                Moneda = dev.MONEDA,
                KgNoPesificable = dev.PES_NO_VEN < 0 ? (dev.PES_NO_VEN + dev.PES_VENCIDA) : dev.PES_NO_VEN,
                KgVencimientoPesificable = dev.PES_VENCIDA < 0 ? 0 : dev.PES_VENCIDA,
                KgTotales = dev.PES_NO_VEN + dev.PES_VENCIDA,
                Precio = dev.PRECIO,
                NombreCorredor = dev.NOM_CORREDOR,
                NombreVendedor = dev.NOM_VEND,
                Unidad = dev.UNIDAD,
                Dolarizado = dev.DOLARIZADO != "NO",
                Clasificacion = dev.CLASIFICACION,
                Anticipo = dev.ANTICIPO,
                Status = dev.STATUS == "" ? "S" : dev.STATUS,
                Cesion = dev.CESION == "X",
                Cantidad = dev.CANTIDAD,
                CantidadLiquidada = dev.CANT_LIQUIDADA,
                CantidadRecibida = dev.CANT_RECIBIDA,
                ConPrecio = dev.CON_PRECIO,

            };
            var fecha = new DateTime(1753, 1, 1);
            if (pesificado.FechaFijacion.HasValue && pesificado.FechaFijacion.Value < fecha)
            {

                logger.Error($"Reporte Pesificado fecha Fijacion:  {pesificado.ToJson()}");
                pesificado.FechaFijacion = (DateTime?)null;
            }
            if (pesificado.FechaHastaDolarizado.HasValue && pesificado.FechaHastaDolarizado.Value < fecha)
            {

                logger.Error($"Reporte Pesificado FechaHastaDolarizado:  {pesificado.ToJson()}");
                pesificado.FechaHastaDolarizado = (DateTime?)null;
            }
            if (pesificado.FechaUltimaAplicacion.HasValue && pesificado.FechaUltimaAplicacion.Value < fecha)
            {

                logger.Error($"Reporte Pesificado FechaUltimaAplicacion:  {pesificado.ToJson()}");
                pesificado.FechaUltimaAplicacion = (DateTime?)null;
            }
            logger.Debug("PESIFICADOS 3 - ConvertirADto");

            return pesificado;
        }

        private PesificarAgentDto ConvertirADtoSinPI(Zmpes6360 dev)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            var pesificado = new PesificarAgentDto
            {
                Material = dev.Material,
                Contrato = dev.Contrato,
                CantidadPendiente = (int)dev.CantPendiente,
                Comercial = dev.Comercial,
                Fijacion = dev.Fijacion,
                FechaFijacion = dev.FechaFijacion == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FechaFijacion, "yyyy-MM-dd", provider),
                FechaHastaDolarizado = dev.FechaHastaDol == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FechaHastaDol, "yyyy-MM-dd", provider),
                FechaUltimaAplicacion = dev.FechaUltApli == "0000-00-00" ? (DateTime?)null : DateTime.ParseExact(dev.FechaUltApli, "yyyy-MM-dd", provider),
                DolarizadoNoProductor = dev.DolarizadoNoProd != "NO",
                CuitCorredor = dev.CuitCorredor,
                CuitVendedor = dev.CuitVendedor,
                DolarizadoExpress = dev.DolarizadoExpress != "NO",
                Moneda = dev.Moneda,
                KgNoPesificable = dev.PesNoVen < 0 ? (dev.PesNoVen + dev.PesVencida) : dev.PesNoVen,
                KgVencimientoPesificable = dev.PesVencida < 0 ? 0 : dev.PesVencida,
                KgTotales = dev.PesNoVen + dev.PesVencida,
                Precio = dev.Precio,
                NombreCorredor = dev.NomCorredor,
                NombreVendedor = dev.NomVend,
                Unidad = dev.Unidad,
                Dolarizado = dev.Dolarizado != "NO",
                Clasificacion = dev.Clasificacion,
                Anticipo = dev.Anticipo,
                Status = dev.Status == "" ? "S" : dev.Status,
                Cesion = dev.Cesion == "X",
                Cantidad = dev.Cantidad,
                CantidadLiquidada = dev.CantLiquidada,
                CantidadRecibida = dev.CantRecibida,
                ConPrecio = dev.ConPrecio,

            };
            var fecha = new DateTime(1753, 1, 1);
            if (pesificado.FechaFijacion.HasValue && pesificado.FechaFijacion.Value < fecha)
            {

                logger.Error($"Reporte Pesificado fecha Fijacion:  {pesificado.ToJson()}");
                pesificado.FechaFijacion = (DateTime?)null;
            }
            if (pesificado.FechaHastaDolarizado.HasValue && pesificado.FechaHastaDolarizado.Value < fecha)
            {

                logger.Error($"Reporte Pesificado FechaHastaDolarizado:  {pesificado.ToJson()}");
                pesificado.FechaHastaDolarizado = (DateTime?)null;
            }
            if (pesificado.FechaUltimaAplicacion.HasValue && pesificado.FechaUltimaAplicacion.Value < fecha)
            {

                logger.Error($"Reporte Pesificado FechaUltimaAplicacion:  {pesificado.ToJson()}");
                pesificado.FechaUltimaAplicacion = (DateTime?)null;
            }
            logger.Debug("PESIFICADOS 3 - ConvertirADto");

            return pesificado;
        }


    }
}
