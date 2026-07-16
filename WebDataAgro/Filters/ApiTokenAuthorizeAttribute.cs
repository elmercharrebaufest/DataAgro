using System;
using System.Text;
using System.Web.Mvc;

namespace WebDataAgro.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public sealed class ApiTokenAuthorizeAttribute : AuthorizeAttribute
    {
        private const string ApiTokenHeader = "X-API-Token";

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            var expectedToken = ApiRequestSecurityHelper.GetExpectedToken();
            if (string.IsNullOrWhiteSpace(expectedToken))
            {
                return false;
            }

            var providedToken = httpContext?.Request?.Headers[ApiTokenHeader];
            if (string.IsNullOrWhiteSpace(providedToken))
            {
                return false;
            }

            return SlowEquals(expectedToken, providedToken);
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            response.StatusCode = 401;
            response.SuppressFormsAuthenticationRedirect = true;
            response.TrySkipIisCustomErrors = true;
            response.ContentType = "application/json";

            filterContext.Result = new JsonResult
            {
                Data = new
                {
                    ok = false,
                    error = "unauthorized",
                    detail = "Token invalido o ausente"
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            var bytesA = Encoding.UTF8.GetBytes(a);
            var bytesB = Encoding.UTF8.GetBytes(b);

            var diff = bytesA.Length ^ bytesB.Length;
            var length = Math.Min(bytesA.Length, bytesB.Length);
            for (int i = 0; i < length; i++)
            {
                diff |= bytesA[i] ^ bytesB[i];
            }

            return diff == 0;
        }
    }
}