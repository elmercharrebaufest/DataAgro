using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEliminarLogsAntiguosHangfireJob : IHangfireJob { }

    public class EliminarLogsAntiguosHangfireJob : IEliminarLogsAntiguosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogManager logManager;

        public EliminarLogsAntiguosHangfireJob(ILogger logger, IRepositorio repositorio, ILogManager logManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logManager = logManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EliminarLogsAntiguosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - EliminarLogsAntiguosHangfireJob - Iniciando");

            try
            {
                logManager.EliminarLogsAntiguos();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info($"HANGFIRE - EliminarLogsAntiguosHangfireJob - Fin");
        }
    }
}
