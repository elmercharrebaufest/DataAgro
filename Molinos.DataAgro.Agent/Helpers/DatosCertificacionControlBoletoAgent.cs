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
        private readonly bool sapSinPi = ConfigurationManager.AppSettings["SAPsinPI"] == "1";

        public DatosCertificacionControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string RegistrarDatosCertificacion(RegistroDatosCertificacionControlDeBoletosDto dto)
        {
            // Modo prueba SAP
            if (valorPruebaSap)
            {
                return "Modificación de contrato satisfactoria";
            }

            // SAP con PI no implementado
            if (!sapSinPi)
            {
                throw new NotImplementedException(
                    "El servicio SAP con PI no está implementado en esta versión."
                );
            }

            try
            {
                var agent = CrearClienteSap();

                var datosCertificacion = dto.Detalle
                    .Select(d => new Zmpes7080
                    {
                        Bolsa = d.Bolsa,
                        FeCertificacion = d.FeCertificacion,
                        FeVencCerti = d.FeVencCerti,
                        Oblea = d.Oblea,
                        Tipo = d.Tipo,
                        Rechazado = d.Rechazado
                    })
                    .ToArray();

                var request = new ZMprfcDatosCertificacion
                {
                    ImContrato = dto.Contrato,
                    ImFecha = dto.Fecha,
                    ImFijacion = dto.Fijacion,
                    ImHora = dto.Hora,
                    ImUsuario = dto.Usuario,
                    DatosCertificacion = datosCertificacion
                };

                var response = agent.ZMprfcDatosCertificacion(request);

                if (activarLogDebug)
                {
                    logger.Debug(request.ToXml());
                    logger.Debug(response.ToXml());
                }

                return response.ExMensaje?.Trim();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al registrar datos de certificación en SAP.");
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
