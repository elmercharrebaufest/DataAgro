using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.EnviarBoleto;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EnviarBoletoAgent : IEnviarBoletoAgent
    {
        private readonly IRepositorio repositorio;
        public EnviarBoletoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Enviar(Boleto boleto)
        {
            logger.Debug("Eviando Contrato Nro: " + boleto.Id);
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {

                return "Ok";
            }
            try
            {

                SI_ZMPWS_DATAAGRO_ENVIAR_BOLETOS_GENEClient agent = new SI_ZMPWS_DATAAGRO_ENVIAR_BOLETOS_GENEClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;


                logger.Debug("Cargando contrato");
                var rq = new Z_MPRFC_ENVIAR_BOLETOS_GENE()
                {
                    IM_CONTRATO = boleto.Negocio.ContratoSAP,
                    IM_FECHA_GENE = boleto.FechaGeneracion.ToString("yyyy-MM-dd"),
                    IM_FIJACION = (boleto.Negocio is FijacionDePrecioContrato) ? (boleto.Negocio as FijacionDePrecioContrato).FijacionSAP : "",
                    IM_GENERADO = "X",
                    IM_HORA_GENE = boleto.FechaGeneracion.ToString("HH:mm:ss"),
                    IM_VERSION = boleto.Version.ToString()
                };
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.SI_ZMPWS_DATAAGRO_ENVIAR_BOLETOS_GENE(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                if (devolucion.EX_MENSAJE != "Ok")
                {
                    throw new Exception(devolucion.EX_MENSAJE);
                }

                return devolucion.EX_MENSAJE;


            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw e;
            }

        }

        private string RellenarEspaciosSAP(string value, int stringLength)
        {
            if (value != null)
            {
                int cantCeros = stringLength - value.Length;
                for (int i = 0; i < cantCeros; i++)
                {
                    value = " " + value;
                }
            }
            return value;
        }
    }
}
