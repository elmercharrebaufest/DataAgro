using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
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
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";

        public SeguimientoControlBoletoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public string SeguimientoBoletos(Entities.Dto.ControlDeBoletos.SeguimientoControlDeBoletosDto seguimientoControlDeBoletos)
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

                        var request = new ZMprfcSeguimientoBoleto()
                        {
                            ImBolsa = seguimientoControlDeBoletos.Bolsa,
                            ImBolsaSellado = seguimientoControlDeBoletos.BolsaSellado,
                            ImContrato = seguimientoControlDeBoletos.Contrato,
                            ImFeEnviadoFirma = seguimientoControlDeBoletos.FeEnviadoFirma,
                            ImFeEnvio = seguimientoControlDeBoletos.FeEnvio,
                            ImFeEnvioAfip = seguimientoControlDeBoletos.FeEnvioAfip,
                            ImFeEnvioBolsa = seguimientoControlDeBoletos.FeEnvioBolsa,
                            ImFeRecepBoleto = seguimientoControlDeBoletos.FeRecepBoleto,
                            ImFeRecibFirma = seguimientoControlDeBoletos.FeRecibFirma,
                            ImFeVueltaAfip = seguimientoControlDeBoletos.FeVueltaAfip,
                            ImFeVueltaBolsa = seguimientoControlDeBoletos.FeVueltaBolsa,
                            ImFecAcopio = seguimientoControlDeBoletos.FecAcopio,
                            ImFecha = seguimientoControlDeBoletos.Fecha,
                            ImFijacion = seguimientoControlDeBoletos.Fijacion,
                            ImHora = seguimientoControlDeBoletos.Hora,
                            ImObsCtrlBoleto = seguimientoControlDeBoletos.ObsCtrlBoleto,
                            ImObsCtrlBoleto2 = seguimientoControlDeBoletos.ObsCtrlBoleto2,
                            ImRechazadoAfip = seguimientoControlDeBoletos.RechazadoAfip,
                            ImTipoBoleto = seguimientoControlDeBoletos.TipoBoleto,
                            ImUsuario = seguimientoControlDeBoletos.Usuario
                        };

                        var response = agent.ZMprfcSeguimientoBoleto(request);
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
