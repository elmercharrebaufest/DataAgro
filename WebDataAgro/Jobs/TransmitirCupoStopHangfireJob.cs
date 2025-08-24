using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ITransmitirCupoStopHangfireJob : IHangfireJob { }

    public class TransmitirCupoStopHangfireJob : ITransmitirCupoStopHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public TransmitirCupoStopHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager; // Assigning the field to resolve the diagnostic
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "TransmitirCupoStopHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ActualizarCumplimientoCupos");
            try
            {
                cupoManager.TransmitirCupos();
            }
            catch (Exception ex)
            {
                logger.Error("HANGFIRE - Error en ActualizarCumplimientoCupos.", ex);
            }
            logger.Info("HANGFIRE - FIN ActualizarCumplimientoCupos");
        }
    }
}
