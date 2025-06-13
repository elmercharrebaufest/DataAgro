using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;
namespace WebDataAgro.Jobs
{
    public interface IActualizarPrecioPizarraHangfireJob : IHangfireJob { }

    public class ActualizarPrecioPizarraHangfireJob : IActualizarPrecioPizarraHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IPrecioPizarraManager precioPizarraManager;

        public ActualizarPrecioPizarraHangfireJob(ILogger logger, IRepositorio repositorio, IPrecioPizarraManager precioPizarraManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.precioPizarraManager = precioPizarraManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarPrecioPizarraHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            if (DateTime.Now > DateTime.Now.Date.AddHours(12) && DateTime.Now < DateTime.Now.Date.AddHours(13).AddMinutes(1))
            {
                logger.Info("INICIO ActualizarPrecioPizarra");
                precioPizarraManager.ActualizarPrecioPizarra(DateTime.Now.Date.AddDays(-1), false);
                logger.Info("FIN ActualizarPrecioPizarra");
            }
        }
    }
}
