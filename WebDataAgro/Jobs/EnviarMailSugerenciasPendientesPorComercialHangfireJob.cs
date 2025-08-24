using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEnviarMailSugerenciasPendientesPorComercialHangfireJob : IHangfireJob { }

    public class EnviarMailSugerenciasPendientesPorComercialHangfireJob : IEnviarMailSugerenciasPendientesPorComercialHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public EnviarMailSugerenciasPendientesPorComercialHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnviarMailSugerenciasPendientesPorComercialHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO EnviarMailSugerenciasPendientesPorComercial");
            cupoManager.EnviarMailSugerenciasPendientesPorComercial();
            logger.Info("HANGFIRE - FIN EnviarMailSugerenciasPendientesPorComercial");
        }
    }
}
