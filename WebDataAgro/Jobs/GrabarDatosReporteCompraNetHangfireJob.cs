using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IGrabarDatosReporteCompraNetHangfireJob : IHangfireJob { }

    public class GrabarDatosReporteCompraNetHangfireJob : IGrabarDatosReporteCompraNetHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IReportesManager reportesManager;

        public GrabarDatosReporteCompraNetHangfireJob(ILogger logger, IRepositorio repositorio, IReportesManager reportesManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.reportesManager = reportesManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "GrabarDatosReporteCompraNetHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO GrabarDatosReporteCompraNet");

            DateTime fechaD = DateTime.Now.Date;
            reportesManager.GrabarDatosReporteCompraNet(fechaD, fechaD, "0", null);

            logger.Info("HANGFIRE - FIN GrabarDatosReporteCompraNet");
        }
    }
}
