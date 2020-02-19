using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;
using System.Web.Script.Serialization;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System;

namespace WebDataAgro.Controllers
{
    public class TareasProgramadasController : Controller
    {
        private readonly ILogger logger;
        private readonly IContratoManager contratoManager;
        private readonly IFijacionDePrecioContratoManager fijacionManager;
        private readonly ICupoManager cupoManager;
        private readonly IContratoAcuerdoManager contratoAcuerdoManager;

        public TareasProgramadasController(ILogger logger, IContratoManager contratoManager, IFijacionDePrecioContratoManager fijacionManager, ICupoManager cupoManager, IContratoAcuerdoManager contratoAcuerdoManager)

        {
            this.logger = logger;
            this.contratoManager = contratoManager;
            this.fijacionManager = fijacionManager;
            this.cupoManager = cupoManager;
            this.contratoAcuerdoManager = contratoAcuerdoManager;
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
            fijacionManager.FinalizacionAutomatica(GlobalVariables.IdActiveDirectory);
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

        public ActionResult TransmitirCupoStop()
        {
            logger.Info($"Transmitiendo cupos a STOP");
            cupoManager.TransmitirCupos();
            logger.Info($"Borrado Automatico - Finalizado");
            return Content("ok");
        }
        public ActionResult ConsultarCuposDiarios()
        {
            logger.Info($"Transmitiendo cupos a STOP");
            cupoManager.ConsultarCuposDiarios();
            logger.Info($"Consulta Cupos a Stop - Finalizado");
            return Content("ok");
        }

        public ActionResult AnularAcuerdos()
        {
            logger.Info($"Anulando cantidad Pendiente de Acuerdos");
            contratoAcuerdoManager.AnularAcuerdos();
            logger.Info($"Anular - Finalizado");
            return Content("ok");
        }

        public ActionResult CrearSugerenciaCupo()
        {
            logger.Info($"CrearSugerenciaCupo");
            cupoManager.CrearSugerenciaCupo();
            logger.Info($"CrearSugerenciaCupo - Finalizado");
            return Content("ok");
        }

    }
}