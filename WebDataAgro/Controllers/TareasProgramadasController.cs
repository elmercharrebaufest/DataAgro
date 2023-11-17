using Autofac.Extras.NLog;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;
using System;
using WebDataAgro.Helpers.Excel;
using Molinos.DataAgro.Interfaces.Managers;
using System.Security.Cryptography;

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
            var diferencial = diferencialManager.TraerDiferencial();
            oHedgeManager.JobCerrarDia(GlobalVariables.ComercialId, GlobalVariables.IdActiveDirectory, mailEnviar, diferencial == null ? 0 : diferencial.DiferencialDefault);
            logger.Info($"CerrarDiaHedge - Finalizado");
            return Content("ok");
        }

        static readonly object _lockPesificados = new object();

        public ActionResult Pesificados()
        {
            lock (_lockPesificados)
            {
                logger.Info($"Pesificados - Iniciando " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                reportesManager.GrabarTodoDatoPesificar();
                logger.Info($"Pesificados - Finalizado " + DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"));
                return Content("ok");
            }

        }

        public ActionResult MigrarContratosPrimary(string fecha)
        {
            DateTime dia = DateTime.Now.Date;
            logger.Info($"MigrarContratosPrimary - Inicio");
            if (!string.IsNullOrEmpty(fecha) && fecha.Length == 8)
            {
                dia = DateTime.ParseExact(fecha, "yyyyMMdd", null);
            }
            negocioManager.MigrarContratosPrimary(dia);
            logger.Info($"MigrarContratosPrimary - Finalizado");
            return Content("ok");
        }

        public ActionResult ActualizarRazonSocial()
        {
            logger.Info("ActualizarRazonSocial - Iniciando");
            try
            {
                proveedorManager.ActualizarRazonSocial();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            logger.Info("ActualizarRazonSocial - Finalizado");
            return Content("ok");
        }

        public ActionResult ReportePagosDiferidos(string fecha)
        {
            logger.Info("ReportePagosDiferidos - Iniciando");
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
                logger.Info("ReportePagosDiferidos - Error");
                logger.Error(ex);
            }
            logger.Info("ReportePagosDiferidos - Finalizado");
            return Content("ok");
        }

        public ActionResult ActualizarCumplimientoCupos()
        {
            logger.Info("ActualizarCumplimientoCupos - Iniciando");
            try
            {
                cupoManager.ActualizarCumplimientoCupos(DateTime.Now.Date.AddDays(-1));
            }
            catch (Exception ex)
            {
                logger.Info("ActualizarCumplimientoCupos - Error");
                logger.Error(ex);
            }
            logger.Info("ActualizarCumplimientoCupos - Finalizado");
            return Content("ok");
        }

        public ActionResult ActualizarCumplimientoCuposMasivo(string fechaDesde, string fechaHasta)
        {
            logger.Info("ActualizarCumplimientoCuposMasivo - Iniciando");
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
            logger.Info("ActualizarCumplimientoCuposMasivo - Finalizado");
            return Content("ok");
        }

        public ActionResult EnvioMailNegociosAnulaYReemplaza()
        {
            logger.Info($"EnvioMailNegociosAnulaYReemplaza - Iniciando");
            negocioManager.EnvioMailNegociosAnulaYReemplaza();
            logger.Info($"EnvioMailNegociosAnulaYReemplaza - Finalizado");
            return Content("ok");
        }

        public ActionResult EnviarMailSugerenciasPendientesPorComercial()
        {
            logger.Info($"EnviarMailSugerenciasPendientesPorComercial - inicio");
            cupoManager.EnviarMailSugerenciasPendientesPorComercial();
            logger.Info($"EnviarMailSugerenciasPendientesPorComercial - Finalizado");
            return Content("ok");
        }

        public ActionResult ActualizarMailProveedor()
        {
            logger.Info($"EnviarMailSugerenciasPendientesPorComercial - inicio");
            proveedorManager.GrabarMailProveedor();
            logger.Info($"EnviarMailSugerenciasPendientesPorComercial - Finalizado");
            return Content("ok");
        }

        public ActionResult ConsultarMisturnosActivos()
        {
            if (DateTime.Now >= DateTime.Now.Date.AddHours(7) && DateTime.Now <= DateTime.Now.Date.AddHours(21))
            {
                logger.Info($"Actualizar CupoNoPropio - MisTurnosActivos");
                cupoManager.ConsultarMisTurnosActivos();
                logger.Info($"Actualizar CupoNoPropio - MisTurnosActivos - Finalizado");
            }
            else
            {
                logger.Info($"Actualizar CupoNoPropio - MisTurnosActivos - fuera de rango");
            }
            return Content("ok");
        }

        public ActionResult ActualizarPrecioPizarra()
        {
            if (DateTime.Now > DateTime.Now.Date.AddHours(10) && DateTime.Now < DateTime.Now.Date.AddHours(13).AddMinutes(1))
            {
                logger.Info($"Actualizar Precios Pizarra");
                precioPizarraManager.ActualizarPrecioPizarra(DateTime.Now.Date.AddDays(-1), false);
                logger.Info($"Actualizar Precios Pizarra - Finalizado");
            }
            return Content("ok");
        }

        public ActionResult ActualizarProveedoresHome()
        {
            //if (DateTime.Now > DateTime.Now.Date.AddHours(10) && DateTime.Now < DateTime.Now.Date.AddHours(11).AddMinutes(1))
            //{
            logger.Info($"Actualizar Proveedores Home");
            proveedorManager.ActualizarProveedoresHome();
            logger.Info($"Actualizar Proveedores Home - Finalizado");
            //}
            return Content("ok");
        }

        public ActionResult ActualizarEstadoDeContratos()
        {
            logger.Info($"Inicio Actualizar EstadoContrato");
            contratoManager.ActualizarEstadoDeContratos();
            logger.Info($"Fin Actualizar EstadoContrato");
            return Content("ok");
        }

        public ActionResult ConfirmacionAutomaticaPizarra13Hrs()
        {
            logger.Info($"Inicio Confirmación Automatica Pizarra 13hrs");
            fijacionManager.ConfirmacionAutomaticaPizarra13Hrs();
            logger.Info($"Fin Confirmación Automatica Pizarra 13hrs");
            return Content("ok");
        }

        public ActionResult ActualizarFechaUltimaActualizacionManualesFAQ()
        {
            logger.Info($"Actualizar fecha última actualización Manuales FAQ");
            faqManager.ActualizarFechaUltimaActualizacionManualesFAQ();
            logger.Info($"Actualizar fecha última actualización Manuales FAQ - Finalizado");
            
            return Content("ok");
        }

        public ActionResult SincronizarResearch()
        {
            logger.Info($"Sincronizar Research Power App");
            researchManager.SincronizarResearchPowerApp();
            logger.Info($"Sincronizar Research Power App");

            return Content("ok");
        }
    }
}