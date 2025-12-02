using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IActualizarScoringCuposDeProveedoresHangfireJob : IHangfireJob { }

    public class ActualizarScoringCuposDeProveedoresHangfireJob : IActualizarScoringCuposDeProveedoresHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IProveedorManager proveedorManager;

        public ActualizarScoringCuposDeProveedoresHangfireJob(ILogger logger, IRepositorio repositorio, IProveedorManager proveedorManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.proveedorManager = proveedorManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ActualizarScoringCuposDeProveedoresHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO ActualizarScoringCuposDeProveedores Hangfire");
            proveedorManager.ActualizarScoringCuposDeProveedores();
            logger.Info("HANGFIRE - FIN ActualizarScoringCuposDeProveedores Hangfire");
        }
    }
}