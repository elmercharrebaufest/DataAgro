using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEnvioMailNegociosConDiaAnteriorHangfireJob : IHangfireJob { }

    public class EnvioMailNegociosConDiaAnteriorHangfireJob : IEnvioMailNegociosConDiaAnteriorHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly INegocioManager negocioManager;

        public EnvioMailNegociosConDiaAnteriorHangfireJob(ILogger logger, IRepositorio repositorio, INegocioManager negocioManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.negocioManager = negocioManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnvioMailNegociosConDiaAnteriorHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO EnvioMailNegociosConDiaAnterior");
            negocioManager.EnvioMailNegociosConDiaAnterior();
            logger.Info("FIN EnvioMailNegociosConDiaAnterior");
        }
    }
}
