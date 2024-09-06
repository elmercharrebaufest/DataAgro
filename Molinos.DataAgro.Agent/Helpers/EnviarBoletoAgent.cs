using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.EnviarBoleto;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

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
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string EnviarBoleto(BoletoDto boleto)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "Se actualizan correctamente los datos";
            }
            try
            {
                SI_ZMPWS_DATAAGRO_ENVIAR_BOLETOS_GENEClient agent = new SI_ZMPWS_DATAAGRO_ENVIAR_BOLETOS_GENEClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var boletoCompraNet = repositorio.Listar<BoletoCompraNet>();
                var rq = new Z_MPRFC_ENVIAR_BOLETOS_GENE()
                {
                    IM_CONTRATO = boleto.ContratoSAP ?? string.Empty,
                    IM_FECHA_GENE = boleto.FechaGeneracion.ToString("yyyy-MM-dd"),
                    IM_FIJACION = !string.IsNullOrEmpty(boleto.FijacionSAP) ? boleto.FijacionSAP : "",
                    IM_GENERADO = "X",
                    IM_HORA_GENE = boleto.FechaGeneracion.ToString("HH:mm:ss"),
                    IM_VERSION = boleto.Version.ToString(),
                    IM_ESTADO = "",
                    IM_ESTADO_DOCUMENTO = "",
                    IM_ESTADO_LOTE = "",
                    IM_ID_DOC_CONFIRMA = "",
                    IM_ID_LOTE_CONFIRMA = "",
                    IM_TIPO_BOLETO = boletoCompraNet.Find(x => x.Id == boleto.TipoBoletoId)?.Descripcion
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

                return devolucion.EX_MENSAJE;

            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP en EnviarBoleto ", e);
                throw;
            }

        }
    }
}
