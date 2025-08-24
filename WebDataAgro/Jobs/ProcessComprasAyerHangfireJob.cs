using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IProcessComprasAyerHangfireJob : IHangfireJob { }

    public class ProcessComprasAyerHangfireJob : IProcessComprasAyerHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComprasManager comprasManager;

        public ProcessComprasAyerHangfireJob(ILogger logger, IRepositorio repositorio, IComprasManager comprasManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.comprasManager = comprasManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessComprasAyerHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ProcessComprasAyer");
            comprasManager.ActualizarComprasAyer();
            logger.Info("HANGFIRE - FIN ProcessComprasAyer");
        }
    }
}
