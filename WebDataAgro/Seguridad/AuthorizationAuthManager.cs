using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Repository;
using System;
using System.Linq;
using System.ServiceModel;
using System.Web.Mvc;

namespace WebDataAgro.Seguridad 
{ 
    public class AuthorizationAuthManager : ServiceAuthorizationManager
    {
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            var repositorio = DependencyResolver.Current.GetService<IRepositorio>();
            var log = DependencyResolver.Current.GetService<ILogger>();

            string usuarioDominio = null;
            var autorizado = false;
            try
            {
                usuarioDominio = operationContext.ServiceSecurityContext.WindowsIdentity.Name;
                if (usuarioDominio != null && usuarioDominio.Contains("\\"))
                {
                    var nombreUsuario = usuarioDominio.Split('\\')[1];

                    autorizado = repositorio.Existe<Comercial>(u => u.IdActiveDirectory == nombreUsuario && u.RolesAsociados.Any(x => x.PermisosAsociados.Any(y => y.Permiso == PermisosDataAgro.ServicioAuth)));

                }
            }
            catch (Exception e)
            {
                log.Error($"No se pudo authorizar al usuario: {usuarioDominio}", e);
            }
            return autorizado;
        }

    }
}
