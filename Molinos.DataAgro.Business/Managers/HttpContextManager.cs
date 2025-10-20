using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Managers
{
    public class HttpContextManager : IHttpContextManager
    {

        public HttpContextManager()
        {

        }

        public string ObtenerPathLogoMail()
        {
            try
            {
                return System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png");
            }
            catch
            {
                return "";
            }
        }
    }
}
