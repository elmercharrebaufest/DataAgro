using Autofac;
using Autofac.Extras.NLog;
using Autofac.Integration.Mvc;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebDataAgro.Core;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Interfaces;
using System.Data.Entity;
using WebDataAgro.Services;
using Autofac.Integration.Wcf;

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
            var builder = new ContainerBuilder();
            
            builder.RegisterControllers(typeof(MvcApplication).Assembly).PropertiesAutowired();
            
            builder.RegisterType<DataAgroDbContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<RepositorioEF>().As<IRepositorio>().InstancePerRequest();

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.EndsWith("Manager"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterModule<NLogModule>();
            builder.RegisterType<DataAgroServices>().As<IDataAgroServices>().InstancePerRequest();
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AutofacHostFactory.Container = container;
        }


        public void Session_OnStart()
        {
            var comercialManager = DependencyResolver.Current.GetService<IComercialManager>();
            var equipo = comercialManager.ListarEquipo(GlobalVariables.IdActiveDirectory);
            GlobalVariables.Perfil = comercialManager.ObtenerPerfilDeUsuario(GlobalVariables.IdActiveDirectory);
            GlobalVariables.EsAdministrador = comercialManager.EsAdministrador(GlobalVariables.IdActiveDirectory);
            GlobalVariables.Equipo = equipo.Equipo;
            GlobalVariables.EquipoReal = equipo.EquipoReal;
            GlobalVariables.ComercialId = comercialManager.ObtenerComercialId(GlobalVariables.IdActiveDirectory);
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            DependencyResolver.Current.GetService<ILogger>().Error("Application_Error: ", exception);
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
                    return (EnumPerfil)HttpContext.Current.Session["perfil"];
                }
                set
                {
                    HttpContext.Current.Session["perfil"] = value;
                }
            }

            public static bool EsAdministrador
            {
                get
                {
                    return HttpContext.Current.Session["esAdministrador"] as bool? ?? false;
                }
                set
                {
                    HttpContext.Current.Session["esAdministrador"] = value;
                }
            }

            public static int ComercialId
            {
                get
                {
                    return (int)HttpContext.Current.Session["comercialId"];
                }
                set
                {
                    HttpContext.Current.Session["comercialId"] = value;
                }
            }

            public static bool TieneEmpleadosACargo
            {
                get
                {
                    return Equipo.Count > 0;
                }
            }

            public static List<int> Equipo
            {
                get
                {
                    return (List<int>)HttpContext.Current.Session["equipo"];
                }
                set
                {
                    HttpContext.Current.Session["equipo"] = value;
                }
            }

            public static List<int> EquipoReal
            {
                get
                {
                    return (List<int>)HttpContext.Current.Session["equipoReal"];
                }
                set
                {
                    HttpContext.Current.Session["equipoReal"] = value;
                }
            }

            public static string IdActiveDirectory
            {
                get
                {
                    return IdActiveDirectoryCompleto.Split('\\')[1];
                }
            }

            public static string IdActiveDirectoryCompleto
            {
                get
                {
                    return HttpContext.Current.User.Identity.Name;
                }
            }
        }

    }

}
