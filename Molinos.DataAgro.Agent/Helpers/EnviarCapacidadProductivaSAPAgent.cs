using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.EnviarCapacidadProductivaSAP;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EnviarCapacidadProductivaSAPAgent : IEnviarCapacidadProductivaSAPAgent
    {
        public EnviarCapacidadProductivaSAPAgent(ILogger logger)
        {
            this.logger = logger;
        }

        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string EnviarCapacidadProductivaSAP(EnviarCapacidadProductivaSAPDto capProd)
        {
            logger.Debug("Enviando capacidad productiva a SAP - Cuit: " + capProd.Cuit + ", Campaña: " +capProd.Campania);
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "OK";
            }
            try
            {

                var agent = new SI_ZMPWS_DATAAGRO_ACTU_CAP_PRODUCTIVAClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_ACT_CAPACIDAD_PROD()
                {
                    IM_COMERCIAL = capProd.UsuarioSAP,
                    IM_CUIT = capProd.Cuit,
                    IM_DETALLE = new List<ZMPES6970> { new ZMPES6970 
                        { CUIT = capProd.Cuit, MATNR = capProd.Material, COSECHA = capProd.Campania, 
                          CANTIDAD = capProd.Cantidad, UNIME = capProd.UnidadMedida, PORC = capProd.Porcentaje, FECHA_ACT = DateTime.Now.ToString("yyyy-MM-dd") } 
                    }.ToArray(),
                    IM_INTAD = DateTime.Now.ToString("yyyy-MM-dd"),
                    IM_TLFNS = capProd.Campania
                };
                logger.Debug(rq.ToXml());

                var devolucion = agent.SI_ZMPWS_DATAAGRO_ACTU_CAP_PRODUCTIVA(rq);
                logger.Debug(devolucion.ToXml());
                
                return devolucion.EX_MENSAJE;

            }
            catch (Exception e)
            {
                logger.Error("Error comunicación SAP", e);
                throw e;
            }

        }
    }
}