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
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";

        public ModificacionContratoControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }
        public string ModificarContrato(string Clasificacion, string Contrato, string Cosecha, string Fecha, string Hora, string Procedencia, string Provincia, string Usuario)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                string mensaje = "Modificacion de contrato satisfactorio";
                return mensaje;
            }
            else
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    try
                    {
                        Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var request = new ZMprfcModContCtrBoleto()
                        {
                            ImClasificacion = Clasificacion,
                            ImContrato = Contrato,
                            ImCosecha = Cosecha,
                            ImFecha = Fecha,
                            ImHora = Hora,
                            ImProcedencia = Procedencia,
                            ImProvincia = Provincia,
                            ImUsuario = Usuario
                        };

                        var response = agent.ZMprfcModContCtrBoleto(request);

                        if (activarLogDebug)
                        {
                            logger.Debug(request.ToXml());
                            logger.Debug(response.ToXml());
                        }

                        return response.ExMensaje.Trim();
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message);
                        throw;
                    }
                }
                else
                {
                    throw new NotImplementedException("El servicio SAP con PI no está implementado en esta versión.");
                }
            }
        }
    }
}
