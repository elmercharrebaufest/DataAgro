using Autofac.Extras.NLog;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class TareasProgramadasController : Controller
    {
        private readonly ILogger logger;
        private readonly IContratoManager contratoManager;

        public TareasProgramadasController(ILogger logger, IContratoManager contratoManager)
        {
            this.logger = logger;
            this.contratoManager = contratoManager;
        }

        public ActionResult EnvioMailPendientes()
        {
            logger.Info($"EnvioMail - Iniciando");
            contratoManager.EnviarMailPendiente();
            logger.Info($"EnvioMail - Finalizado");
            return Content("ok");
        }

        public ActionResult FinalizacionContratos()
        {
            logger.Info($"Finalización Automatica - Iniciando");
            contratoManager.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory);
            logger.Info($"Finalización Automatica - Finalizado");
            return Content("ok");
        }

        public ActionResult BorradoContratos()
        {
            logger.Info($"Borrado Automatico - Iniciando");
            contratoManager.BorradoAutomatico();
            logger.Info($"Borrado Automatico - Finalizado");
            return Content("ok");
        }
    }
}