using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IConsultarMisturnosActivosHangfireJob : IHangfireJob { }

    public class ConsultarMisturnosActivosHangfireJob : IConsultarMisturnosActivosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public ConsultarMisturnosActivosHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ConsultarMisturnosActivosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            if (DateTime.Now >= DateTime.Now.Date.AddHours(7) && DateTime.Now <= DateTime.Now.Date.AddHours(21))
            {
                logger.Info("HANGFIRE - INICIO ConsultarMisturnosActivos - Actualizar CupoNoPropio");
                cupoManager.ConsultarMisTurnosActivos();
                logger.Info("HANGFIRE - FIN ConsultarMisturnosActivos - Actualizar CupoNoPropio");
            }
            else
            {
                logger.Info("Fuera de Rango ConsultarMisturnosActivos - Actualizar CupoNoPropio");
            }
        }
    }
}
