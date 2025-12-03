using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IReportePagosDiferidosHangfireJob : IHangfireJob { }

    public class ReportePagosDiferidosHangfireJob : IReportePagosDiferidosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public ReportePagosDiferidosHangfireJob(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ReportePagosDiferidosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - Ejecución ReportePagosDiferidosHangfireJob iniciada");
            // TODO: Falta implementar
        }
    }
}
