using System;
using System.Configuration;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelComercial;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

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

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    logger.Info("SAP sin PI - RFC ZMprfcDatosComercial");
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
                    logger.Info("SAP sin PI - RFC ZMprfcDatosComercial");
                    logger.Debug(valor1.ToXml());
                    return new DatosComercialAgentDto { EX_GRUPO_COMPRAS = valor1.ExGrupoCompras, EX_ZONA = valor1.ExZona};
                }
                else
                {
                    SI_ZMPWS_DATAAGRO_DATOS_COMERCIALESClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMERCIALESClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_DATOS_COMERCIAL() { IM_USUARIO = Usuario.ToUpper() };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now.Date,
                        Xml = rq.ToXml()
                    };

                    repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_COMERCIALES(rq);
                    logger.Debug(valor1.ToXml());
                    return new DatosComercialAgentDto { EX_GRUPO_COMPRAS = valor1.EX_GRUPO_COMPRAS, EX_ZONA = valor1.EX_ZONA };
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

    }
}
