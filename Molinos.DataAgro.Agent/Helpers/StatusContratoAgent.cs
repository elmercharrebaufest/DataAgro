using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.StatusContrato;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class StatusContratoAgent : IStatusContratoAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";

        public StatusContratoAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public EstadoSAPDto ValidarEstado(string contratoSap)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new EstadoSAPDto { Status = "", NumeroSio = 0 };
            }
            else
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new ZMprfcStatusDeContrato()
                        {
                            ImContrato = new List<string> { contratoSap }.ToArray()
                        };

                        var valor = agent.ZMprfcStatusDeContrato(rq);
                        if (activarLogDebug)
                        {
                            logger.Debug(rq.ToXml());
                            logger.Debug(valor.ToXml());
                        }
                        if (valor.ExSalida == null || valor.ExSalida.Length == 0)
                        {
                            return new EstadoSAPDto();
                        }

                        //parche hasta que se haga para varios
                        var valor2 = valor.ExSalida[0];

                        long.TryParse(valor2.NumSio, out long numsio);
                        logger.Debug($"CONTRATO: {valor2.Contrato}. EX_STATUS: {valor2.Status}. NUM_SIO: {numsio}. MENSAJE: {valor2.Mensaje}. FECHA_CONFIRMADO_SAP: {valor2.FechaConfir}");
                        var estado = new EstadoSAPDto()
                        {
                            ContratoSap = valor2.Contrato,
                            NumeroSio = numsio,
                            Status = valor2.Status,
                            Mensaje = valor2.Mensaje,
                            FechaConfirmadoSAP = string.IsNullOrEmpty(valor2.FechaConfir) || valor2.FechaConfir == "0000-00-00" ? null : DateTime.ParseExact(valor2.FechaConfir, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) as DateTime?,
                        };
                        return estado;
                    }
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_STATUS_DE_CONTRATO()
                        {
                            IM_CONTRATO = new List<string> { contratoSap }.ToArray()
                        };

                        var valor = agent.SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATO(rq);
                        if (activarLogDebug)
                        {
                            logger.Debug(rq.ToXml());
                            logger.Debug(valor.ToXml());
                        }
                        if (valor.EX_SALIDA == null || valor.EX_SALIDA.Length == 0)
                        {
                            return new EstadoSAPDto();
                        }

                        //parche hasta que se haga para varios
                        var valor2 = valor.EX_SALIDA[0];

                        long.TryParse(valor2.NUM_SIO, out long numsio);
                        logger.Debug($"CONTRATO: {valor2.CONTRATO}. EX_STATUS: {valor2.STATUS}. NUM_SIO: {numsio}. MENSAJE: {valor2.MENSAJE}. FECHA_CONFIRMADO_SAP: {valor2.FECHA_CONFIR}");
                        var estado = new EstadoSAPDto()
                        {
                            ContratoSap = valor2.CONTRATO,
                            NumeroSio = numsio,
                            Status = valor2.STATUS,
                            Mensaje = valor2.MENSAJE,
                            FechaConfirmadoSAP = string.IsNullOrEmpty(valor2.FECHA_CONFIR) || valor2.FECHA_CONFIR == "0000-00-00" ? null : DateTime.ParseExact(valor2.FECHA_CONFIR, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) as DateTime?,

                        };
                        return estado;
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

        public List<EstadoSAPDto> ValidarEstados(List<string> contratosSap)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<EstadoSAPDto> { new EstadoSAPDto { Status = "", NumeroSio = 0, ContratoSap = "" } };
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_STATUS_DE_CONTRATO()
                    {
                        IM_CONTRATO = contratosSap.ToArray()
                    };

                    var valor = agent.SI_ZMPWS_DATAAGRO_STATUS_DE_CONTRATO(rq);

                    if (activarLogDebug)
                    {
                        logger.Debug(rq.ToXml());
                        logger.Debug(valor.ToXml());
                    }

                    var estados = new List<EstadoSAPDto>();

                    if (valor.EX_SALIDA != null)
                    {
                        foreach (var item in valor.EX_SALIDA)
                        {
                            long.TryParse(item.NUM_SIO, out long numsio);
                            logger.Debug($"Contrato: {item.CONTRATO}. EX_STATUS: {item.STATUS}. NUM_SIO: {numsio}. MENSAJE: {item.MENSAJE}. FECHA_CONFIRMADO_SAP: {item.FECHA_CONFIR}");
                            var estado = new EstadoSAPDto()
                            {
                                NumeroSio = numsio,
                                Status = item.STATUS,
                                ContratoSap = item.CONTRATO,
                                Mensaje = item.MENSAJE,
                                FechaConfirmadoSAP = string.IsNullOrEmpty(item.FECHA_CONFIR) || item.FECHA_CONFIR == "0000-00-00" ? null : DateTime.ParseExact(item.FECHA_CONFIR, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) as DateTime?,
                            };
                            estados.Add(estado);
                        }
                    }

                    return estados;

                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }
    }
}