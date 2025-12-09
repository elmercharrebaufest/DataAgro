using NLog;
using Molinos.DataAgro.Agent.ConsultarEstadoBoleto;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ConsultarEstadoBoletoAgent : IConsultarEstadoBoletoAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ConsultarEstadoBoletoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosEstadoBoletoDto EstadoBoleto(string ContratoSAP, string FijacionSAP)
        {
            logger.Debug("Consultando EstadoBoleto del ContratoSAP Nro " + ContratoSAP);
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
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcConsultarEstadoBolet()
                    {
                        ImContrato = ContratoSAP,
                        ImFijacion = FijacionSAP
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcConsultarEstadoBolet(rq);
                    logger.Debug(devolucion.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    DatosEstadoBoletoDto estadoBoleto = new DatosEstadoBoletoDto
                    {
                        Version = devolucion.ExVersion,
                        Anulado = devolucion.ExAnulado,
                        FechaRecepcionBoleto = devolucion.ExFeRecepBoleto,
                        Contrato = devolucion.ExContrato,
                        FechaConfirmacion = devolucion.ExFechaConfir,
                        Generado = devolucion.ExGenerado
                    };
                    foreach (var item in devolucion.ExCondFijacion)
                    {
                        estadoBoleto.CondicionFijacion.Add(new CondicionFijacionEstadoBoletoDto
                        {
                            Contrato = item.Contrnum,
                            CantidadMaxima = item.CantMax,
                            CantidadMinima = item.CantMin,
                            FechaDesde = item.FeDesde,
                            FechaHasta = item.FeHasta,
                            Meins = item.Meins
                        });
                    }
                    return estadoBoleto;
                }
                else
                {
                    SI_ZMPWS_DATAAGRO_CONSULTAR_ESTADO_BOLETClient agent = new SI_ZMPWS_DATAAGRO_CONSULTAR_ESTADO_BOLETClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

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
                    foreach (var item in devolucion.EX_COND_FIJACION)
                    {
                        estadoBoleto.CondicionFijacion.Add(new CondicionFijacionEstadoBoletoDto
                        {
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
            }
            catch (Exception e)
            {
                logger.Error(e, "Error comunicacion SAP en EstadoBoletoAgent: ");
                throw;
            }
        }
    }
}
