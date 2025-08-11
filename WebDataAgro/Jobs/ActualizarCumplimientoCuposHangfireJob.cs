
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IActualizarCumplimientoCuposHangfireJob : IHangfireJob { }

    public class ActualizarCumplimientoCuposHangfireJob : IActualizarCumplimientoCuposHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public ActualizarCumplimientoCuposHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager; // Assigning the field to resolve the diagnostic
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarCumplimientoCuposHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO ActualizarCumplimientoCupos");
            cupoManager.ActualizarCumplimientoCupos(DateTime.Now.Date.AddDays(-1));
            logger.Info("FIN ActualizarCumplimientoCupos");
        }
    }
}
