using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ComprasDetalle;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ComprasDetalleAgent : IComprasDetalleAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ComprasDetalleAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public List<CompraDetalleAgentDto> Comprar(string CUIT, string UsuarioComercial)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS_DETALLE() { IM_CUIT = new List<String>() { CUIT }.ToArray(), IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLE(rq);
            var compra = new List<CompraDetalleAgentDto>();
            if (devolucion.EX_SALIDA != null)
            {
                foreach (var dev in devolucion.EX_SALIDA)
                {
                    compra.Add(ConvertirADto(dev));
                }
            }
            return compra;
        }

        public List<CompraDetalleAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial)
        {
            var compra = new List<CompraDetalleAgentDto>();

            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    logger.Info("SAP sin PI - RFC ZMprfcDatosComprasDetalle");
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcDatosComprasDetalle() { ImCuit = CUIT.ToArray(), ImUsuario = UsuarioComercial };
                    var devolucion = agent.ZMprfcDatosComprasDetalle(rq);
                    logger.Info("SAP sin PI - RFC ZMprfcDatosComprasDetalle");
                    if (devolucion.ExSalida != null)
                    {
                        foreach (var dev in devolucion.ExSalida)
                        {
                            compra.Add(ConvertirADtoSinPI(dev));
                        }
                    }
                    return compra;

                }
                else
                {
                    SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_DATOS_COMPRAS_DETALLE() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = UsuarioComercial };
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLE(rq);

                    if (devolucion.EX_SALIDA != null)
                    {
                        foreach (var dev in devolucion.EX_SALIDA)
                        {
                            compra.Add(ConvertirADto(dev));
                        }
                    }
                    return compra;
                }
            }
            catch (Exception e)
            {
                logger.Error($"Error obtener compras detalle con el usuario {UsuarioComercial} y CUITs {CUIT.ToJson()}");
                logger.Error(e);
                return compra;
            }
        }
        private CompraDetalleAgentDto ConvertirADtoSinPI(Zmpes5620 dev)
        {
            var compraAgent = new CompraDetalleAgentDto
            {
                COSECHA = dev.Cosecha,
                MATERIAL = dev.Material,
                VENDEDOR = dev.Vendedor,
                CLASE_DOC = dev.ClaseDoc,
                CLASIFICACION = dev.Clasificacion,
                CONTRATO = dev.Contrato,
                CORREDOR = dev.Corredor,
                FECHA = dev.Fecha,
                PEND_APLICAR = dev.PendAplicar,
                PEND_FIJAR = dev.PendFijar,
                TN_AMPLIADAS = dev.TnAmpliadas,
                TN_ANULADAS = dev.TnAnuladas,
                TN_APLICADAS = dev.TnAplicadas,
                TN_CONTRATO = dev.TnContrato,
                TN_FIJADAS = dev.TnFijadas,
                FECHA_HASTA = dev.FechaHasta,
                FECHA_DESDE = dev.FechaDesde,
                CENTRO = dev.Centro
            };
            return compraAgent;
        }
        private CompraDetalleAgentDto ConvertirADto(ZMPES5620 dev)
        {
            var compraAgent = new CompraDetalleAgentDto
            {
                COSECHA = dev.COSECHA,
                MATERIAL = dev.MATERIAL,
                VENDEDOR = dev.VENDEDOR,
                CLASE_DOC = dev.CLASE_DOC,
                CLASIFICACION = dev.CLASIFICACION,
                CONTRATO = dev.CONTRATO,
                CORREDOR = dev.CORREDOR,
                FECHA = dev.FECHA,
                PEND_APLICAR = dev.PEND_APLICAR,
                PEND_FIJAR = dev.PEND_FIJAR,
                TN_AMPLIADAS = dev.TN_AMPLIADAS,
                TN_ANULADAS = dev.TN_ANULADAS,
                TN_APLICADAS = dev.TN_APLICADAS,
                TN_CONTRATO = dev.TN_CONTRATO,
                TN_FIJADAS = dev.TN_FIJADAS,
                FECHA_HASTA = dev.FECHA_HASTA,
                FECHA_DESDE = dev.FECHA_DESDE,
                CENTRO = dev.CENTRO
            };
            return compraAgent;
        }
    }
}
