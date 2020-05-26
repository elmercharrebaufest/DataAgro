using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebGrease.Css.Extensions;

namespace WebDataAgro.Seguridad
{
    public class DataAgroClaimsAuthenticationManager : ClaimsAuthenticationManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger log;

        private DataAgroClaimsAuthenticationManager(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            log = LogManager.GetLogger("Global");
        }

        public DataAgroClaimsAuthenticationManager()
            : this(DependencyResolver.Current.GetService<IRepositorio>())
        {
        }

        public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
        {
            if (incomingPrincipal == null || !incomingPrincipal.Identity.IsAuthenticated)
            {
                return incomingPrincipal;
            }
            log.Debug("Recibida autenticación de usuario");
            var identity = ((ClaimsIdentity)incomingPrincipal.Identity);
            var claim = identity.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Name);
            if (claim == null)
            {
                var sb = new StringBuilder();
                identity.Claims.ForEach(x => sb.Append(x.Type + ": " + x.Value + "\n"));
                log.Error("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb);
                throw new SecurityException(string.Format("No se encontró el nombre de usuario en los claims recibidos. No se puede continuar con la autenticación. Claims recibidos: {0}", sb));
            }
            log.Debug("Claim Value: {0}", claim.Value);
            var nombrearr = claim.Value.Split('\\');
            var nombreUsuario = string.Empty;
            bool esExterno = false;
            if (nombrearr.Length > 1)
            {
                nombreUsuario = claim.Value.Split('\\')[1];
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nombreUsuario));
            }
            else
            {
                nombreUsuario = claim.Value;
                esExterno = true;
            }

            log.Debug("Agregando claims de permisos DataAgro para el usuario {0}", nombreUsuario);
            if (esExterno)
            {
                var usuario = repositorio.ObtenerNoTracking<Proveedor>(u => u.CUIT == nombreUsuario);
                if (usuario != null)
                {
                    foreach (var permiso in usuario.RolesAsociados.SelectMany(rol => rol.PermisosAsociados).Distinct())
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, permiso.Permiso.ToString()));
                    }
                }
            }
            else
            {
                var usuario = repositorio.ObtenerNoTracking<Comercial>(u => u.IdActiveDirectory == nombreUsuario);
                if (usuario != null && usuario.Deshabilitado != true)
                {
                    foreach (var permiso in usuario.RolesAsociados.SelectMany(rol => rol.PermisosAsociados).Distinct())
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, permiso.Permiso.ToString()));
                    }
                }
            }

            var ci = new ClaimsIdentity(((ClaimsIdentity)incomingPrincipal.Identity).Claims, "Negotiate");
            var transformedPrincipal = new ClaimsPrincipal(ci);
            CreateSession(transformedPrincipal);
            return transformedPrincipal;
        }

        private void CreateSession(ClaimsPrincipal transformedPrincipal)
        {
            var sessionSecurityToken = new SessionSecurityToken(transformedPrincipal, TimeSpan.FromHours(8));
            FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionSecurityToken);
        }
    }
}