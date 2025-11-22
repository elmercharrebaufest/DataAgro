using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Claims;
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
            private static ClaimsIdentity Identity
            {
                get
                {
                    var principal = HttpContext.Current.User as ClaimsPrincipal;
                    return principal?.Identity as ClaimsIdentity;
                }
            }

            // -------------------
            // Helpers
            // -------------------
            private static T GetClaimValue<T>(string claimType, T defaultValue = default(T))
            {
                var claim = Identity?.FindFirst(claimType);
                if (claim == null) return defaultValue;

                try
                {
                    return JsonConvert.DeserializeObject<T>(claim.Value);
                }
                catch
                {
                    return defaultValue;
                }
            }

            private static void SetClaimValue<T>(string claimType, T value)
            {
                if (Identity == null) return;

                // Borro si ya existe
                var existing = Identity.FindFirst(claimType);
                if (existing != null)
                    Identity.RemoveClaim(existing);

                // Guardo serializado
                var json = JsonConvert.SerializeObject(value);
                Identity.AddClaim(new Claim(claimType, json));
            }


            // -------------------------
            // Variables reemplazadas
            // -------------------------

            public static EnumPerfil Perfil
            {
                get
                {
                    return GetClaimValue("perfil", EnumPerfil.Visualizador);
                }
                set
                {
                    SetClaimValue("perfil", value);
                }
            }

            public static bool EsAdministrador
            {
                get
                {
                    return GetClaimValue("esAdministrador", false);
                }
                set
                {
                    SetClaimValue("esAdministrador", value);
                }
            }

            public static bool EsCupera
            {
                get
                {
                    return GetClaimValue("EsCupera", false);
                }
                set
                {
                    SetClaimValue("EsCupera", value);
                }
            }

            public static int ComercialId
            {
                get
                {
                    return GetClaimValue("comercialId", 0);
                }
                set
                {
                    SetClaimValue("comercialId", value);
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
                    return GetClaimValue("equipo", new List<int>());
                }
                set
                {
                    SetClaimValue("equipo", value);
                }
            }

            public static List<int> EquipoReal
            {
                get
                {
                    return GetClaimValue("equipoReal", new List<int>());
                }
                set
                {
                    SetClaimValue("equipoReal", value);
                }
            }

            public static string IdActiveDirectory
            {
                get
                {
                    return GetClaimValue("IdActiveDirectory", "");
                }
                set
                {
                    SetClaimValue("IdActiveDirectory", value);
                }
            }

            public static string IdActiveDirectoryCompleto
            {
                get
                {
                    return GetClaimValue("IdActiveDirectoryCompleto", "");
                }
                set
                {
                    SetClaimValue("IdActiveDirectoryCompleto", value);
                }
            }

            public static List<int> CorredoresComercial
            {
                get
                {
                    return GetClaimValue("corredoresComercial", new List<int>());
                }
                set
                {
                    SetClaimValue("corredoresComercial", value);
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