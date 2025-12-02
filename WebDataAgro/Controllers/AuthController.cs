using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;

namespace WebDataAgro.Controllers
{
    public class AuthController : Controller
    {
        private readonly ITokenManager tokenManager;
        private readonly IUsuarioManager usuarioManager;

        public AuthController(ITokenManager tokenManager, IUsuarioManager usuarioManager)
        {
            this.tokenManager = tokenManager;
            this.usuarioManager = usuarioManager;
        }

        public ActionResult Index(string t)
        {
            var token = tokenManager.ValidarToken(t);
            if (token != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, token.Cuit.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, token.NombreUsuario),
                };
                var identity = new ClaimsIdentity(claims, "tokenApi");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                // Set current principal

                var transformer = FederatedAuthentication.FederationConfiguration.IdentityConfiguration.ClaimsAuthenticationManager;
                if (transformer != null)
                {
                    var transformedPrincipal = transformer.Authenticate(string.Empty, claimsPrincipal);
                    HttpContext.User = transformedPrincipal;
                    Thread.CurrentPrincipal = transformedPrincipal;
                }
                if (!usuarioManager.AceptoTerminosYCondiciones(token.NombreUsuario, token.Cuit.ToString()))
                {
                    return RedirectToAction("Index", "TerminosYCondiciones");
                }
                else if (PermisosHelper.Is(PermisosDataAgro.VisualizarCompraNet))
                {
                    return RedirectToAction("Index", "CompraNet");
                }
                else if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                {
                    return RedirectToAction("Index", "Cupo");
                }

            }
            return RedirectToAction("Index", "Error");
        }

    }
}