
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
using NLog;
using Molinos.DataAgro.Entities.Entities;

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

                    var logger = LogManager.GetLogger("Global");
                    logger.Error(filterContext.Exception.GetOriginalException(), "Excepción no manejada: ");
                    var oErroresManager = new ErroresManager(oMSContext);

                    oErroresManager.Grabar(oErrores);
                }
                catch (Exception ex)
                {
                    var logger = LogManager.GetLogger("Global");
                    logger.Error(ex, "Excepción no manejada: ");
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