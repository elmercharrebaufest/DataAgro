using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using WebDataAgro.Helpers.Excel;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface ICerrarDiaHangfireJob : IHangfireJob { }

    public class CerrarDiaHangfireJob : ICerrarDiaHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IHedgeManager hedgeManager;
        private readonly IReportesManager reportesManager;
        private readonly IDiferencialManager diferencialManager;

        public CerrarDiaHangfireJob(ILogger logger, IRepositorio repositorio, IHedgeManager hedgeManager, 
            IReportesManager reportesManager, IDiferencialManager diferencialManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.hedgeManager = hedgeManager;
            this.reportesManager = reportesManager;
            this.diferencialManager = diferencialManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "CerrarDiaHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("HANGFIRE - INICIO CerrarDiaHedge");
            try
            {
                var mailEnviar = ExcelReporteCompleto.GenerarExcel(hedgeManager.ObtenerDatosReporte(), reportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true);
                var diferencial = diferencialManager.TraerDiferencial();

                hedgeManager.JobCerrarDia(44, mailEnviar, diferencial == null ? 0 : diferencial.DiferencialDefault);
                logger.Info("HANGFIRE - FIN CerrarDiaHedge");
            }
            catch (Exception ex)
            {
                logger.Error("HANGFIRE - Error al ejecutar job CerrarDiaHangfireJob", ex);
                throw;
            }
        }
    }
}
