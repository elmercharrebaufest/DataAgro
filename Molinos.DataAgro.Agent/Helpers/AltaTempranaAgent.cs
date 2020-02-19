using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AltaTempranaNosisBolsaRuca;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class AltaTempranaAgent : IAltaTempranaAgent
    {
        public AltaTempranaAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public AltaTempranaNRCODto ObtenerAlta(string cuit)
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
                        Corredor = "NO"
                    },
                    Consignatario = "SI",
                    PlanCanje = "SI",
                    
                };
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_ALTA_TEMPRANA_N_R_COClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_ALTA_TEMPRANA_N_R_CO()
                    {
                        IM_CUIT = cuit
                    };
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_ALTA_TEMPRANA_N_R_CO(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

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
                            Corredor = valor.EX_RUCA.CORREDOR
                        },
                        PlanCanje = valor.EX_PLAN_CANJE,
                        Consignatario = valor.EX_CONSIGNATARIO,
                        Nosis = valor.EX_NOSIS,
                        AltaTemprana = valor.EX_ALTA_TEMPRANA,
                        Bolsa = valor.EX_BOLSA,
                        Carta = valor.EX_CARTA,
                        FechaActualizacion = valor.EX_FECHA_ACTUALIZACION,
                        Mensaje = valor.EX_MENSAJE
                    };
                    return retorno;
                }catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
