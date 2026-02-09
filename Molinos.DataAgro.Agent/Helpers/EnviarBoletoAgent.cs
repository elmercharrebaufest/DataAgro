using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class EnviarBoletoAgent : IEnviarBoletoAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public EnviarBoletoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string EnviarBoleto(BoletoDto boleto)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "Se actualizan correctamente los datos";
            }
            try
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var boletoCompraNet = repositorio.Listar<BoletoCompraNet>();
                var rq = new ZMprfcEnviarBoletosGene()
                {
                    ImContrato = boleto.ContratoSAP ?? string.Empty,
                    ImFechaGene = boleto.FechaGeneracion.ToString("yyyy-MM-dd"),
                    ImFijacion = !string.IsNullOrEmpty(boleto.FijacionSAP) ? boleto.FijacionSAP : "",
                    ImGenerado = "X",
                    ImHoraGene = boleto.FechaGeneracion.ToString("HH:mm:ss"),
                    ImVersion = boleto.Version.ToString(),
                    ImEstado = "",
                    ImEstadoDocumento = "",
                    ImEstadoLote = "",
                    ImIdDocConfirma = "",
                    ImIdLoteConfirma = "",
                    ImTipoBoleto = boletoCompraNet.Find(x => x.Id == boleto.TipoBoletoId)?.Descripcion
                };
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.ZMprfcEnviarBoletosGene(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                return devolucion.ExMensaje;
            }
            catch (Exception e)
            {
                logger.Error(e, "Error comunicacion SAP en EnviarBoleto ");
                throw;
            }

        }
    }
}
