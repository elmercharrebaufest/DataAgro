using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IAnularAcuerdosHangfireJob : IHangfireJob { }

    public class AnularAcuerdosHangfireJob : IAnularAcuerdosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IContratoAcuerdoManager contratoAcuerdoManager;

        public AnularAcuerdosHangfireJob(ILogger logger, IRepositorio repositorio, IContratoAcuerdoManager contratoAcuerdoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratoAcuerdoManager = contratoAcuerdoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "AnularAcuerdosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO AnularAcuerdos (cantidad Pendiente de Acuerdos)");
            contratoAcuerdoManager.AnularAcuerdos();
            logger.Info("FIN AnularAcuerdos");
        }
    }
}
