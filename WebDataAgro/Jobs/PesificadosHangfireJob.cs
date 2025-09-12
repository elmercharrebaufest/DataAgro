using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IPesificadosHangfireJob : IHangfireJob { }

    public class PesificadosHangfireJob : IPesificadosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IReportesManager reportesManager;
        private static readonly object _lockPesificados = new object();

        public PesificadosHangfireJob(ILogger logger, IRepositorio repositorio, IReportesManager reportesManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.reportesManager = reportesManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "PesificadosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            lock (_lockPesificados)
            {
                logger.Info("HANGFIRE - INICIO Pesificados - " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                reportesManager.GrabarTodoDatoPesificar();
                logger.Info("HANGFIRE - FIN Pesificados - " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
            }
        }
    }
}
