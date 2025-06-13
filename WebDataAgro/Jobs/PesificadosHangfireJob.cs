using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IPesificadosHangfireJob : IHangfireJob { }

    public class PesificadosHangfireJob : IPesificadosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public PesificadosHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "PesificadosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución PesificadosHangfireJob iniciada");
        }
    }
}
