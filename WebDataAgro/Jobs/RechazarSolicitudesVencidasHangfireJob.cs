using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IRechazarSolicitudesVencidasHangfireJob : IHangfireJob { }

    public class RechazarSolicitudesVencidasHangfireJob : IRechazarSolicitudesVencidasHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public RechazarSolicitudesVencidasHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "RechazarSolicitudesVencidasHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - Ejecución RechazarSolicitudesVencidasHangfireJob iniciada");
            // TODO: Falta implementar
        }
    }
}
