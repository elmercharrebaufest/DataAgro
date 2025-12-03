using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IRechazarSolicitudesVencidasHangfireJob : IHangfireJob { }

    public class RechazarSolicitudesVencidasHangfireJob : IRechazarSolicitudesVencidasHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IAdministracionCupoManager administracionCupoManager;

        public RechazarSolicitudesVencidasHangfireJob(ILogger logger, IRepositorio repositorio, IAdministracionCupoManager administracionCupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.administracionCupoManager = administracionCupoManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "RechazarSolicitudesVencidasHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - Ejecución RechazarSolicitudesVencidasHangfireJob iniciada");
            administracionCupoManager.RechazarSolicitudesVencidas();
            logger.Info("HANGFIRE - Ejecución RechazarSolicitudesVencidasHangfireJob finalizada");
        }
    }
}
