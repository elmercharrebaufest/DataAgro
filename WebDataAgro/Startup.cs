using Hangfire;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebDataAgro.App_Start.Startup))]

namespace WebDataAgro.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Servidor Hangfire
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });

            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[]
                {
                    new AuthorizationFilter { Roles = "ConfiguracionTareasProgramadas"}
                }
            };

            app.UseHangfireDashboard("/Hangfire", dashboardOptions);

            var backgroundJobServerOptions = new BackgroundJobServerOptions
            {
                WorkerCount = 1 // Solo un worker, una tarea a la vez
            };
            app.UseHangfireServer(backgroundJobServerOptions);

            // Registrar jobs
            JobRegistration.HangfireJobRegistry.Register();
        }
    }
}