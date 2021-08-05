using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AnularFijacionVirtual;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class AnularFijacionVirtualAgent : IAnularFijacionVirtualAgent
    {
        public AnularFijacionVirtualAgent(IRepositorio repositorio, ILogger logger)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;

        public string AnularFijacionVirtual(FijacionDePrecioContrato fijacion, string comercial)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                var resp = "OK";                
                return resp;
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_ANULAR_FIJ_VIR_CANJEClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var rq = new SI_ZMPWS_DATAAGRO_ANULAR_FIJ_VIR_CANJERequest()
                    {
                       Z_MPRFC_ANULAR_FIJ_VIR_CANJE = new Z_MPRFC_ANULAR_FIJ_VIR_CANJE
                       {
                          IM_CONTRNUM = fijacion.ContratoSAP.Substring(3),
                          IM_NRO_FIJ = fijacion.FijacionSAP.Substring(10),
                          IM_UNAME = comercial.ToUpper()
                       }
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_ANULAR_FIJ_VIR_CANJE(rq.Z_MPRFC_ANULAR_FIJ_VIR_CANJE);
                    logger.Debug(devolucion.ToXml());

                    return devolucion.EX_MENSAJE;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
        }
    }
}
