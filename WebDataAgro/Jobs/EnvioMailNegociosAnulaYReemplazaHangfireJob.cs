using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;
namespace WebDataAgro.Jobs
{
    public interface IEnvioMailNegociosAnulaYReemplazaHangfireJob : IHangfireJob { }

    public class EnvioMailNegociosAnulaYReemplazaHangfireJob : IEnvioMailNegociosAnulaYReemplazaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly INegocioManager negocioManager;

        public EnvioMailNegociosAnulaYReemplazaHangfireJob(ILogger logger, IRepositorio repositorio, INegocioManager negocioManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.negocioManager = negocioManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnvioMailNegociosAnulaYReemplazaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO EnvioMailNegociosAnulaYReemplaza");
            negocioManager.EnvioMailNegociosAnulaYReemplaza();
            logger.Info("HANGFIRE - FIN EnvioMailNegociosAnulaYReemplaza");
        }
    }
}
