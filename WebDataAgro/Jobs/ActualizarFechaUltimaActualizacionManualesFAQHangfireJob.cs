using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;
namespace WebDataAgro.Jobs
{
    public interface IActualizarFechaUltimaActualizacionManualesFAQHangfireJob : IHangfireJob { }

    public class ActualizarFechaUltimaActualizacionManualesFAQHangfireJob : IActualizarFechaUltimaActualizacionManualesFAQHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IFAQManager faqManager;

        public ActualizarFechaUltimaActualizacionManualesFAQHangfireJob(ILogger logger, IRepositorio repositorio, IFAQManager faqManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.faqManager = faqManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarFechaUltimaActualizacionManualesFAQHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO Actualizar fecha última actualización Manuales FAQ");
            faqManager.ActualizarFechaUltimaActualizacionManualesFAQ();
            logger.Info("FIN Actualizar fecha última actualización Manuales FAQ");
        }
    }
}
