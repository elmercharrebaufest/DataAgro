using Hangfire;
using Hangfire.Dashboard;
using Owin;

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        public void ConfigureHangfire(IAppBuilder app)
        {
            // Servidor Hangfire
            GlobalConfiguration.Configuration
                .UseSqlServerStorage("HfContexto")
                .UseAutofacActivator(_container);

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 1 });

            var dashboardOptions = new DashboardOptions
            {
                Authorization = new[]
                {
                    new AuthorizationFilter { Roles = "ConfiguracionTareasProgramadas" }
                }
            };

            app.UseHangfireDashboard("/Hangfire", dashboardOptions);

            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                WorkerCount = 1
            });

            JobRegistration.HangfireJobRegistry.Register();
        }
    }
}
