using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IEnvioMailPendientesHangfireJob : IHangfireJob { }

    public class EnvioMailPendientesHangfireJob : IEnvioMailPendientesHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IContratoManager contratoManager;

        public EnvioMailPendientesHangfireJob(ILogger logger, IRepositorio repositorio, IContratoManager contratoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratoManager = contratoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "EnvioMailPendientesHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución EnvioMailPendientesHangfireJob iniciada");
            contratoManager.EnviarMailPendiente();
            logger.Info($"EnvioMail - Finalizado");
        }
    }
}
