using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEnvioMailSinCTGHangfireJob : IHangfireJob { }

    public class EnvioMailSinCTGHangfireJob : IEnvioMailSinCTGHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public EnvioMailSinCTGHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnvioMailSinCTGHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución EnvioMailSinCTGHangfireJob iniciada");
        }
    }
}
