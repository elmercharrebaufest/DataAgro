using Autofac;
using Autofac.Integration.Mvc;
using Molinos.DataAgro.Repository;
using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using WebDataAgro.Services;

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        public void RegistrarDependencias(ContainerBuilder builder)
        {
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

            // ---------- LOGGER ----------
            builder.Register((c, p) =>
            {
                // Autofac: info del componente que se está resolviendo
                var ctx = c.Resolve<IComponentContext>();
                var registration = ctx.ComponentRegistry.Registrations.LastOrDefault();

                var declaringType = registration?.Activator.LimitType
                                   ?? typeof(object); // fallback si no lo encuentra

                // Crear logger de NLog para ese tipo
                return NLog.LogManager.GetLogger(declaringType.FullName);
            }).As<NLog.ILogger>().InstancePerDependency();


            // ----------------------------------

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                 .Where(t => t.Name.EndsWith("Criterios"))
                 .AsImplementedInterfaces()
                 .InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(Assembly.Load("Molinos.DataAgro.Business"))
                 .Where(t => t.Name.StartsWith("Procesador"))
                 .InstancePerLifetimeScope();

            builder.RegisterType<Cache>().As<ICache>().SingleInstance();

            var assembliesToRegister = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a =>
                    a.FullName.StartsWith("WebDataAgro") ||
                    a.FullName.StartsWith("SustitucionMOA") ||
                    a.FullName.StartsWith("Molinos.DataAgro"))
                .ToArray();

            builder.RegisterAssemblyTypes(assembliesToRegister)
                   .AsImplementedInterfaces();
        }
    }
}