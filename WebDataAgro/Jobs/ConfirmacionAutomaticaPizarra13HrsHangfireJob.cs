using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;

namespace WebDataAgro.Jobs
{
    public interface IConfirmacionAutomaticaPizarra13HrsHangfireJob : IHangfireJob { }

    public class ConfirmacionAutomaticaPizarra13HrsHangfireJob : IConfirmacionAutomaticaPizarra13HrsHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IFijacionDePrecioContratoManager fijacionManager;

        public ConfirmacionAutomaticaPizarra13HrsHangfireJob(ILogger logger, IRepositorio repositorio, IFijacionDePrecioContratoManager fijacionManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.fijacionManager = fijacionManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "ConfirmacionAutomaticaPizarra13HrsHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info("INICIO ConfirmacionAutomaticaPizarra13hrs");
            fijacionManager.ConfirmacionAutomaticaPizarra13Hrs();
            logger.Info("FIN ConfirmacionAutomaticaPizarra13hrs");
        }
    }
}
