using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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

            var cultureInfo = new CultureInfo("es-AR");
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            cultureInfo.DateTimeFormat.LongDatePattern = "dddd, dd 'de' MMMM 'de' yyyy";
            cultureInfo.DateTimeFormat.ShortTimePattern = "HH:mm";
            cultureInfo.DateTimeFormat.LongTimePattern = "HH:mm:ss";
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            ServicePointManager.ServerCertificateValidationCallback =
            delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true; // Ignora todos los errores de certificado
            };

            // Habilita TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }


        public void Session_OnStart()
        {
            //var comercialManager = DependencyResolver.Current.GetService<IComercialManager>();
            //var equipo = comercialManager.ListarEquipo(GlobalVariables.IdActiveDirectory);
            ////GlobalVariables.Perfil = comercialManager.ObtenerPerfilDeUsuario(GlobalVariables.IdActiveDirectory);
            //GlobalVariables.EsAdministrador = comercialManager.EsAdministrador(GlobalVariables.IdActiveDirectory);
            //GlobalVariables.EsCupera = comercialManager.EsCupera(GlobalVariables.IdActiveDirectory);
            //GlobalVariables.Equipo = equipo.Equipo;
            //GlobalVariables.EquipoReal = equipo.EquipoReal;
            //GlobalVariables.ComercialId = comercialManager.ObtenerComercialId(GlobalVariables.IdActiveDirectory);
            //GlobalVariables.CorredoresComercial = comercialManager.ListarCorredoresComercial();
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (exception is SqlException && (exception as SqlException).Number == -2)
            {
                EnviarMailTimeOut(exception as SqlException);
            }

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

        private static void EnviarMailTimeOut(SqlException filterContext)
        {
            if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            }

            SmtpClient oCliente = default(SmtpClient);
            int Condicion = 0;
            if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
            {
                oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
            }
            else
            {
                oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
            }
            if (ConfigurationManager.AppSettings["SmtpAnonimo"] != "S")
            {
                oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                    ConfigurationManager.AppSettings["CredentialPassword"]);
            }
            oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";
            MailMessage oMensaje = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]),
                Subject = (ConfigurationManager.AppSettings["AmbientePruebas"] == "1" ? "Pruebas QA" : "Producción") + " - TIME OUT ERROR DATAAGRO DB",
                Body = filterContext.Message + ":<br>" + filterContext.StackTrace,
                IsBodyHtml = true
            };
            foreach (var item in ConfigurationManager.AppSettings["EmailDASoporte"].Split(';'))
            {
                oMensaje.To.Add(item);
            }

            oCliente.Send(oMensaje);
        }

    }
}