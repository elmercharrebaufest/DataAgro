using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ICrearSugerenciaCupoHangfireJob : IHangfireJob { }

    public class CrearSugerenciaCupoHangfireJob : ICrearSugerenciaCupoHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CrearSugerenciaCupoHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "CrearSugerenciaCupoHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución CrearSugerenciaCupoHangfireJob iniciada");
        }
    }
}
