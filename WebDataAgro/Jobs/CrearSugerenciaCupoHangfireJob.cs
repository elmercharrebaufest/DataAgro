using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ICrearSugerenciaCupoHangfireJob : IHangfireJob { }

    public class CrearSugerenciaCupoHangfireJob : ICrearSugerenciaCupoHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;

        public CrearSugerenciaCupoHangfireJob(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "CrearSugerenciaCupoHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO CrearSugerenciaCupo");
            cupoManager.CrearSugerenciaCupo();
            logger.Info("FIN CrearSugerenciaCupo");
        }
    }
}
