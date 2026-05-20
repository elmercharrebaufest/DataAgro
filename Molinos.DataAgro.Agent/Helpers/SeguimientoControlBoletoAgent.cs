using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces.Agent;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class SeguimientoControlBoletoAgent : ISeguimientoControlBoletoAgent
    {
        private readonly ILogger logger;

        private readonly string userSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string passSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";
        private readonly bool valorPruebaSap = ConfigurationManager.AppSettings["ValorPruebaSap"] == "1";

        public SeguimientoControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string RegistrarSeguimiento(SeguimientoControlDeBoletosDto dto)
        {
            // Modo prueba
            if (valorPruebaSap)
            {
                return "Modificación de contrato satisfactoria";
            }

            try
            {
                var agent = CrearClienteSap();

                var request = new ZMprfcSeguimientoBoleto
                {
                    ImBolsa = dto.Bolsa,
                    ImBolsaSellado = dto.BolsaSellado,
                    ImContrato = dto.Contrato,
                    ImFeEnviadoFirma = dto.FeEnviadoFirma,
                    ImFeEnvio = dto.FeEnvio,
                    ImFeEnvioAfip = dto.FeEnvioAfip,
                    ImFeEnvioBolsa = dto.FeEnvioBolsa,
                    ImFeRecepBoleto = dto.FeRecepBoleto,
                    ImFeRecibFirma = dto.FeRecibFirma,
                    ImFeVueltaAfip = dto.FeVueltaAfip,
                    ImFeVueltaBolsa = dto.FeVueltaBolsa,
                    ImFecAcopio = dto.FecAcopio,
                    ImFecha = dto.Fecha,
                    ImHora = dto.Hora,
                    ImObsCtrlBoleto = dto.ObsCtrlBoleto,
                    ImObsCtrlBoleto2 = dto.ObsCtrlBoleto2,
                    ImRechazadoAfip = dto.RechazadoAfip,
                    ImTipoBoleto = dto.TipoBoleto,
                    ImUsuario = dto.Usuario
                };
                logger.Debug(request.ToXml());

                var response = agent.ZMprfcSeguimientoBoleto(request);

                if (activarLogDebug)
                {
                    logger.Debug(response.ToXml());
                }

                return response.ExMensaje?.Trim();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al registrar seguimiento de control de boleto en SAP.");
                throw;
            }
        }

        private Z_MP_WS_DATAAGRO_DIRECTOClient CrearClienteSap()
        {
            var client = new Z_MP_WS_DATAAGRO_DIRECTOClient();
            client.ClientCredentials.UserName.UserName = userSap;
            client.ClientCredentials.UserName.Password = passSap;
            return client;
        }
    }
}
