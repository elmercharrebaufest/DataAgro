using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ConsultarEstadoBoleto;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConsultarEstadoBoletoAgent : IConsultarEstadoBoletoAgent
    {
        private readonly IRepositorio repositorio;
        public ConsultarEstadoBoletoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public DatosEstadoBoletoDto EstadoBoleto(string ContratoSAP, string FijacionSAP)
        {
            logger.Debug("Enviando ContratoSAP Nro: " + ContratoSAP);
            //Cambiar el Dto a la convencion de DA
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                DatosEstadoBoletoDto estadoBoleto = new DatosEstadoBoletoDto
                {
                    //EX_BOLETO = "",
                    Version = "1",
                    Anulado = "",
                    FechaRecepcionBoleto = DateTime.Now.ToString("yyyy-MM-dd"),
                    Contrato = ContratoSAP,
                    FechaConfirmacion = DateTime.Now.ToString("yyyy-MM-dd"),
                    Generado = ""
                };
                estadoBoleto.CondicionFijacion.Add(new CondicionFijacionEstadoBoletoDto
                {
                    Contrato = ContratoSAP,
                    CantidadMaxima = 10,
                    CantidadMinima = 1,
                    FechaDesde = DateTime.Now.AddMonths(-2).ToString("yyyy-MM-dd"),
                    FechaHasta = DateTime.Now.AddMonths(2).ToString("yyyy-MM-dd"),
                    Meins = "MEINS"
                });
                return estadoBoleto;
            }
            try
            {

                SI_ZMPWS_DATAAGRO_CONSULTAR_ESTADO_BOLETClient agent = new SI_ZMPWS_DATAAGRO_CONSULTAR_ESTADO_BOLETClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;


                logger.Debug("Cargando contrato");
                var rq = new Z_MPRFC_CONSULTAR_ESTADO_BOLET()
                {
                    IM_CONTRATO = ContratoSAP,
                    IM_FIJACION = FijacionSAP
                };
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.SI_ZMPWS_DATAAGRO_CONSULTAR_ESTADO_BOLET(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                DatosEstadoBoletoDto estadoBoleto = new DatosEstadoBoletoDto
                {
                    //EX_BOLETO = "",
                    Version = devolucion.EX_VERSION,
                    Anulado = devolucion.EX_ANULADO,
                    FechaRecepcionBoleto = devolucion.EX_FE_RECEP_BOLETO,
                    Contrato = devolucion.EX_CONTRATO,
                    //EX_COND_FIJACION = devolucion.EX_COND_FIJACION,
                    FechaConfirmacion = devolucion.EX_FECHA_CONFIR,
                    Generado = devolucion.EX_GENERADO
                };
                foreach(var item in devolucion.EX_COND_FIJACION)
                {
                    estadoBoleto.CondicionFijacion.Add(new CondicionFijacionEstadoBoletoDto {
                        Contrato = item.CONTRNUM, 
                        CantidadMaxima = item.CANT_MAX,
                        CantidadMinima = item.CANT_MIN,
                        FechaDesde = item.FE_DESDE,
                        FechaHasta = item.FE_HASTA,
                        Meins = item.MEINS
                    });
                }
                return estadoBoleto;

            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw e;
            }
        }
    }
}
