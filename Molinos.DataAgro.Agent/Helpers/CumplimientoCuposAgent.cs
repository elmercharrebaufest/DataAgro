using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class CumplimientoCuposAgent : ICumplimientoCuposAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public CumplimientoCuposAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public List<CumplimientoCupoDto> Ejecutar(List<string> cupos, DateTime? fecha)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                logger.Debug("CumplimientoCuposAgent - ValorPruebaSap = 1, se devuelve valor de prueba.");
                return new List<CumplimientoCupoDto>
                {
                    new CumplimientoCupoDto { Codigo = "Cupo1", Cumplimiento = true },
                    new CumplimientoCupoDto { Codigo = "Cupo2", Cumplimiento = false },
                    new CumplimientoCupoDto { Codigo = "Cupo3", Cumplimiento = true }
                };
            }
            else
            {
                try
                {
                    logger.Debug("CumplimientoCuposAgent ");
                    List<CumplimientoCupoDto> resultado = new List<CumplimientoCupoDto>();

                    ZMprfcCumplimientoCupos request = new ZMprfcCumplimientoCupos
                    {
                        ImCupos = cupos.ToArray(),
                        ImFecha = fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd") : ""
                    };

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    logger.Debug(request.ToXml());
                    ZMprfcCumplimientoCuposResponse devolucion = agent.ZMprfcCumplimientoCupos(request);
                    foreach (var item in devolucion.ExSalida)
                    {
                        resultado.Add(new CumplimientoCupoDto
                        {
                            Cumplimiento = item.Cumplimiento == "X",
                            Codigo = item.Codigo
                        });
                    }
                    logger.Debug("Sin Error");
                    return resultado;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al consultar cumplimiento de cupos.");
                    throw;
                }
            }
        }
    }
}
