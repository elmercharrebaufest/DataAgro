using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IProcessEstadoHangfireJob : IHangfireJob { }

    public class ProcessEstadoHangfireJob : IProcessEstadoHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IEstadoProveedorManager estadoProveedorManager;

        public ProcessEstadoHangfireJob(ILogger logger, IRepositorio repositorio, IEstadoProveedorManager estadoProveedorManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.estadoProveedorManager = estadoProveedorManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ProcessEstadoHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ProcessEstado");
            estadoProveedorManager.ActualizarProveedores(null);
            logger.Info("HANGFIRE - FIN ProcessEstado");
        }
    }
}
