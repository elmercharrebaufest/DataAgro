using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ISincronizarResearchHangfireJob : IHangfireJob { }

    public class SincronizarResearchHangfireJob : ISincronizarResearchHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IResearchManager researchManager;

        public SincronizarResearchHangfireJob(ILogger logger, IRepositorio repositorio, IResearchManager researchManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.researchManager = researchManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "SincronizarResearchHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO SincronizarResearch Power App");
            researchManager.SincronizarResearchPowerApp();
            logger.Info("HANGFIRE - FIN SincronizarResearch Power App");
        }
    }
}
