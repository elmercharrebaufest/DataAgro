using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ModificarFijacion;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ModificarFijacionAgent : IModificarFijacionAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ModificarFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string Modificar(FijacionDePrecioContrato contratoGuardado)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "Se actualizaron los datos correctamente";
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_MODIFICAR_FIJACIONClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    logger.Debug("Modificando Fijacion Nro: " + contratoGuardado.FijacionSAP);
                    logger.Debug("Contrato Obtenido: " + contratoGuardado.Id);
                    logger.Debug("Cargando contrato");
                    var fechaDolarizadoString = contratoGuardado.FechaDolarizado?.ToString("yyyy-MM-dd");
                    var rq = new Z_MPRFC_MODIFICAR_FIJACION
                    {
                        IM_CONTRATO = contratoGuardado.ContratoSAP,
                        IM_ZLSCH = contratoGuardado.ChequeElectronico == true ? "=" : "",
                        IM_CUENTA_MRP = contratoGuardado.PagoCBU != null ? contratoGuardado.PagoCBU.Split('-')[0] : "",
                        IM_FIJACION = contratoGuardado.FijacionSAP,
                        IM_DOLARIZADO = contratoGuardado.Dolarizado == true ? "X" : "",
                        IM_DOL_CORREDOR = contratoGuardado.DolarizadoCorredor == true ? "X" : "",
                        IM_DOL_EXPRESS = contratoGuardado.DolarizadoExpress == true ? "X" : "",
                        IM_FECHA_LIMITE = fechaDolarizadoString,
                        IM_DIAS_DIFERIM = contratoGuardado.DiasPesificado != null ? contratoGuardado.DiasPesificado.ToString() : "0"
                    };

                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_MODIFICAR_FIJACION(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    logger.Debug(devolucion != null && !string.IsNullOrEmpty(devolucion.EX_MENSAJE) ? "Respuesta SAP: " + devolucion.EX_MENSAJE : "OK SAP null");
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
