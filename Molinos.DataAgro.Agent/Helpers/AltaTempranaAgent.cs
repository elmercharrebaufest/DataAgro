using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using NLog;
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
                        SinOblea = valor.ExSinOblea,
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
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
