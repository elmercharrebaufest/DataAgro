using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IProcessCapacidadProductivaHangfireJob : IHangfireJob { }

    public class ProcessCapacidadProductivaHangfireJob : IProcessCapacidadProductivaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICapacidadProductivaManager capProdManager;

        public ProcessCapacidadProductivaHangfireJob(ILogger logger, IRepositorio repositorio, ICapacidadProductivaManager capProdManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.capProdManager = capProdManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessCapacidadProductivaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO ProcessCapacidadProductiva");
            capProdManager.ActualizarCapacidadProductiva();
            logger.Info("FIN ProcessCapacidadProductiva");
        }
    }
}
