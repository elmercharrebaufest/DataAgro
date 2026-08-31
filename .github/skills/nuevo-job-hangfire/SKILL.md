---
name: nuevo-job-hangfire
description: 'Workflow para crear una tarea programada (job) nueva con Hangfire en WebDataAgro: clase del job, registro del cron y verificación de la tabla de habilitación. Usar cuando pidan agregar un job/tarea programada/proceso recurrente/background job.'
---

# Nuevo job de Hangfire

## Procedimiento

1. **Crear la clase del job** en `WebDataAgro/Jobs/{Nombre}HangfireJob.cs`. La interfaz `I{Nombre}HangfireJob : IHangfireJob` se define en el mismo archivo (no en `Molinos.DataAgro.Interfaces`, es la excepción a esa convención):
   ```csharp
   public interface I{Nombre}HangfireJob : IHangfireJob { }

   public class {Nombre}HangfireJob : I{Nombre}HangfireJob
   {
       private readonly ILogger logger;      // NLog.ILogger
       private readonly IRepositorio repositorio;

       public {Nombre}HangfireJob(ILogger logger, IRepositorio repositorio /*, managers necesarios */)
       {
           this.logger = logger;
           this.repositorio = repositorio;
       }

       public void Execute()
       {
           var habilitacion = repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "{Nombre}HangfireJob");
           if (habilitacion == null || !habilitacion.Habilitado) return;

           logger.Info("HANGFIRE - {Nombre}HangfireJob - Iniciando");
           try
           {
               // lógica del job
           }
           catch (Exception ex)
           {
               logger.Error(ex);
               throw;
           }
       }
   }
   ```
   No hace falta registrar la clase en Autofac: se resuelve sola (registro por convención sobre todo el assembly `WebDataAgro`).

2. **Registrar el cron** en `WebDataAgro/App_Start/RegisterHangfireJobs.cs` (`HangfireJobRegistry.Register()`):
   ```csharp
   RecurringJob.AddOrUpdate<I{Nombre}HangfireJob>("{Nombre}HangfireJob", s => s.Execute(), "cron expression", new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
   ```

3. **Verificar/crear el registro en la tabla `HabilitacionJob`.** Muchos jobs (no todos) arrancan chequeando `repositorio.Obtener<HabilitacionJob>(a => a.Nombre == "{Nombre}HangfireJob")` y si no existe o `Habilitado = false`, no hacen nada. Si el job nuevo usa ese guard, **avisar explícitamente al usuario** que hace falta un script SQL/post-deploy para insertar el registro correspondiente en `HabilitacionJob`, o el job va a quedar agendado en Hangfire pero sin ejecutar lógica.

## Fuera de alcance
- No es obligatorio usar el guard de `HabilitacionJob`; solo aplicarlo si el usuario lo pide o si jobs similares del mismo dominio ya lo usan.
- No inventar la expresión cron: confirmarla con el usuario o basarse en jobs similares existentes.
