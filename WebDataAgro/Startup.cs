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
            var builder = new ContainerBuilder();

            RegistrarDependencias(builder);

            _container = builder.Build();

            ConfigureAuth(app);
            ConfigureMvcAutofac(app);
            ConfigureHangfire(app);

        }
    }
}
