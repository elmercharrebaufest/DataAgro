using Molinos.DataAgro.Interfaces.Managers;
using static WebDataAgro.MvcApplication;
using Molinos.DataAgro.Interfaces;
using WebDataAgro.Helpers.Excel;
using Autofac.Extras.NLog;
using System.Web.Mvc;
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
        private readonly IReportesManager reportesManager;
        private readonly INegocioManager negocioManager;
        private readonly IAdministracionCupoManager administracionCupoManager;
        private readonly IHedgeManager oHedgeManager;
        private readonly IDiferencialManager diferencialManager;
        private readonly IProveedorManager proveedorManager;
        private readonly IPrecioPizarraManager precioPizarraManager;
        private readonly IFAQManager faqManager;
        private readonly IResearchManager researchManager;

        public TareasProgramadasController(ILogger logger, IContratoManager contratoManager, IFijacionDePrecioContratoManager fijacionManager,
            ICupoManager cupoManager, IContratoAcuerdoManager contratoAcuerdoManager,
            IReportesManager reportesManager, INegocioManager negocioManager,
            IAdministracionCupoManager administracionCupoManager, IHedgeManager oHedgeManager, IDiferencialManager diferencialManager,
            IProveedorManager proveedorManager, IPrecioPizarraManager precioPizarraManager, IFAQManager faqManager, IResearchManager researchManager)

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
            this.proveedorManager = proveedorManager;
            this.precioPizarraManager = precioPizarraManager;
            this.faqManager = faqManager;
            this.researchManager = researchManager;
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
            logger.Info("INICIO TransmitirCupoStop");
            cupoManager.TransmitirCupos();
            logger.Info("FIN TransmitirCupoStop - Borrado Automatico");
            return Content("ok");
        }

        public ActionResult ConsultarCuposDiarios()
        {
            logger.Info("INICIO ConsultarCuposDiarios a STOP");
            cupoManager.ConsultarCuposDiarios();
            logger.Info("FIN ConsultarCuposDiarios a STOP");
            return Content("ok");
        }

        public ActionResult AnularAcuerdos()
        {
            logger.Info("INICIO AnularAcuerdos (cantidad Pendiente de Acuerdos)");
            contratoAcuerdoManager.AnularAcuerdos();
            logger.Info("FIN AnularAcuerdos");
            return Content("ok");
        }

        public ActionResult CrearSugerenciaCupo()
        {
            logger.Info("INICIO CrearSugerenciaCupo");
            cupoManager.CrearSugerenciaCupo();
            logger.Info("FIN CrearSugerenciaCupo");
            return Content("ok");
        }

        public ActionResult GrabarDatosReporteCompraNet(string fecha)
        {
            DateTime fechaD = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
            {
                fechaD = DateTime.ParseExact(fecha, "yyyyMMdd", null);
            }
            logger.Info("INICIO GrabarReporteCompraNet");
            reportesManager.GrabarDatosReporteCompraNet(fechaD, fechaD, "0", null);

            logger.Info("FIN GrabarReporteCompraNet");
            return Content("ok");
        }

        public ActionResult EnvioMailSinCTG()
        {
            logger.Info("INICIO EnvioMailSinCTG");
            cupoManager.EnviarMailSinCtg();
            logger.Info("FIN EnvioMailSinCTG");
            return Content("ok");
        }

        public ActionResult EnvioMailNegociosConDiaAnterior()
        {
            logger.Info("INICIO EnvioMailNegociosConDiaAnterior");
            negocioManager.EnvioMailNegociosConDiaAnterior();
            logger.Info("FIN EnvioMailNegociosConDiaAnterior");
            return Content("ok");
        }

        public ActionResult RechazarSolicitudesVencidas()
        {
            logger.Info("INICIO RechazarSolicitudesVencidas");
            administracionCupoManager.RechazarSolicitudesVencidas();
            logger.Info("FIN RechazarSolicitudesVencidas");
            return Content("ok");
        }

        public ActionResult CerrarDia()
        {
            logger.Info("INICIO CerrarDiaHedge");
            var mailEnviar = ExcelReporteCompleto.GenerarExcel(oHedgeManager.ObtenerDatosReporte(), reportesManager.PosicionPorMaterial(DateTime.Now, DateTime.Now), true);
            var diferencial = diferencialManager.TraerDiferencial();
            oHedgeManager.JobCerrarDia(GlobalVariables.ComercialId, GlobalVariables.IdActiveDirectory, mailEnviar, diferencial == null ? 0 : diferencial.DiferencialDefault);
            logger.Info("FIN CerrarDiaHedge");
            return Content("ok");
        }

        static readonly object _lockPesificados = new object();

        public ActionResult Pesificados()
        {
            lock (_lockPesificados)
            {
                logger.Info("INICIO Pesificados - " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                reportesManager.GrabarTodoDatoPesificar();
                logger.Info("FIN Pesificados - " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                return Content("ok");
            }

        }

        public ActionResult MigrarContratosPrimary(string fecha)
        {
            DateTime dia = DateTime.Now.Date;
            logger.Info("INICIO MigrarContratosPrimary");
            if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
            {
                dia = DateTime.ParseExact(fecha, "yyyyMMdd", null);
            }
            if (!(dia.DayOfWeek == DayOfWeek.Saturday || dia.DayOfWeek == DayOfWeek.Sunday))
                negocioManager.MigrarContratosPrimary(dia);
            logger.Info("FIN MigrarContratosPrimary");
            return Content("ok");
        }

        public ActionResult ActualizarRazonSocial()
        {
            logger.Info("INICIO ActualizarRazonSocial");
            try
            {
                proveedorManager.ActualizarRazonSocial();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info("FIN ActualizarRazonSocial");
            return Content("ok");
        }

        public ActionResult ReportePagosDiferidos(string fecha)
        {
            logger.Info("INICIO ReportePagosDiferidos");
            try
            {
                var hoy = DateTime.Now.Date;
                var ultimoDiaDelMesSiguiente = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(-1);
                DateTime? desde = null;
                if (hoy.DayOfWeek == DayOfWeek.Friday)//el semanal solo se envian los viernes 
                {
                    desde = hoy.AddDays(-7);
                }

                if (hoy == ultimoDiaDelMesSiguiente)// si es el  ultimo dia del mes manda el informe del mes
                {
                    desde = new DateTime(hoy.Year, hoy.Month, 1);
                }
                if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
                {
                    desde = DateTime.ParseExact(fecha, "yyyyMMdd", null);
                }
                if (desde.HasValue)
                {
                    var datos = reportesManager.ObtenerDatosReportePagosDiferidos(desde.Value, hoy);

                    var excel = ExcelReporteCompleto.ExcelReportePagosDiferidos(datos, datos.Desde, datos.Hasta);
                    reportesManager.EnviarMailReportePagosDiferidos(excel, desde.Value, hoy);

                }

            }
            catch (Exception ex)
            {
                logger.Error("ReportePagosDiferidos", ex);
            }
            logger.Info("FIN ReportePagosDiferidos");
            return Content("ok");
        }

        public ActionResult ActualizarCumplimientoCupos()
        {
            logger.Info("INICIO ActualizarCumplimientoCupos");
            try
            {
                cupoManager.ActualizarCumplimientoCupos(DateTime.Now.Date.AddDays(-1));
            }
            catch (Exception ex)
            {
                logger.Error("ActualizarCumplimientoCupos", ex);
            }
            logger.Info("FIN ActualizarCumplimientoCupos");
            return Content("ok");
        }

        public ActionResult ActualizarCumplimientoCuposMasivo(string fechaDesde, string fechaHasta)
        {
            logger.Info("INICIO ActualizarCumplimientoCuposMasivo");
            DateTime desde = DateTime.ParseExact(fechaDesde, "yyyyMMdd", null);
            DateTime hasta = DateTime.ParseExact(fechaHasta, "yyyyMMdd", null);
            for (var dt = desde; dt <= hasta; dt = dt.AddDays(1))
            {
                try
                {
                    cupoManager.ActualizarCumplimientoCupos(dt);
                }
                catch (Exception ex)
                {
                    logger.Info("ActualizarCumplimientoCuposMasivo - Error en el dia " + dt.ToString("yyyyMMdd"));
                    logger.Error(ex);
                }
            }
            logger.Info("FIN ActualizarCumplimientoCuposMasivo");
            return Content("ok");
        }

        public ActionResult EnvioMailNegociosAnulaYReemplaza()
        {
            logger.Info("INICIO EnvioMailNegociosAnulaYReemplaza");
            negocioManager.EnvioMailNegociosAnulaYReemplaza();
            logger.Info("FIN EnvioMailNegociosAnulaYReemplaza");
            return Content("ok");
        }

        public ActionResult EnviarMailSugerenciasPendientesPorComercial()
        {
            logger.Info("INICIO EnviarMailSugerenciasPendientesPorComercial");
            cupoManager.EnviarMailSugerenciasPendientesPorComercial();
            logger.Info("FIN EnviarMailSugerenciasPendientesPorComercial");
            return Content("ok");
        }

        public ActionResult ActualizarMailProveedor()
        {
            logger.Info("INICIO EnviarMailSugerenciasPendientesPorComercial");
            proveedorManager.GrabarMailProveedor();
            logger.Info("FIN EnviarMailSugerenciasPendientesPorComercial");
            return Content("ok");
        }

        public ActionResult ConsultarMisturnosActivos()
        {
            if (DateTime.Now >= DateTime.Now.Date.AddHours(7) && DateTime.Now <= DateTime.Now.Date.AddHours(21))
            {
                logger.Info("INICIO ConsultarMisturnosActivos - Actualizar CupoNoPropio");
                cupoManager.ConsultarMisTurnosActivos();
                logger.Info("FIN ConsultarMisturnosActivos - Actualizar CupoNoPropio");
            }
            else
            {
                logger.Info("Fuera de Rango ConsultarMisturnosActivos - Actualizar CupoNoPropio");
            }
            return Content("ok");
        }

        public ActionResult ActualizarPrecioPizarra()
        {
            if (DateTime.Now > DateTime.Now.Date.AddHours(12) && DateTime.Now < DateTime.Now.Date.AddHours(13).AddMinutes(1))
            {
                logger.Info("INICIO ActualizarPrecioPizarra");
                precioPizarraManager.ActualizarPrecioPizarra(DateTime.Now.Date.AddDays(-1), false);
                logger.Info("FIN ActualizarPrecioPizarra");
            }
            return Content("ok");
        }

        public ActionResult ActualizarProveedoresHome()
        {
            //if (DateTime.Now > DateTime.Now.Date.AddHours(10) && DateTime.Now < DateTime.Now.Date.AddHours(11).AddMinutes(1))
            //{
            logger.Info("INICIO ActualizarProveedoresHome");
            proveedorManager.ActualizarProveedoresHome();
            logger.Info("FIN ActualizarProveedoresHome");
            //}
            return Content("ok");
        }

        public ActionResult ActualizarEstadoDeContratos()
        {
            logger.Info("INICIO ActualizarEstadoDeContrato");
            contratoManager.ActualizarEstadoDeContratos();
            logger.Info("FIN ActualizarEstadoDeContrato");
            return Content("ok");
        }

        public ActionResult ConfirmacionAutomaticaPizarra13Hrs()
        {
            logger.Info("INICIO ConfirmacionAutomaticaPizarra13hrs");
            fijacionManager.ConfirmacionAutomaticaPizarra13Hrs();
            logger.Info("FIN ConfirmacionAutomaticaPizarra13hrs");
            return Content("ok");
        }

        public ActionResult ActualizarFechaUltimaActualizacionManualesFAQ()
        {
            logger.Info("INICIO Actualizar fecha última actualización Manuales FAQ");
            faqManager.ActualizarFechaUltimaActualizacionManualesFAQ();
            logger.Info("FIN Actualizar fecha última actualización Manuales FAQ");

            return Content("ok");
        }

        public ActionResult SincronizarResearch()
        {
            logger.Info("INICIO SincronizarResearch Power App");
            researchManager.SincronizarResearchPowerApp();
            logger.Info("FIN SincronizarResearch Power App");

            return Content("ok");
        }

        public ActionResult VerificarSolicitudesExtraordinariasPendientes(string fecha)
        {
            DateTime dia = DateTime.Now.Date;
            logger.Info("INICIO VerificarSolicitudesExtraordinariasPendientes");
            if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
            {
                dia = DateTime.ParseExact(fecha, "yyyyMMdd", null);
            }
            cupoManager.VerificarSolicitudesExtraordinariasPendientes(dia);
            logger.Info("FIN VerificarSolicitudesExtraordinariasPendientes");
            return Content("ok");
        }
    }
}