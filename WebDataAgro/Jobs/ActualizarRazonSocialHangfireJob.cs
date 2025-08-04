using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IActualizarRazonSocialHangfireJob : IHangfireJob { }

    public class ActualizarRazonSocialHangfireJob : IActualizarRazonSocialHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IProveedorManager proveedorManager;

        public ActualizarRazonSocialHangfireJob(ILogger logger, IRepositorio repositorio, IProveedorManager proveedorManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.proveedorManager = proveedorManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarRazonSocialHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO ActualizarRazonSocial");
            proveedorManager.ActualizarRazonSocial();
            logger.Info("FIN ActualizarRazonSocial");
        }
    }
}
