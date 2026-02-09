using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
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
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcStatusDeContrato()
                    {
                        ImContrato = contratosSap.ToArray()
                    };

                    var valor = agent.ZMprfcStatusDeContrato(rq);

                    if (activarLogDebug)
                    {
                        logger.Debug(rq.ToXml());
                        logger.Debug(valor.ToXml());
                    }

                    var estados = new List<EstadoSAPDto>();

                    if (valor.ExSalida != null)
                    {
                        foreach (var item in valor.ExSalida)
                        {
                            long.TryParse(item.NumSio, out long numsio);
                            logger.Debug($"Contrato: {item.Contrato}. EX_STATUS: {item.Status}. NUM_SIO: {numsio}. MENSAJE: {item.Mensaje}. FECHA_CONFIRMADO_SAP: {item.FechaConfir}");
                            var estado = new EstadoSAPDto()
                            {
                                NumeroSio = numsio,
                                Status = item.Status,
                                ContratoSap = item.Contrato,
                                Mensaje = item.Mensaje,
                                FechaConfirmadoSAP = string.IsNullOrEmpty(item.FechaConfir) || item.FechaConfir == "0000-00-00" ? null : DateTime.ParseExact(item.FechaConfir, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture) as DateTime?,
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