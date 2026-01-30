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
    public class ModificarCupoAgent : IModificarCupoAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ModificarCupoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string Modificar(Cupo cupo)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "Ok";
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var corredor = repositorio.Existe<CorredorProveedor>(x => x.CorredorId == cupo.ProveedorId) ? "C" : "00";
                    var rq = new ZMprfcModificarCupo
                    {
                        ImComercial = cupo.Comercial.IdActiveDirectory,
                        ImCalidad = cupo.Calidad == "Camara" ? "01" : cupo.Calidad == "Fabrica" ? "03" : "",
                        ImDestinatario = cupo.Destinatario,
                        ImObservaciones = cupo.Observaciones,
                        ImProveedor = corredor + cupo.Proveedor.CUIT.Remove(cupo.Proveedor.CUIT.Length - 1).Remove(0, 2),
                        ImDescprov = cupo.Proveedor.RazonSocial.Length > 35 ? cupo.Proveedor.RazonSocial.Substring(0, 35) : cupo.Proveedor.RazonSocial,
                        ImCodigo = cupo.CupoSap,
                        ImFleteProc = cupo.FleteProcedencia == true ? "S" : "N",
                        ImExcepcion = cupo.Proveedor.CuposConRiesgo == true ? "X" : ""
                    };

                    var logId = repositorio.Agregar(new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    });
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var devolucion = agent.ZMprfcModificarCupo(rq);
                    logger.Debug(devolucion.ToXml());
                    var log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    logger.Debug("Guardado en la base");

                    if (devolucion.ExMensaje != "Ok")
                    {
                        throw new Exception(devolucion.ExMensaje);
                    }
                    logger.Debug("Sin Error");

                    return devolucion.ExMensaje;

                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al modificar cupo.");
                    throw;
                }
            }
        }
    }
}
