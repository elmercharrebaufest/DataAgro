using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces.Agent;
using NLog;
using System;
using System.Configuration;
using System.Runtime.InteropServices.WindowsRuntime;


namespace Molinos.DataAgro.Agent.Helpers
{
    public class ModificacionContratoControlBoletoAgent : IModificacionContratoControlBoletoAgent
    {
        private readonly ILogger logger;

        private readonly string userSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string passSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";
        private readonly bool valorPruebaSap = ConfigurationManager.AppSettings["ValorPruebaSap"] == "1";
        private readonly bool sapSinPi = ConfigurationManager.AppSettings["SAPsinPI"] == "1";

        public ModificacionContratoControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string ModificarContrato(
            string clasificacion,
            string contrato,
            string cosecha,
            string fecha,
            string hora,
            string procedencia,
            string provincia,
            string usuario)
        {
            // Modo prueba
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

                var request = new ZMprfcModContCtrBoleto
                {
                    ImClasificacion = clasificacion,
                    ImContrato = contrato,
                    ImCosecha = cosecha,
                    ImFecha = fecha,
                    ImHora = hora,
                    ImProcedencia = procedencia,
                    ImProvincia = provincia,
                    ImUsuario = usuario
                };

                var response = agent.ZMprfcModContCtrBoleto(request);

                if (activarLogDebug)
                {
                    logger.Debug(request.ToXml());
                    logger.Debug(response.ToXml());
                }

                return response.ExMensaje?.Trim();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error al modificar contrato de control de boleto en SAP.");
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
