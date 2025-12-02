using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IBorradoContratosHangfireJob : IHangfireJob { }

    public class BorradoContratosHangfireJob : IBorradoContratosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IContratoManager contratoManager;

        public BorradoContratosHangfireJob(ILogger logger, IRepositorio repositorio, IContratoManager contratoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratoManager = contratoManager; // Assigning the field to resolve the diagnostic
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "BorradoContratosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info($"HANGFIRE - Borrado Automatico - Iniciando");
            contratoManager.BorradoAutomatico();
            logger.Info($"HANGFIRE - Borrado Automatico - Finalizado");
        }
    }
}
