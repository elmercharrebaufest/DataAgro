using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarContrato;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarFijacionAgent
    {
        public FinalizarFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio; 
        public string Finalizar(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return repositorio.Listar<FijacionDePrecioContrato>().Select(x=>x.FijacionSAP).Last()+1.ToString();
            }
            else
            {
                try
                {
                    SI_ZMPWS_DATAAGRO_PRE_SLIPClient agent = new SI_ZMPWS_DATAAGRO_PRE_SLIPClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaDescuentos = new List<ZMPES5290>();

                    var rq = new Z_MPRFC_PRE_SLIP()
                    {
                        IM_CONTRATO = new ZMPES5270
                        {
                        },
                        IM_TOPES_FIJ = new ZMPES5280
                        {
                        },
                    };

                    logger.Debug(rq.ToXml());
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_PRE_SLIP(rq);
                    logger.Debug(devolucion.ToXml());
                    if (devolucion.EX_MENSAJE_ERROR != null && devolucion.EX_MENSAJE_ERROR != "")
                    {
                        throw new Exception(devolucion.EX_MENSAJE_ERROR);
                    }

                    return devolucion.EX_CONTRATO_SAP;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
        }

        private string RellenarEspaciosSAP(string value, int stringLength)
        {
            if (value != null) {
                int cantCeros = stringLength - value.Length;
                for (int i = 0; i < cantCeros; i++) {
                    value = " " + value;
                }                
            }
            return value;
        }

    }
}
