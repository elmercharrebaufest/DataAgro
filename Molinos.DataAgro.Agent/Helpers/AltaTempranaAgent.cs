using NLog;
using Molinos.DataAgro.Agent.AltaTempranaNosisBolsaRuca;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class AltaTempranaAgent : IAltaTempranaAgent
    {
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";

        public AltaTempranaAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public AltaTempranaNRCODto ObtenerAlta(string cuit, string tipoProv)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new AltaTempranaNRCODto
                {
                    AltaTemprana = "SI",
                    Bolsa = "SI",
                    Carta = "SI",
                    FechaActualizacion = "SI",
                    Nosis = "SI",
                    Ruca = new Ruca
                    {
                        Acopiador = new ValoresRuca
                        {
                            Consignatario = "SI",
                            Directo = "SI",
                            PlanCanje = "NO"
                        },
                        Otros = new ValoresRuca
                        {
                            Consignatario = "SI",
                            Directo = "SI",
                            PlanCanje = "SI"
                        },
                        Corredor = tipoProv == "CORR" ? "SI" : "NO",
                        Fason = "NO"
                    },
                    Consignatario = "SI",
                    PlanCanje = "SI",
                    BoletoFisico = "SI",

                    PeticionBorradoGral = "",
                    PeticionBorradoSociedad = "",
                    BloqueoProveedorGral = "",
                    BloqueoProveedorSociedad = "",
                    RiesgoComercial = "",
                    AuthGralMP = "",
                    AuthSociedadMP = "",
                    BloqueoProveedor = "",
                    FechaActualizacionLegajo = "",
                    HistoricoFechaActualizacionLegajo = new List<HistoricoFechaActualizacionLegajo>() {
                        new HistoricoFechaActualizacionLegajo() {
                            Cosecha = "",
                            Material = "",
                            FechaAtualizacion = "" }
                    }
                };
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

                        var rq = new ZMprfcAltaTempranaNRCo()
                        {
                            ImCuit = cuit,
                            ImKtokk = tipoProv,
                        };

                        var valor = agent.ZMprfcAltaTempranaNRCo(rq);
                        if (activarLogDebug)
                        {
                            logger.Debug(rq.ToXml());
                            logger.Debug(valor.ToXml());
                        }

                        var retorno = new AltaTempranaNRCODto
                        {
                            Ruca = new Ruca
                            {
                                Acopiador = new ValoresRuca
                                {
                                    Consignatario = valor.ExRuca.Acopiador.Consignatario,
                                    Directo = valor.ExRuca.Acopiador.Directo,
                                    PlanCanje = valor.ExRuca.Acopiador.ProvPlanCanje
                                },
                                Otros = new ValoresRuca
                                {
                                    Consignatario = valor.ExRuca.Otros.Consignatario,
                                    Directo = valor.ExRuca.Otros.Directo,
                                    PlanCanje = valor.ExRuca.Otros.ProvPlanCanje
                                },
                                Corredor = valor.ExRuca.Corredor,
                                Fason = valor.ExRuca.Fason
                            },
                            PlanCanje = valor.ExPlanCanje,
                            Consignatario = valor.ExConsignatario,
                            Nosis = valor.ExNosis,
                            AltaTemprana = valor.ExAltaTemprana,
                            Bolsa = valor.ExBolsa,
                            Carta = valor.ExCarta,
                            FechaActualizacion = valor.ExFechaActualizacion,
                            Mensaje = valor.ExMensaje,
                            ProveedorGrano = valor.ExProveedorGranos,
                            BoletoFisico = valor.ExBoletoFisico,

                            PeticionBorradoGral = valor.ExPetBorradoGeneral,
                            PeticionBorradoSociedad = valor.ExPetBorradoSociedad,
                            BloqueoProveedorGral = valor.ExBloqueoGeneral,
                            BloqueoProveedorSociedad = valor.ExBloqueoSociedad,
                            RiesgoComercial = valor.ExRiesgoComercial,
                            AuthGralMP = valor.ExBegruGeneral,
                            AuthSociedadMP = valor.ExBegruSociedad,
                            BloqueoProveedor = valor.ExBloqueo,
                            FechaActualizacionLegajo = valor.ExIntad,
                            HistoricoFechaActualizacionLegajo = valor.ExFechaActualizacionLegajo == null ? new List<HistoricoFechaActualizacionLegajo>() :
                                valor.ExFechaActualizacionLegajo.ToList().Select(x => new HistoricoFechaActualizacionLegajo()
                                {
                                    Material = x.Matnr,
                                    Cosecha = x.Cosecha,
                                    FechaAtualizacion = x.FechaAct,
                                }).ToList(),
                        };
                        return retorno;
                    }
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_ALTA_TEMPRANA_N_R_COClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;

                        var rq = new Z_MPRFC_ALTA_TEMPRANA_N_R_CO()
                        {
                            IM_CUIT = cuit,
                            IM_KTOKK = tipoProv,
                        };

                        var valor = agent.SI_ZMPWS_DATAAGRO_ALTA_TEMPRANA_N_R_CO(rq);
                        if (activarLogDebug)
                        {
                            logger.Debug(rq.ToXml());
                            logger.Debug(valor.ToXml());
                        }

                        var retorno = new AltaTempranaNRCODto
                        {
                            Ruca = new Ruca
                            {
                                Acopiador = new ValoresRuca
                                {
                                    Consignatario = valor.EX_RUCA.ACOPIADOR.CONSIGNATARIO,
                                    Directo = valor.EX_RUCA.ACOPIADOR.DIRECTO,
                                    PlanCanje = valor.EX_RUCA.ACOPIADOR.PROV_PLAN_CANJE
                                },
                                Otros = new ValoresRuca
                                {
                                    Consignatario = valor.EX_RUCA.OTROS.CONSIGNATARIO,
                                    Directo = valor.EX_RUCA.OTROS.DIRECTO,
                                    PlanCanje = valor.EX_RUCA.OTROS.PROV_PLAN_CANJE
                                },
                                Corredor = valor.EX_RUCA.CORREDOR,
                                Fason = valor.EX_RUCA.FASON
                            },
                            PlanCanje = valor.EX_PLAN_CANJE,
                            Consignatario = valor.EX_CONSIGNATARIO,
                            Nosis = valor.EX_NOSIS,
                            AltaTemprana = valor.EX_ALTA_TEMPRANA,
                            Bolsa = valor.EX_BOLSA,
                            Carta = valor.EX_CARTA,
                            FechaActualizacion = valor.EX_FECHA_ACTUALIZACION,
                            Mensaje = valor.EX_MENSAJE,
                            ProveedorGrano = valor.EX_PROVEEDOR_GRANOS,
                            BoletoFisico = valor.EX_BOLETO_FISICO,

                            PeticionBorradoGral = valor.EX_PET_BORRADO_GENERAL,
                            PeticionBorradoSociedad = valor.EX_PET_BORRADO_SOCIEDAD,
                            BloqueoProveedorGral = valor.EX_BLOQUEO_GENERAL,
                            BloqueoProveedorSociedad = valor.EX_BLOQUEO_SOCIEDAD,
                            RiesgoComercial = valor.EX_RIESGO_COMERCIAL,
                            AuthGralMP = valor.EX_BEGRU_GENERAL,
                            AuthSociedadMP = valor.EX_BEGRU_SOCIEDAD,
                            BloqueoProveedor = valor.EX_BLOQUEO,
                            FechaActualizacionLegajo = valor.EX_INTAD,
                            HistoricoFechaActualizacionLegajo = valor.EX_FECHA_ACTUALIZACION_LEGAJO == null ? new List<HistoricoFechaActualizacionLegajo>() :
                                valor.EX_FECHA_ACTUALIZACION_LEGAJO.ToList().Select(x => new HistoricoFechaActualizacionLegajo()
                                {
                                    Material = x.MATNR,
                                    Cosecha = x.COSECHA,
                                    FechaAtualizacion = x.FECHA_ACT,
                                }).ToList(),
                        };
                        return retorno;
                    }
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