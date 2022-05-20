using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ModificarCupos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ModificarCupoAgent : IModificarCupoAgent
    {
        public ModificarCupoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
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
                    var agent = new SI_ZMPWS_DATAAGRO_MODIFICAR_CUPOClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var corredor = repositorio.Existe<CorredorProveedor>(x => x.CorredorId == cupo.ProveedorId) ? "C" : "00";
                    var rq = new Z_MPRFC_MODIFICAR_CUPO
                    {
                        IM_COMERCIAL = cupo.Comercial.IdActiveDirectory,
                        IM_CALIDAD = cupo.Calidad == "Camara" ? "01" : cupo.Calidad == "Fabrica" ? "03" : "",
                        IM_DESTINATARIO = cupo.Destinatario,
                        IM_OBSERVACIONES = cupo.Observaciones,
                        IM_PROVEEDOR= corredor + cupo.Proveedor.CUIT.Remove(cupo.Proveedor.CUIT.Length - 1).Remove(0, 2),
                        IM_DESCPROV = cupo.Proveedor.RazonSocial.Length > 35 ? cupo.Proveedor.RazonSocial.Substring(0, 35) : cupo.Proveedor.RazonSocial,
                        IM_CODIGO = cupo.CupoSap,
                        IM_FLETE_PROC= cupo.FleteProcedencia == true? "S" : "N",
                        IM_EXCEPCION = cupo.Proveedor.CuposConRiesgo == true ? "X" : ""
                    };

                    var logId = repositorio.Agregar(new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    });
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_MODIFICAR_CUPO(rq);

                    logger.Debug(devolucion.ToXml());
                    var log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    logger.Debug("Guardado en la base");

                    if (devolucion.EX_MENSAJE != "Ok")
                    {
                        throw new Exception(devolucion.EX_MENSAJE);
                    }
                    logger.Debug("Sin Error");

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
