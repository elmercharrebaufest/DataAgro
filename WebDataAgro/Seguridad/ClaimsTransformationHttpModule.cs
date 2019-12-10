using System;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace WebDataAgro.Seguridad
{
    public class ClaimsTransformationHttpModule : IHttpModule
    {
        public void Dispose()
        { }

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += Context_PostAuthenticateRequest;
        }

        void Context_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var context = ((HttpApplication)sender).Context;

            // no need to call transformation if session already exists
            if ((FederatedAuthentication.SessionAuthenticationModule != null && FederatedAuthentication.SessionAuthenticationModule.ContainsSessionTokenCookie(context.Request.Cookies))||
                (context.Request.RawUrl.Contains(".svc") && context.User.Identity.IsAuthenticated))
            {
                return;
            }

            var transformer = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
            if (transformer != null)
            {
                var transformedPrincipal = transformer.Authenticate(context.Request.RawUrl, context.User as ClaimsPrincipal);

                context.User = transformedPrincipal;
                Thread.CurrentPrincipal = transformedPrincipal;
            }
        }
    }
}