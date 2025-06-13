using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ICerrarDiaHangfireJob : IHangfireJob { }

    public class CerrarDiaHangfireJob : ICerrarDiaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CerrarDiaHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "CerrarDiaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución CerrarDiaHangfireJob iniciada");
        }
    }
}
