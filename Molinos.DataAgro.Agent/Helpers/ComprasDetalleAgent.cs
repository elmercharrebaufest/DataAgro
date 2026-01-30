using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
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

        public List<CompraDetalleAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial)
        {
            var compra = new List<CompraDetalleAgentDto>();

            try
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new ZMprfcDatosComprasDetalle() { ImCuit = CUIT.ToArray(), ImUsuario = UsuarioComercial };
                logger.Info(rq.ToXml());

                var devolucion = agent.ZMprfcDatosComprasDetalle(rq);
                logger.Info(devolucion.ToXml());
                if (devolucion.ExSalida != null)
                {
                    foreach (var dev in devolucion.ExSalida)
                    {
                        compra.Add(ConvertirADtoSinPI(dev));
                    }
                }
                return compra;

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

    }
}
