using Autofac;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebDataAgro.App_Start.Startup))]

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        private IContainer _container;

        public void Configuration(IAppBuilder app)
        {
            // 1️ Configurar Autofac solo para MVC/Hangfire
            var builder = new ContainerBuilder();
            RegistrarDependencias(builder);
            _container = builder.Build();
            ConfigureMvcAutofac(app);

            // 2️ Configurar OWIN solo para cookies
            ConfigureAuth(app);

            // 3️ Configurar Hangfire
            ConfigureHangfire(app);
        }
    }
}
