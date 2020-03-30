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
            builder.RegisterType<DataAgroServices>().As<IDataAgroServices>().InstancePerLifetimeScope();
            builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();

            builder.RegisterType<DataAgroDbContext>().As<DbContext>().InstancePerLifetimeScope();
            builder.RegisterType<RepositorioEF>().As<IRepositorio>().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.EndsWith("Manager"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Agent"))
                   .Where(t => t.Name.EndsWith("Agent"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterModule<NLogModule>();
            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.EndsWith("Criterios"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                   .Where(t => t.Name.StartsWith("Procesador"))
                   .InstancePerLifetimeScope();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            AutofacHostFactory.Container = container;

            ValueProviderFactories.Factories.Remove(ValueProviderFactories.Factories.OfType<JsonValueProviderFactory>().FirstOrDefault());
            ValueProviderFactories.Factories.Add(new JsonNetValueProviderFactory());
        }


        public void Session_OnStart()
        {
            var comercialManager = DependencyResolver.Current.GetService<IComercialManager>();
            var equipo = comercialManager.ListarEquipo(GlobalVariables.IdActiveDirectory);
            //GlobalVariables.Perfil = comercialManager.ObtenerPerfilDeUsuario(GlobalVariables.IdActiveDirectory);
            GlobalVariables.EsAdministrador = comercialManager.EsAdministrador(GlobalVariables.IdActiveDirectory);
            GlobalVariables.EsCupera = comercialManager.EsCupera(GlobalVariables.IdActiveDirectory);
            GlobalVariables.Equipo = equipo.Equipo;
            GlobalVariables.EquipoReal = equipo.EquipoReal;
            GlobalVariables.ComercialId = comercialManager.ObtenerComercialId(GlobalVariables.IdActiveDirectory);
            GlobalVariables.CorredoresComercial = comercialManager.ListarCorredoresComercial();
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
            public static bool EsCupera
            {
                get
                {
                    return HttpContext.Current.Session["EsCupera"] as bool? ?? false;
                }
                set
                {
                    HttpContext.Current.Session["EsCupera"] = value;
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
                    return IdActiveDirectoryCompleto.Split('\\').Length > 1 ? IdActiveDirectoryCompleto.Split('\\')[1] : IdActiveDirectoryCompleto;
                }
            }

            public static string IdActiveDirectoryCompleto
            {
                get
                {
                    return HttpContext.Current.User.Identity.Name;
                    //return "molinosagro\\nunezml";
                }
            }

            public static List<int> CorredoresComercial
            {
                get
                {
                    return (List<int>)HttpContext.Current.Session["corredoresComercial"];
                }
                set
                {
                    HttpContext.Current.Session["corredoresComercial"] = value;
                }
            }
        }

    }

}
