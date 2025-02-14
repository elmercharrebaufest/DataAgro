using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CumplimientoCupos;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
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
            try
            {
                logger.Debug("CumplimientoCuposAgent ");

                List<CumplimientoCupoDto> resultado = new List<CumplimientoCupoDto>();
                Z_MPRFC_CUMPLIMIENTO_CUPOS request = new Z_MPRFC_CUMPLIMIENTO_CUPOS
                {
                    IM_CUPOS = cupos.ToArray(),
                    IM_FECHA = fecha.HasValue ? fecha.Value.ToString("yyyy-MM-dd") : ""
                };
                var agent = new SI_ZMPWS_DATAAGRO_CUMPLIMIENTO_CUPOSClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                logger.Debug(request.ToXml());
                Z_MPRFC_CUMPLIMIENTO_CUPOSResponse devolucion = agent.SI_ZMPWS_DATAAGRO_CUMPLIMIENTO_CUPOS(request);

                foreach (var item in devolucion.EX_SALIDA)
                {
                    resultado.Add(new CumplimientoCupoDto
                    {
                        Cumplimiento = item.CUMPLIMIENTO == "X",
                        Codigo = item.CODIGO
                    });
                }
                //logger.Debug(devolucion.ToXml());

                logger.Debug("Sin Error");
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw e;
            }

        }
    }
}
