using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
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
    public class DatosCertificacionControlBoletoAgent : IDatosCertificacionControlBoletoAgent
    {
        private readonly ILogger logger;

        private readonly string userSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string passSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";
        private readonly bool valorPruebaSap = ConfigurationManager.AppSettings["ValorPruebaSap"] == "1";

        public DatosCertificacionControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string RegistrarDatosCertificacion(RegistroDatosCertificacionControlDeBoletosDto dto)
        {
            string mensaje = string.Empty;
            // Modo prueba SAP
            if (valorPruebaSap)
            {
                return "Modificación de contrato satisfactoria";
            }


            try
            {
                var agent = CrearClienteSap();

                var datosCertificacion = dto.Detalle
                    .Select(d => new Zmpes7080
                    {
                        Bolsa = ValorPorDefecto(d.Bolsa),
                        FeCertificacion = ValorPorDefecto(d.FeCertificacion),
                        FeVencCerti = ValorPorDefecto(d.FeVencCerti),
                        Oblea = ValorPorDefecto(d.Oblea),
                        Tipo = ValorPorDefecto(d.Tipo),
                        Rechazado = ValorPorDefecto(d.Rechazado)
                    })
                    .ToArray();

                var request = new ZMprfcDatosCertificacion
                {
                    ImContrato = ValorPorDefecto(dto.Contrato),
                    ImFecha = ValorPorDefecto(dto.Fecha),
                    ImFijacion = ValorPorDefecto(dto.Fijacion),
                    ImHora = ValorPorDefecto(dto.Hora),
                    ImUsuario = ValorPorDefecto(dto.Usuario),
                    DatosCertificacion = datosCertificacion
                };
                logger.Debug(request.ToXml());
                var response = agent.ZMprfcDatosCertificacion(request);
                logger.Debug(response.ToXml());
                mensaje = response.ExMensaje?.Trim();
                return response.ExMensaje?.Trim();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al registrar datos de certificación en SAP");
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
        private string ValorPorDefecto(string valor)
        {
            return string.IsNullOrWhiteSpace(valor) ? string.Empty : valor;
        }
    }

}
