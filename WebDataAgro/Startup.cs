using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Net;
using System.Web.Mvc;

[assembly: OwinStartup(typeof(WebDataAgro.App_Start.Startup))]

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        private IContainer _container;

        public void Configuration(IAppBuilder app)
        {
            // 1️⃣ Configurar Autofac solo para MVC/Hangfire
            var builder = new ContainerBuilder();
            RegistrarDependencias(builder);
            _container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            AutofacHostFactory.Container = _container;

            // 2️⃣ Configurar OWIN solo para cookies
            ConfigureAuth(app);

            // 3️⃣ Si usas Hangfire
            ConfigureHangfire(app);
        }

        public void ConfigureAuth(IAppBuilder app)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                CookieName = ".DataAgro.Auth",
                CookieSecure = CookieSecureOption.Never,
                CookieSameSite = Microsoft.Owin.SameSiteMode.Lax,
                ExpireTimeSpan = TimeSpan.FromHours(8),
                SlidingExpiration = false
            });
        }
    }
}
