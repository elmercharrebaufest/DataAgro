using System.Linq;
using System.Security.Claims;

namespace Molinos.DataAgro.Entities.Seguridad
{
    public class PermisosHelper
    {
        public static bool Is(params PermisosDataAgro[] permisos)
        {
            var permisosUsuario = ClaimsPrincipal.Current.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value);
            return permisos.Any(p => permisosUsuario.Contains(p.ToString()));
        }
    }
}