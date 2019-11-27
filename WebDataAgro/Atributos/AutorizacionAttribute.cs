using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Authentication;
using System.Web.Mvc;
using Molinos.DataAgro.Entities.Seguridad;

namespace WebDataAgro.Atributos
{
    public sealed class AutorizacionAttribute : AuthorizeAttribute
    {
        public AutorizacionAttribute(params PermisosDataAgro[] permisos)
        {
            base.Roles = string.Join(", ", permisos);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations"), Obsolete("Usar el constructor con la lista de PermisosDataAgro.", true)]
        public new string Roles
        {
            get { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosDataAgro."); }
            set { throw new AuthenticationException("No usar esta propiedad. Usar el constructor con la lista de PermisosDataAgro."); }
        }
    }
}