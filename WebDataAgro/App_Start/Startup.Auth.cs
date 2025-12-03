using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;
using System.Net;

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {

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
