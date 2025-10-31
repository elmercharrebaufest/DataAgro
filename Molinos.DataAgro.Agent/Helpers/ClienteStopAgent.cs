using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class ClienteStopAgent : IClienteStopAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly Func<ICupoManager> cupoManagerInj;
        private readonly ClienteStopV1Agent clienteStopV1Agent;
        private readonly ClienteStopV2Agent clienteStopV2Agent;
        private readonly string versionClienteStop = ConfigurationManager.AppSettings["VersionClienteSTOP"];

        public ClienteStopAgent(ILogger logger, IRepositorio repositorio, Func<ICupoManager> cupoManagerInj,
            ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
            this.cupoManagerInj = cupoManagerInj;
            this.clienteStopV1Agent = new ClienteStopV1Agent(logger, repositorio, cupoManagerInj, logDataAgroManager);
            this.clienteStopV2Agent = new ClienteStopV2Agent(logger, repositorio, cupoManagerInj, logDataAgroManager);
        }

        public void CrearCupo(List<string> listaCupos)
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    clienteStopV1Agent.CrearCupo(listaCupos);
                }
                if (versionClienteStop == "2.0.0")
                {
                    clienteStopV2Agent.CrearCupo(listaCupos);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al Crear Cupo.", e);
                throw;
            }
        }

        public void TransmitirJobCupos()
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    clienteStopV1Agent.TransmitirJobCupos();
                }
                if (versionClienteStop == "2.0.0")
                {
                    clienteStopV2Agent.TransmitirJobCupos();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al transmitir Cupos.", e);
                throw;
            }
        }

        public Resultado EliminarCupo(Cupo cupo, TokenStop tokenNuevo = null, RepositorioEF repo = null)
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    return clienteStopV1Agent.EliminarCupo(cupo, tokenNuevo, repo);
                }
                if (versionClienteStop == "2.0.0")
                {
                    return clienteStopV2Agent.EliminarCupo(cupo, tokenNuevo, repo);
                }

                return clienteStopV1Agent.EliminarCupo(cupo, tokenNuevo, repo);
            }
            catch (Exception e)
            {
                logger.Error("Error en EliminarCupo", e);
                throw;
            }
        }

        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    return clienteStopV1Agent.ConsultarCuposDiarios();
                }
                if (versionClienteStop == "2.0.0")
                {
                    return clienteStopV2Agent.ConsultarCuposDiarios();
                }

                return clienteStopV1Agent.ConsultarCuposDiarios();
            }
            catch (Exception e)
            {
                logger.Error("Error al consultar cupos diarias.", e);
                throw;
            }
        }

        public void ModificarCupo(Cupo cupo)
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    clienteStopV1Agent.ModificarCupo(cupo);
                }
                if (versionClienteStop == "2.0.0")
                {
                    clienteStopV2Agent.ModificarCupo(cupo);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al modificar Cupo", e);
                throw;
            }
        }

        #region MisturnosActivos(CupoNoPropio)
        public List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos()
        {
            try
            {
                if (versionClienteStop == "1.1.0")
                {
                    return clienteStopV1Agent.ConsultarMisTurnosActivos();
                }
                if (versionClienteStop == "2.0.0")
                {
                    return clienteStopV2Agent.ConsultarMisTurnosActivos();
                }
                return clienteStopV1Agent.ConsultarMisTurnosActivos();
            }
            catch (Exception e)
            {
                logger.Error("Error al consultar turnos activos", e);
                throw;
            }
        }
        #endregion

        public TokenStop ObtenerTokenStop(RepositorioEF repo = null)
        {
            var r = repo ?? repositorio;
            var datosConfiguracion = r.Obtener<Configuracion>(1);

            if (versionClienteStop == "1.1.0")
            {
                return clienteStopV1Agent.ObtenerToken(datosConfiguracion.ClaveStop);
            }
            else if (versionClienteStop == "2.0.0")
            {
                return clienteStopV2Agent.ObtenerToken(datosConfiguracion.ClaveStop);
            }
            else
            {
                return clienteStopV1Agent.ObtenerToken(datosConfiguracion.ClaveStop);
            }
        }
    }
}
