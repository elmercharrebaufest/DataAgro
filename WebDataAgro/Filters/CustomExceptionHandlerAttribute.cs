using NLog;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace WebDataAgro.Filters
{
    public class CustomExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (ApiRequestSecurityHelper.IsValidacionBoletosApiRequest(filterContext.HttpContext?.Request))
            {
                return;
            }

            if (!filterContext.ExceptionHandled)
            {
                try
                {
                    var logger = LogManager.GetLogger("Global");
                    logger.Error(filterContext.Exception.GetOriginalException(), "Excepción no manejada: ");
                }
                catch (Exception ex)
                {
                    var logger = LogManager.GetLogger("Global");
                    logger.Error(ex, "Excepción no manejada: ");
                }

                filterContext.ExceptionHandled = true;
            }
            try
            {
                if (filterContext.Exception is SqlException && (filterContext.Exception as SqlException).Number == -2)
                {
                    EnviarMailTimeOut(filterContext);
                }
            }
            catch (Exception)
            {
            }
        }

        private static void EnviarMailTimeOut(ExceptionContext filterContext)
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
                Body = filterContext.Exception.Message + "<br>" + filterContext.Exception.StackTrace,
                IsBodyHtml = true
            };
            foreach (var item in ConfigurationManager.AppSettings["EmailDASoporte"].Split(';'))
            {
                oMensaje.To.Add(item);
            }

            oCliente.Send(oMensaje);
        }
    }

    public static class ExceptionExtensions
    {
        public static Exception GetOriginalException(this Exception ex)
        {
            if (ex.InnerException == null) return ex;

            return ex.InnerException.GetOriginalException();
        }

        public static string GetAllFootprints(this Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = "";
            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString +=
                    "File: " + frame.GetFileName() +
                    ", Method:" + frame.GetMethod().Name +
                    ", LineNumber: " + frame.GetFileLineNumber();

                traceString += "  -->  ";
            }

            return traceString;
        }
    }
}