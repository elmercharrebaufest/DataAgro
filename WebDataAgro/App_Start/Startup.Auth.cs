//using Microsoft.Owin.Security;
//using Microsoft.Owin.Security.Cookies;
//using Owin;
//using System;
//using System.Net;

//namespace WebDataAgro.App_Start
//{
//    public partial class Startup
//    {
//        private static readonly string ClientId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientId"];
//        private static readonly string TenantId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_TenantId"];
//        private static readonly string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientSecret"];
//        private static readonly string RedirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_RedirectUri"];
//        private static readonly string PostLogoutUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_PostLogoutUri"];

//        private static readonly string AuthorityBase = $"https://login.microsoftonline.com/{TenantId}/v2.0";

//        public void ConfigureAuth(IAppBuilder app)
//        {
//            //app.UseAutofacMiddleware(_container);

//            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

//            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

//            app.UseCookieAuthentication(new CookieAuthenticationOptions
//            {
//                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
//                CookieName = ".DataAgro.Auth",
//                CookieSecure = CookieSecureOption.Never,  // porque usás http
//                CookieSameSite = Microsoft.Owin.SameSiteMode.Lax,
//                ExpireTimeSpan = TimeSpan.FromHours(8),
//                SlidingExpiration = false
//            });

//        }
//    }
//}
