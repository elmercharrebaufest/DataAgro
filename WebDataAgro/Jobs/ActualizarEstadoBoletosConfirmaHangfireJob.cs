using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces.Managers;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IActualizarEstadoBoletosConfirmaHangfireJob : IHangfireJob { }

    public class ActualizarEstadoBoletosConfirmaHangfireJob : IActualizarEstadoBoletosConfirmaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IControlDeBoletosManager controlDeBoletosManager;

        public ActualizarEstadoBoletosConfirmaHangfireJob(ILogger logger, IRepositorio repositorio, IControlDeBoletosManager controlDeBoletosManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.controlDeBoletosManager = controlDeBoletosManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarEstadoBoletosConfirmaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ActualizarEstadoBoletosConfirma");
            controlDeBoletosManager.ActualizarEstadoBoletosConfirma();
            logger.Info("HANGFIRE - FIN ActualizarEstadoBoletosConfirma");
        }
    }
}
