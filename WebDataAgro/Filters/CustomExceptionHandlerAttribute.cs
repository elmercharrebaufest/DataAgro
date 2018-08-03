
using NLog;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace WebDataAgro.Filters
{
    public class CustomExceptionHandlerAttribute : FilterAttribute, IExceptionFilter
    {        
        public void OnException(ExceptionContext filterContext)
        {
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