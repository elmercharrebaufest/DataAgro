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

            string idActiveDirectory = "DATAAGRO";

            logger.Info($"HANGFIRE - FinalizacionContratos - Contratos - Inicio");
            contratoManager.FinalizacionAutomatica(idActiveDirectory);
            logger.Info($"HANGFIRE - FinalizacionContratos - Contratos - Fin");

            logger.Info($"HANGFIRE - FinalizacionContratos - Fijaciones - Inicio");
            fijacionManager.FinalizacionAutomatica(idActiveDirectory);
            logger.Info($"HANGFIRE - FinalizacionContratos - Fijaciones - Fin");
        }
    }
}
