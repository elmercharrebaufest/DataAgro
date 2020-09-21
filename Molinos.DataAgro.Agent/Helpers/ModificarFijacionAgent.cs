using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ModificarFijacion;
using Molinos.DataAgro.Entities.Dto;
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
    public class ModificarFijacionAgent : IModificarFijacionAgent
    {
        private readonly IRepositorio repositorio;
        public ModificarFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Modificar(FijacionDePrecioContrato contrato, FijacionDePrecioContrato contratoGuardado)
        {
            try
            {
                var agent = new SI_ZMPWS_DATAAGRO_MODIFICAR_FIJACIONClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                logger.Debug("Modificando Fijacion Nro: " + contrato.Id);
                logger.Debug("Contrato Obtenido: " + contratoGuardado.Id);               
                logger.Debug("Cargando contrato");
               
                var rq = new Z_MPRFC_MODIFICAR_FIJACION
                {
                    IM_CONTRATO = contratoGuardado.ContratoSAP,                   
                    IM_ZLSCH = contrato.ChequeElectronico == true ? "=" : "",
                    IM_CUENTA_MRP = contrato.PagoCBU != null ? contrato.PagoCBU.Split('-')[0] : "",
                    IM_FIJACION = contratoGuardado.FijacionSAP
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
