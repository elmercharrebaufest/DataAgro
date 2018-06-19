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
            ModelBinders.Binders.Add(typeof(KendoGridMvcRequest), new KendoGridMvcModelBinder());

            //Autofac Configuration
            var builder = new Autofac.ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();

            builder.RegisterType<MSContextProvider>().As<IMSContextProvider>().InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.EndsWith("Manager"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            var container = builder.Build();
            
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }


        public void Session_OnStart()
        {            
            var usuario = Util.GetUsuario();
            bool esPerfilAdministrativo = Util.EsPerfilAdministrativo(usuario);
            bool esPerfilVisualizador = Util.EsPerfilVisualizador(usuario);
            bool esAdministrador = Util.EsAdministrador(usuario);

            GlobalVariables.EsPerfilAdministrativo = esPerfilAdministrativo.ToString();
            GlobalVariables.EsPerfilVisualizador = esPerfilVisualizador.ToString();
            GlobalVariables.EsAdministrador = esAdministrador.ToString();
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
            public static string EsPerfilAdministrativo
            {
                get
                {
                    return HttpContext.Current.Application["esPerfilAdministrativo"] as string;
                }
                set
                {
                    HttpContext.Current.Application["esPerfilAdministrativo"] = value;
                }
            }

            public static string EsPerfilVisualizador
            {
                get
                {
                    return HttpContext.Current.Application["esPerfilVisualizador"] as string;
                }
                set
                {
                    HttpContext.Current.Application["esPerfilVisualizador"] = value;
                }
            }

            public static string EsAdministrador
            {
                get
                {
                    return HttpContext.Current.Application["esAdministrador"] as string;
                }
                set
                {
                    HttpContext.Current.Application["esAdministrador"] = value;
                }
            }

        }

    }

}
