using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Reflection;


using Autofac;
using Autofac.Integration.Mvc;

using Molinos.DataAgro.Interfaces;
using WebDataAgro.Core;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Autofac.Extras.NLog;

namespace WebDataAgro
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //ModelBinders.Binders.Add(typeof(DateTime), new MyDateTimeBinder());
            ModelBinders.Binders.Add(typeof(KendoGridMvcRequest), new KendoGridMvcModelBinder());

            //Autofac Configuration
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            builder.RegisterType<MSContextProvider>().As<IMSContextProvider>().InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.EndsWith("Manager"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterModule<NLogModule>();
            var container = builder.Build();
            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }


        public void Session_OnStart()
        {            
            var usuario = Util.GetUsuario();
            var perfil = Util.ObtenerPerfilDeUsuario(usuario);
            var equipo = Util.ListarEquipo(usuario);
            
            GlobalVariables.Perfil = (EnumPerfil)perfil;
            GlobalVariables.EsAdministrador = Util.EsAdministrador(usuario);
            GlobalVariables.TieneEmpleadosACargo = equipo.Count > 1;
            GlobalVariables.Equipo = equipo;
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Response.Clear();

            HttpException httpException = exception as HttpException;

            int error = httpException != null ? httpException.GetHttpCode() : 0;

            Server.ClearError();
            Response.Redirect("~/Error/");
        }


        public static class GlobalVariables
        {            
            // read-write variable
            public static EnumPerfil Perfil
            {
                get
                {
                    return (EnumPerfil)HttpContext.Current.Application["perfil"];
                }
                set
                {
                    HttpContext.Current.Application["perfil"] = value;
                }
            }

            public static bool EsAdministrador
            {
                get
                {
                    return HttpContext.Current.Application["esAdministrador"] as bool? ?? false;
                }
                set
                {
                    HttpContext.Current.Application["esAdministrador"] = value;
                }
            }

            public static bool TieneEmpleadosACargo
            {
                get
                {
                    return HttpContext.Current.Application["tieneEmpleadosACargo"] as bool? ?? false;
                }
                set
                {
                    HttpContext.Current.Application["tieneEmpleadosACargo"] = value;
                }
            }

            public static List<int> Equipo
            {
                get
                {
                    return (List<int>)HttpContext.Current.Application["equipo"];
                }
                set
                {
                    HttpContext.Current.Application["equipo"] = value;
                }
            }
        }

    }

}
