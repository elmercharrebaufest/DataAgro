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
                    ImBolsa = ValorPorDefecto(dto.Bolsa),
                    ImBolsaSellado = ValorPorDefecto(dto.BolsaSellado),
                    ImContrato = ValorPorDefecto(dto.Contrato),
                    ImFeEnviadoFirma = ValorPorDefecto(dto.FechaEnvioFirma),
                    ImFeEnvioBolsa = ValorPorDefecto(dto.FechaEnvioBolsa),
                    ImFeEnvioAfip = ValorPorDefecto(dto.FechaEnvioAfip),
                    ImFeRecibFirma = ValorPorDefecto(dto.FechaRecepcionFirma),
                    ImFeVueltaBolsa = ValorPorDefecto(dto.FechaRecepcionBolsa),
                    ImFeRecepBoleto = ValorPorDefecto(dto.FechaRecepBoleto),
                    ImFeVueltaAfip = ValorPorDefecto(dto.FechaRecepcionAfip),
                    ImFeEnvio = ValorPorDefecto(dto.FechaEnvioSellado),
                    ImFecAcopio = ValorPorDefecto(dto.FecAcopio),
                    ImFecha = ValorPorDefecto(dto.Fecha),
                    ImHora = ValorPorDefecto(dto.Hora),
                    ImObsCtrlBoleto = ValorPorDefecto(dto.ObsCtrlBoleto),
                    ImObsCtrlBoleto2 = ValorPorDefecto(dto.ObsCtrlBoleto2),
                    ImRechazadoAfip = ValorPorDefecto(dto.RechazadoAfip),
                    ImTipoBoleto = ValorPorDefecto(dto.TipoBoleto),
                    ImUsuario = ValorPorDefecto(dto.Usuario),
                    ImFijacion = string.Empty
                };
                logger.Debug(request.ToXml());

                var response = agent.ZMprfcSeguimientoBoleto(request);
                logger.Debug(response.ToXml());
                if (response == null)
                {
                    logger.Error("La respuesta de SAP es nula al registrar seguimiento de boleto.");
                    throw new Exception("No se recibió respuesta de SAP al registrar seguimiento de boleto.");
                }

                var mensaje = response.ExMensaje?.Trim() ?? string.Empty;

                // Validar si el mensaje indica error
                if (mensaje.ToLower().Contains("error") || mensaje.ToLower().Contains("fallo"))
                {
                    logger.Error($"SAP retornó error: {mensaje}");
                    throw new Exception($"Error en SAP: {mensaje}");
                }

                return mensaje;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al registrar seguimiento de control de boleto en SAP.");
                throw;
            }
        }
        private string ValorPorDefecto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor;
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
