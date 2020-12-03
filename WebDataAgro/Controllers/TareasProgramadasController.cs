using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;
using System.Web.Script.Serialization;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System;
using Molinos.DataAgro.Entities.Dto;
using WebDataAgro.Helpers.Excel;
using System.Linq;

namespace WebDataAgro.Controllers
{
    public class TareasProgramadasController : Controller
    {
        private readonly ILogger logger;
        private readonly IContratoManager contratoManager;
        private readonly IFijacionDePrecioContratoManager fijacionManager;
        private readonly ICupoManager cupoManager;
        private readonly IContratoAcuerdoManager contratoAcuerdoManager;
        private readonly IReportesManager reportesManager;
        private readonly INegocioManager negocioManager;
        private readonly IAdministracionCupoManager administracionCupoManager;
        private readonly IHedgeManager oHedgeManager;
        private readonly IDiferencialManager diferencialManager;


        public TareasProgramadasController(ILogger logger, IContratoManager contratoManager, IFijacionDePrecioContratoManager fijacionManager, 
            ICupoManager cupoManager, IContratoAcuerdoManager contratoAcuerdoManager, 
            IReportesManager reportesManager, INegocioManager negocioManager, 
            IAdministracionCupoManager administracionCupoManager, IHedgeManager oHedgeManager, IDiferencialManager diferencialManager)

        {
            this.logger = logger;
            this.contratoManager = contratoManager;
            this.fijacionManager = fijacionManager;
            this.cupoManager = cupoManager;
            this.contratoAcuerdoManager = contratoAcuerdoManager;
            this.reportesManager = reportesManager;
            this.negocioManager = negocioManager;
            this.administracionCupoManager = administracionCupoManager;
            this.oHedgeManager = oHedgeManager;
            this.diferencialManager = diferencialManager;
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
            cupoManager.CrearSugerenciaCupo(null);
            logger.Info($"CrearSugerenciaCupo - Finalizado");
            return Content("ok");
        }

        public ActionResult GrabarDatosReporteCompraNet(string fecha)
        {
            DateTime fechaD = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
            {
                fechaD = DateTime.ParseExact(fecha, "yyyyMMdd", null);
            }
            logger.Info($"ReporteCompraNet");
            reportesManager.GrabarDatosReporteCompraNet(fechaD, fechaD, "0", null);

            logger.Info($"ReporteCompraNet - Finalizado");
            return Content("ok");
        }

        public ActionResult EnvioMailSinCTG()
        {
            logger.Info($"EnvioMailSinCtg - Iniciando");
            cupoManager.EnviarMailSinCtg();
            logger.Info($"EnvioMailSinCtg - Finalizado");
            return Content("ok");
        }

        public ActionResult EnvioMailNegociosConDiaAnterior()
        {
            logger.Info($"EnvioMailNegociosConDiaAnterior - Iniciando");
            negocioManager.EnvioMailNegociosConDiaAnterior();
            logger.Info($"EnvioMailNegociosConDiaAnterior - Finalizado");
            return Content("ok");
        }

        public ActionResult RechazarSolicitudesVencidas()
        {
            logger.Info($"RechazarSolicitudesVencidas - Iniciando");
            administracionCupoManager.RechazarSolicitudesVencidas();
            logger.Info($"RechazarSolicitudesVencidas - Finalizado");
            return Content("ok");
        }


        public ActionResult CerrarDia()
        {
            logger.Info($"CerrarDiaHedge - Iniciando");
            var mailEnviar = ExcelReporteCompleto.GenerarExcel(oHedgeManager.ObtenerDatosReporte(), reportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true);
            var hoy = DateTime.Now.Date;
            oHedgeManager.EnviarMail(GlobalVariables.ComercialId, hoy, oHedgeManager.GenerarCuerpoMail(""), mailEnviar);          
            logger.Info($"CerrarDiaHedge - Finalizado");
            return Content("ok");
        }
    }
}