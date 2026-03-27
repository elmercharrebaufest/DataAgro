using Hangfire;
using System;
using WebDataAgro.Jobs;

namespace WebDataAgro.JobRegistration
{
    public static class HangfireJobRegistry
    {
        public static void Register()
        {
            RecurringJob.AddOrUpdate<IActualizarCumplimientoCuposHangfireJob>("ActualizarCumplimientoCuposHangfireJob", s => s.Execute(), "0 5 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarEstadoDeContratosHangfireJob>("ActualizarEstadoDeContratosHangfireJob", s => s.Execute(), "0 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarFechaUltimaActualizacionManualesFAQHangfireJob>("ActualizarFechaUltimaActualizacionManualesFAQHangfireJob", s => s.Execute(), "30 9 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarMailProveedorHangfireJob>("ActualizarMailProveedorHangfireJob", s => s.Execute(), "0 6 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarPrecioPizarraHangfireJob>("ActualizarPrecioPizarraHangfireJob", s => s.Execute(), "*/10 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarProveedoresHomeHangfireJob>("ActualizarProveedoresHomeHangfireJob", s => s.Execute(), "0 2 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarRazonSocialHangfireJob>("ActualizarRazonSocialHangfireJob", s => s.Execute(), "30 8 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IAnularAcuerdosHangfireJob>("AnularAcuerdosHangfireJob", s => s.Execute(), "50 22 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<IBorradoContratosHangfireJob>("BorradoContratosHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<ICerrarDiaHangfireJob>("CerrarDiaHangfireJob", s => s.Execute(), "15 18 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IConfirmacionAutomaticaPizarra13HrsHangfireJob>("ConfirmacionAutomaticaPizarra13HrsHangfireJob", s => s.Execute(), "0 13 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IConsultarCuposDiariosHangfireJob>("ConsultarCuposDiariosHangfireJob", s => s.Execute(), "*/10 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IConsultarMisturnosActivosHangfireJob>("ConsultarMisturnosActivosHangfireJob", s => s.Execute(), "10 * * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<ICrearSugerenciaCupoHangfireJob>("CrearSugerenciaCupoHangfireJob", s => s.Execute(), "0 6 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<IEnviarMailConfirmaHangfireJob>("EnviarMailConfirmaHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<IEnviarMailSugerenciasPendientesPorComercialHangfireJob>("EnviarMailSugerenciasPendientesPorComercialHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IEnvioMailNegociosAnulaYReemplazaHangfireJob>("EnvioMailNegociosAnulaYReemplazaHangfireJob", s => s.Execute(), "50 23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IEnvioMailNegociosConDiaAnteriorHangfireJob>("EnvioMailNegociosConDiaAnteriorHangfireJob", s => s.Execute(), "02 23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IEnvioMailPendientesHangfireJob>("EnvioMailPendientesHangfireJob", s => s.Execute(), "0 9 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IEnvioMailSinCTGHangfireJob>("EnvioMailSinCTGHangfireJob", s => s.Execute(), "0 10,15 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IFinalizacionContratosHangfireJob>("FinalizacionContratosHangfireJob", s => s.Execute(), "0 20,23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IGrabarDatosReporteCompraNetHangfireJob>("GrabarDatosReporteCompraNetHangfireJob", s => s.Execute(), "10 23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IMigrarContratosPrimaryHangfireJob>("MigrarContratosPrimaryHangfireJob", s => s.Execute(), "30 18,23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IPesificadosHangfireJob>("PesificadosHangfireJob", s => s.Execute(), "0 */2 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IRechazarSolicitudesVencidasHangfireJob>("RechazarSolicitudesVencidasHangfireJob", s => s.Execute(), "30 12 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<IReportePagosDiferidosHangfireJob>("ReportePagosDiferidosHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<ISincronizarResearchHangfireJob>("SincronizarResearchHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<ITransmitirCupoStopHangfireJob>("TransmitirCupoStopHangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IVerificarSolicitudesExtraordinariasPendientesHangfireJob>("VerificarSolicitudesExtraordinariasPendientesHangfireJob", s => s.Execute(), "10 18 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

            RecurringJob.AddOrUpdate<IProcessComprasAyerHangfireJob>("ProcessComprasAyerHangfireJob", s => s.Execute(), "15 20 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            //RecurringJob.AddOrUpdate<IProcessRg2300HangfireJob>("ProcessRg2300HangfireJob", s => s.Execute(), Cron.Daily, new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IProcessSisaHangfireJob>("ProcessSisaHangfireJob", s => s.Execute(), "0 10 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IProcessFacacopHangfireJob>("ProcessFacacopHangfireJob", s => s.Execute(), "0 9 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IProcessEstadoHangfireJob>("ProcessEstadoHangfireJob", s => s.Execute(), "30 20 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IProcessComprasHangfireJob>("ProcessComprasHangfireJob", s => s.Execute(), "15 23 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IProcessCapacidadProductivaHangfireJob>("ProcessCapacidadProductivaHangfireJob", s => s.Execute(), "0 7,12,16 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IEliminarLogsAntiguosHangfireJob>("EliminarLogsAntiguosHangfireJob", s => s.Execute(), "0 8 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
            RecurringJob.AddOrUpdate<IActualizarScoringCuposDeProveedoresHangfireJob>("ActualizarScoringCuposDeProveedoresHangfireJob", s => s.Execute(), "0 1 * * *", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

            RecurringJob.RemoveIfExists("JobDePrueba");
        }
    }
}
