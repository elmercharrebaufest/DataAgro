using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.AltaTempranaNosisBolsaRuca;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class AltaTempranaAgent : IAltaTempranaAgent
    {
        public AltaTempranaAgent(ILogger logger)
        {
            this.logger = logger;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public AltaTempranaNRCODto ObtenerAlta(string cuit)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new AltaTempranaNRCODto { AltaTemprana = "SI", Bolsa = "SI", Carta = "SI", FechaActualizacion = "SI", Nosis = "SI", Ruca = new Ruca { Acopiador = "SI", Otros = "SI" } };
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

                    var valor = agent.SI_ZMPWS_DATAAGRO_ALTA_TEMPRANA_N_R_CO(rq);
                    var retorno = new AltaTempranaNRCODto
                    {
                        Ruca = new Ruca { Acopiador = valor.EX_RUCA.ACOPIADOR, Otros = valor.EX_RUCA.OTROS },
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
