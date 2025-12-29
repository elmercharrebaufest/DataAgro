using System.Web.Mvc;

namespace WebDataAgro.Filters
{
    public class RequireLoginAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            // Si ya tiene cookie → pasa
            if (filterContext.HttpContext.User?.Identity?.IsAuthenticated == true)
                return;

            // Si viene del Login, no hacemos nada
            var path = request.Url?.AbsolutePath?.ToLower();
            if (path == "/home/login" || path == "/home/error")
                return;
            // Si Azure está enviando el "code", dejamos seguir
            var code = filterContext.HttpContext.Request.QueryString["code"];
            if (!string.IsNullOrEmpty(code))
                return;

            // Si no está autenticado → mandarlo a Login manual
            filterContext.Result = new RedirectResult("/Home/Login");
        }
    }
}