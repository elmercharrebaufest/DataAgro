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
    public class DatoDelComercialAgent : IDatoDelComercialAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatoDelComercialAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosComercialAgentDto ObtenerDatosDeComercial(string Usuario)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                Usuario = ConfigurationManager.AppSettings["SapPruebaUser"];
            }

            try
            {


                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;


                var rq = new ZMprfcDatosComercial() { ImUsuario = Usuario.ToUpper() };
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now.Date,
                    Xml = rq.ToXml()
                };

                repositorio.Agregar(log);
                repositorio.GuardarCambios();
                var valor1 = agent.ZMprfcDatosComercial(rq);
                logger.Debug(valor1.ToXml());
                return new DatosComercialAgentDto { EX_GRUPO_COMPRAS = valor1.ExGrupoCompras, EX_ZONA = valor1.ExZona };

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

    }
}
