using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IVerificarSolicitudesExtraordinariasPendientesHangfireJob : IHangfireJob { }

    public class VerificarSolicitudesExtraordinariasPendientesHangfireJob : IVerificarSolicitudesExtraordinariasPendientesHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public VerificarSolicitudesExtraordinariasPendientesHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "VerificarSolicitudesExtraordinariasPendientesHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            DateTime dia = DateTime.Today;
            logger.Info("HANGFIRE - INICIO VerificarSolicitudesExtraordinariasPendientes");
            cupoManager.VerificarSolicitudesExtraordinariasPendientes(dia);
            logger.Info("HANGFIRE - FIN VerificarSolicitudesExtraordinariasPendientes");
        }
    }
}
