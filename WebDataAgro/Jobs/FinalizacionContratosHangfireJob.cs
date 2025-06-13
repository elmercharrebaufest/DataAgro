using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using WebDataAgro.Job;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Jobs
{
    public interface IFinalizacionContratosHangfireJob : IHangfireJob { }

    public class FinalizacionContratosHangfireJob : IFinalizacionContratosHangfireJob
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IContratoManager contratoManager;
        private readonly IFijacionDePrecioContratoManager fijacionManager;

        public FinalizacionContratosHangfireJob(ILogger logger, IRepositorio repositorio, IContratoManager contratoManager, IFijacionDePrecioContratoManager fijacionManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratoManager = contratoManager;
            this.fijacionManager = fijacionManager;
        }

        public void Execute()
        {
            var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "FinalizacionContratosHangfireJob");
            if (habilitacion == null || !habilitacion.Habilitado)
                return;

            logger.Info($"Finalización Automatica - Iniciando");
            contratoManager.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory);
            fijacionManager.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory);
            logger.Info($"Finalización Automatica - Finalizado");
        }
    }
}
