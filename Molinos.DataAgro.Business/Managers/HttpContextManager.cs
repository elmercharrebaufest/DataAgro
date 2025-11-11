using Molinos.DataAgro.Interfaces;
using System;
using System.IO;

namespace Molinos.DataAgro.Business.Managers
{
    public class HttpContextManager : IHttpContextManager
    {

        public HttpContextManager()
        {

        }

        //public string ObtenerPathLogoMail()
        //{
        //    try
        //    {
        //        return System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png");
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        public string ObtenerPathLogoMail()
        {
            string path = "";

            try
            {
                if (System.Web.HttpContext.Current != null)
                {
                    path = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png");
                }
                else
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    path = Path.Combine(baseDir, "Content", "Images", "MolinosAgro.png");
                }
            }
            catch { }

            return File.Exists(path) ? path : "";
        }
    }
}
