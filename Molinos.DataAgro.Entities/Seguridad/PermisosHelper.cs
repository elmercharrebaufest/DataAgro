using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Molinos.DataAgro.Entities.Seguridad
{
    public class PermisosHelper
    {
        public static bool Is(params PermisosDataAgro[] permisos)
        {
            var permisosUsuario = ClaimsPrincipal.Current.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            return permisos.Any(p => permisosUsuario.Contains(p.ToString()));
        }
        public static string ObtenerCuit()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();
        }
        public static string ObtenerUsuario()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
        }
    }
}