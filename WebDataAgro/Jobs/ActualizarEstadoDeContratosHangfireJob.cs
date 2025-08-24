using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IActualizarEstadoDeContratosHangfireJob : IHangfireJob { }

    public class ActualizarEstadoDeContratosHangfireJob : IActualizarEstadoDeContratosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IContratoManager contratoManager;

        public ActualizarEstadoDeContratosHangfireJob(ILogger logger, IRepositorio repositorio, IContratoManager contratoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratoManager = contratoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarEstadoDeContratosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ActualizarEstadoDeContrato");
            contratoManager.ActualizarEstadoDeContratos();
            logger.Info("HANGFIRE - FIN ActualizarEstadoDeContrato");
        }
    }
}
