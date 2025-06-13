using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEnviarMailConfirmaHangfireJob : IHangfireJob { }

    public class EnviarMailConfirmaHangfireJob : IEnviarMailConfirmaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IConfirmaManager confirmaManager;

        public EnviarMailConfirmaHangfireJob(ILogger logger, IRepositorio repositorio, IConfirmaManager confirmaManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.confirmaManager = confirmaManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnviarMailConfirmaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            DateTime dia = DateTime.Today;
            logger.Info("INICIO EnviarMailConfirma");
            confirmaManager.EnviarMailConfirma(dia);
            logger.Info("FIN EnviarMailConfirma");
        }
    }
}
