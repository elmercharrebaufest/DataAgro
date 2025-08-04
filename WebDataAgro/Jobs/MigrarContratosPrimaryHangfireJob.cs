using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IMigrarContratosPrimaryHangfireJob : IHangfireJob { }

    public class MigrarContratosPrimaryHangfireJob : IMigrarContratosPrimaryHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public MigrarContratosPrimaryHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "MigrarContratosPrimaryHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución MigrarContratosPrimaryHangfireJob iniciada");
            // TODO: Falta implementar
        }
    }
}
