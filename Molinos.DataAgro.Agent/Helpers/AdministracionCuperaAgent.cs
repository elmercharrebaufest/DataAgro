using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class AdministracionCuperaAgent : IAdministracionCuperaAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public AdministracionCuperaAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string AdministrarCupera(ConfiguracionCupoDto c)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var zonas = new List<Zmpes6540>();
                    zonas = c.CantidadCupo.Select(a =>
                        new Zmpes6540
                        {
                            ImLimiteCupos = a.CantidadCupo.ToString(),
                            ImLimiteCuposAnt = a.CantidadCupoAnterior.ToString(),
                            ImZona = a.ZonaCupo
                        }
                    ).ToList();
                    var rq = new ZMprfcAdminCupos()
                    {
                        ImCierre = c.CierreCupera.HasValue && c.CierreCupera.Value ? "X" : "",
                        ImFecha = c.Fecha.ToString("yyyy-MM-dd"),
                        ImLimiteCupos = c.LimiteCupo.ToString(),
                        ImLimiteCuposAnt = c.LimiteCupoAnterior.ToString(),
                        ImMatnr = c.Material,
                        //IM_ZONA = c.ZonaCupo,
                        ImWerks = c.Centro,
                        ImApertura = zonas.ToArray(),
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.ZMprfcAdminCupos(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return valor.ExMensaje;

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
