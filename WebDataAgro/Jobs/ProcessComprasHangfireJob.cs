using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IProcessComprasHangfireJob : IHangfireJob { }

    public class ProcessComprasHangfireJob : IProcessComprasHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComprasManager comprasManager;

        public ProcessComprasHangfireJob(ILogger logger, IRepositorio repositorio, IComprasManager comprasManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.comprasManager = comprasManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessComprasHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ProcessCompras");
            comprasManager.ActualizarComprasDetalle("", "");
            logger.Info("HANGFIRE - FIN ProcessCompras");
        }
    }
}
