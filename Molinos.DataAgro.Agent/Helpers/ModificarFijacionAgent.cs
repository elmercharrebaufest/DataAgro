using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
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
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    logger.Debug("Modificando Fijacion Nro: " + contratoGuardado.FijacionSAP);
                    logger.Debug("Contrato Obtenido: " + contratoGuardado.Id);
                    logger.Debug("Cargando contrato");
                    var fechaDolarizadoString = contratoGuardado.FechaDolarizado?.ToString("yyyy-MM-dd");
                    var rq = new ZMprfcModificarFijacion
                    {
                        ImContrato = contratoGuardado.ContratoSAP,
                        ImZlsch = contratoGuardado.ChequeElectronico == true ? "=" : "",
                        ImCuentaMrp = contratoGuardado.PagoCBU != null ? contratoGuardado.PagoCBU.Split('-')[0] : "",
                        ImFijacion = contratoGuardado.FijacionSAP,
                        ImDolarizado = contratoGuardado.Dolarizado == true ? "X" : "",
                        ImDolCorredor = contratoGuardado.DolarizadoCorredor == true ? "X" : "",
                        ImDolExpress = contratoGuardado.DolarizadoExpress == true ? "X" : "",
                        ImFechaLimite = fechaDolarizadoString,
                        ImDiasDiferim = contratoGuardado.DiasPesificado != null ? contratoGuardado.DiasPesificado.ToString() : "0"
                    };

                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcModificarFijacion(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    logger.Debug(devolucion != null && !string.IsNullOrEmpty(devolucion.ExMensaje) ? "Respuesta SAP: " + devolucion.ExMensaje : "OK SAP null");
                    return devolucion.ExMensaje;

                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al modificar fijación.");
                    throw;
                }
            }
        }
    }
}
