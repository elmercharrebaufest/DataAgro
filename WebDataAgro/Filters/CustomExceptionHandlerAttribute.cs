
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net;
using System.Diagnostics;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;

using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities;
using System.IO;
using System.Configuration;

namespace WebDataAgro.Filters
{
    public class CustomExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {
        //--------------------------------------------------
        //  Constantes Privadas
        //--------------------------------------------------

        private const string DefaultCNPrefix = "DataAgro";

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
        
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                try
                {
                    var oErrorRegister = new ErrorRegister();

                    var oErrorData = oErrorRegister.GetFullErrorData(filterContext.Exception);

                    var oErrores = new Errores()
                    {
                        ErrorDateTime = DateTime.Now,
                        MachineName = oErrorData.MachineName,
                        AppDomainName = oErrorData.AppDomainName,
                        //ThreadIdentity = oErrorData.ThreadIdentity,
                        WindowsIdentity = oErrorData.WindowsIdentity,
                        Message = oErrorData.Message,
                        FullException = oErrorData.FullException
                    };
                    
                    var json = oErrorRegister.GetJsonExceptionMessage(filterContext.Exception);
                    
                    filterContext.Result = new ContentResult { Content = json, ContentType = "application/json" };
                    
                    var oMSContext = new MSContext()
                    {
                        CNPrefix = DefaultCNPrefix,
                        EmpresaId = 1
                    };


                    if (ConfigurationManager.AppSettings["LogFile"]== "1")
                    {
                        // FileStream stream = new FileStream(ConfigurationManager.AppSettings["pathError"] + "error.txt", FileMode.OpenOrCreate, FileAccess.Write);
                        //StreamWriter writer = new StreamWriter(stream);
                        StreamWriter writer = File.AppendText(@ConfigurationManager.AppSettings["pathError"] + "error.txt");
                        // Escribimos fecha y hora del instante
                        writer.WriteLine("Hora Inicio " + DateTime.Now);
                        writer.WriteLine("Error");
                        writer.WriteLine(filterContext.Exception.GetOriginalException().Message);
                        writer.WriteLine("Detalle");
                        writer.WriteLine(filterContext.Exception.GetAllFootprints());
                        writer.WriteLine("Stack");
                        writer.WriteLine(new StackTrace(filterContext.Exception, true).ToString());
                        writer.WriteLine("Fin");
                        writer.Close();
                    }
                    var oErroresManager = new ErroresManager(oMSContext);

                    oErroresManager.Grabar(oErrores);


                }
                catch (Exception ex)
                {

                    // FileStream stream = new FileStream(ConfigurationManager.AppSettings["pathError"] + "error.txt", FileMode.OpenOrCreate, FileAccess.Write);
                    //StreamWriter writer = new StreamWriter(stream);
                    StreamWriter writer = File.AppendText(@ConfigurationManager.AppSettings["pathError"] + "error.txt");
                    // Escribimos fecha y hora del instante
                    writer.WriteLine("Hora Inicio " + DateTime.Now);
                    writer.WriteLine("Error");
                    writer.WriteLine(ex.GetOriginalException().Message);
                    writer.WriteLine("Detalle");
                    writer.WriteLine(ex.GetAllFootprints());
                    writer.WriteLine("Stack");
                    writer.WriteLine(new StackTrace(ex, true).ToString());
                    writer.WriteLine("Fin");
                    writer.Close();
                }

                filterContext.ExceptionHandled = true;
            }
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