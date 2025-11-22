using Autofac.Integration.Mvc;
using Autofac.Integration.Wcf;
using Owin;
using System.Web.Mvc;

namespace WebDataAgro.App_Start
{
    public partial class Startup
    {
        public void ConfigureMvcAutofac(IAppBuilder app)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
            AutofacHostFactory.Container = _container;
        }
    }
}