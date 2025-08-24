using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IConsultarCuposDiariosHangfireJob : IHangfireJob { }

    public class ConsultarCuposDiariosHangfireJob : IConsultarCuposDiariosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public ConsultarCuposDiariosHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ConsultarCuposDiariosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ConsultarCuposDiarios a STOP");
            cupoManager.ConsultarCuposDiarios();
            logger.Info("HANGFIRE - FIN ConsultarCuposDiarios a STOP");
        }
    }
}
