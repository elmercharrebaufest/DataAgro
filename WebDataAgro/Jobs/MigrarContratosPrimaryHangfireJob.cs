using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IMigrarContratosPrimaryHangfireJob : IHangfireJob { }

    public class MigrarContratosPrimaryHangfireJob : IMigrarContratosPrimaryHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly INegocioManager negocioManager;

        public MigrarContratosPrimaryHangfireJob(ILogger logger, IRepositorio repositorio, INegocioManager negocioManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.negocioManager = negocioManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "MigrarContratosPrimaryHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - Ejecución MigrarContratosPrimaryHangfireJob iniciada");

            DateTime dia = DateTime.Today;
            
            if (!(dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday))
                negocioManager.MigrarContratosPrimary(dia);

            logger.Info("HANGFIRE - Ejecución MigrarContratosPrimaryHangfireJob finalizada");
        }
    }
}
