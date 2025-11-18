using Autofac;
using Autofac.Integration.Owin;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Host.SystemWeb;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Microsoft.Owin.Security.OpenIdConnect;
using Molinos.DataAgro.Interfaces;
using Owin;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebDataAgro.Helpers;

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        // Leer desde web.config
        private static readonly string ClientId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientId"];
        private static readonly string TenantId = System.Configuration.ConfigurationManager.AppSettings["AzureAd_TenantId"];
        private static readonly string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["AzureAd_ClientSecret"];
        private static readonly string RedirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_RedirectUri"];
        private static readonly string PostLogoutUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_PostLogoutUri"];
        private static readonly string Authority = $"https://login.microsoftonline.com/{TenantId}/v2.0";
        private static readonly string[] Scopes = new string[] { };

        public void ConfigureAuth(IAppBuilder app)
        {
            app.UseAutofacMiddleware(_container);
            // Required for Azure webapps, as by default they force TLS 1.2 and this project attempts 1.0
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            //app.UseCookieAuthentication(new CookieAuthenticationOptions
            //{
            //    CookieManager = new SystemWebChunkingCookieManager(),

            //    //AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
            //    //CookieName = ".DataAgro.Auth",
            //    //ExpireTimeSpan = TimeSpan.FromHours(8),
            //    //SlidingExpiration = true,
            //    //CookieSecure = CookieSecureOption.Never,
            //    //CookieSameSite = SameSiteMode.Lax
            //    AuthenticationType = "DataAgro.Auth",
            //    CookieName = ".DataAgro.Auth",
            //    CookieSecure = CookieSecureOption.SameAsRequest, // acepta HTTP
            //    CookieSameSite = Microsoft.Owin.SameSiteMode.None, // para que funcione cross-site si hay redirecciones
            //    ExpireTimeSpan = TimeSpan.FromHours(8)
            //});

            //app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            //{
            //    CookieManager = new SystemWebCookieManager(),
            //    ClientId = clientId,
            //    Authority = authority,
            //    RedirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_RedirectUri"],
            //    PostLogoutRedirectUri = System.Configuration.ConfigurationManager.AppSettings["AzureAd_PostLogoutUri"],
            //    ResponseType = "code id_token",
            //    Scope = "openid profile email",

            //    Notifications = new OpenIdConnectAuthenticationNotifications
            //    {
            //        RedirectToIdentityProvider = OnRedirectToIdentityProvider,
            //        AuthorizationCodeReceived = OnAuthorizationCodeReceived,
            //        AuthenticationFailed = OnAuthenticationFailed,
            //        SecurityTokenValidated = OnSecurityTokenValidated
            //    }
            //});



            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                // ASP.NET web host compatible cookie manager
                CookieManager = new SystemWebChunkingCookieManager(),
                ExpireTimeSpan = TimeSpan.FromMinutes(480),
                SlidingExpiration = true // Esto asegura que las cookies se renueven con actividad del usuario.
            });

            var options = new OpenIdConnectAuthenticationOptions
            {
                // Generate the metadata address using the tenant and policy information
                MetadataAddress = $"{Authority}/.well-known/openid-configuration",


                // These are standard OpenID Connect parameters, with values pulled from web.config
                ClientId = ClientId,
                RedirectUri = RedirectUri,
                PostLogoutRedirectUri = RedirectUri,

                // Specify the callbacks for each type of notifications
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    AuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    AuthenticationFailed = OnAuthenticationFailed,
                    SecurityTokenValidated = OnSecurityTokenValidated

                },

                // Specify the claim type that specifies the Name property.
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    ValidateIssuer = false
                },

                // ASP.NET web host compatible cookie manager
                CookieManager = new SystemWebCookieManager(),

                // Specify the scope by appending all of the scopes requested into one string (separated by a blank space)
                Scope = $"openid profile offline_access",

                //UseTokenLifetime = false,
            };
            options.ProtocolValidator.RequireNonce = System.Configuration.ConfigurationManager.AppSettings["AmbienteLocal"].ToString() != "1";
            options.ProtocolValidator.RequireState = System.Configuration.ConfigurationManager.AppSettings["AmbienteLocal"].ToString() != "1";
            app.UseOpenIdConnectAuthentication(
                options
            );
        }

        //Agrego esta función del callback. Ya que esta es llamada desde el registro y desde el login. 
        private Task OnSecurityTokenValidated(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            LogStep("OnSecurityTokenValidated", notification.OwinContext);

            var identity = notification.AuthenticationTicket.Identity;

            var email = identity.FindFirst(ClaimTypes.Upn)?.Value
                       ?? identity.FindFirst("preferred_username")?.Value;

            notification.AuthenticationTicket = new AuthenticationTicket(
                identity,
                notification.AuthenticationTicket.Properties
            );

            //Resolver IComercialManager desde Autofac
            var lifetimeScope = notification.OwinContext.GetAutofacLifetimeScope();
            var comercialManager = lifetimeScope.Resolve<IComercialManager>();
            //var comercialManager = DependencyResolver.Current.GetService<IComercialManager>();

            // Obtener permisos desde tu BD local
            var permisos = comercialManager.ObtenerPermisosPorEmail(email);

            // Agregar los permisos como claims de rol
            foreach (var permiso in permisos)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, permiso));
            }
            return Task.FromResult(0);
        }

        private Task OnRedirectToIdentityProvider(RedirectToIdentityProviderNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            LogStep("OnRedirectToIdentityProvider", notification.OwinContext);

            var cookies = notification.OwinContext.Response.Cookies;

            return Task.FromResult(0);
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            LogStep("OnAuthenticationFailed", notification.OwinContext);

            notification.HandleResponse();
            // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page
            // because password reset is not supported by a "sign-up or sign-in policy"
            if (notification.ProtocolMessage.ErrorDescription != null && notification.ProtocolMessage.ErrorDescription.Contains("AADB2C90118"))
            {
                // If the user clicked the reset password link, redirect to the reset password route
                notification.Response.Redirect("/ResetPassword");
            }
            else if (notification.Exception.Message == "access_denied")
            {
                //Log.AzureError(notification.Exception);
                notification.Response.Redirect("/");
            }
            else
            {
                // Log.AzureError(notification.Exception);
                notification.Response.Redirect("/");
            }

            return Task.FromResult(0);
        }

        private async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedNotification notification)
        {
            LogStep("OnAuthorizationCodeReceived", notification.OwinContext);

            IConfidentialClientApplication confidentialClient = MsalAppBuilder.BuildConfidentialClientApplication(new ClaimsPrincipal(notification.AuthenticationTicket.Identity));

            // Upon successful sign in, get & cache a token using MSAL
            AuthenticationResult result = await confidentialClient.AcquireTokenByAuthorizationCode(Scopes, notification.Code).ExecuteAsync();
            var code = notification.Code;

            //var cca = ConfidentialClientApplicationBuilder
            //            .Create(ClientId)
            //            .WithClientSecret(ClientSecret)
            //            .WithAuthority(Authority)
            //            .Build();

            //try
            //{
            //    await cca.AcquireTokenByAuthorizationCode(
            //        new[] { "openid", "profile", "email" }, code
            //    ).ExecuteAsync();
            //}
            //catch { }
        }


        private void LogStep(string step, IOwinContext context, string extra = null)
        {
            try
            {
                var logPath = System.Web.Hosting.HostingEnvironment.MapPath("/AuthDebug.log");
                using (var writer = new System.IO.StreamWriter(logPath, true))
                {
                    writer.WriteLine($"--- {DateTime.Now:yyyy-MM-dd HH:mm:ss} | {step} ---");
                    writer.WriteLine($"Request URL: {context.Request.Uri}");
                    foreach (var key in context.Request.Cookies)
                    {
                        writer.WriteLine($"Cookie: {key.Key} = {key.Value}");
                    }
                    if (!string.IsNullOrEmpty(extra))
                    {
                        writer.WriteLine($"Extra: {extra}");
                    }
                    writer.WriteLine();
                }
            }
            catch
            {
                // No queremos que falle el logging el flujo de login
            }
        }


    }
}