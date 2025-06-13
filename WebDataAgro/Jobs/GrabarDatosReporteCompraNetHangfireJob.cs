using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IGrabarDatosReporteCompraNetHangfireJob : IHangfireJob { }

    public class GrabarDatosReporteCompraNetHangfireJob : IGrabarDatosReporteCompraNetHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public GrabarDatosReporteCompraNetHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "GrabarDatosReporteCompraNetHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("Ejecución GrabarDatosReporteCompraNetHangfireJob iniciada");
        }
    }
}
