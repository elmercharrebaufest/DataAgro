using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Process;
using System.IO;
using System.Configuration;
using System.Diagnostics;

namespace Processes
{
    class Program
    {
        static void Main(string[] args)
        {
            var process = new ProcessHelper();


            try
            {
                if (ConfigurationManager.AppSettings["ProcessComprasAyer"] == "1")
                    process.ActualizarComprasAyer();
            }
            catch (Exception ex)
            {
                GrabarError(ex);
            }

            try
            {
                if (ConfigurationManager.AppSettings["ActualizarGrupoCompras"] == "1")
                    process.ActualizarGrupoCompras();

            }
            catch (Exception ex)
            {
                GrabarError(ex);
            }

            try
            {
                if (ConfigurationManager.AppSettings["ProcessRg2300"] == "1")
                    process.ProcesarRg2300();
            }
            catch(Exception ex)
            {
                GrabarError(ex);
            }

            try
            {
                if (ConfigurationManager.AppSettings["ProcessFacacop"] == "1")
                    process.ProcesarFacacop();
            }
            catch (Exception ex)
            {
                GrabarError(ex);
            }

            try
            {
                if (ConfigurationManager.AppSettings["ProcessEstado"] == "1")
                    process.ActualizarEstadoDelProveedor();
            }
            catch(Exception ex)
            {
                GrabarError(ex);
            }

            try
            {
                if (ConfigurationManager.AppSettings["ProcessCompras"] == "1")
                    process.ActualizarComprasProveedor();
            }
            catch(Exception ex)
            {
                GrabarError(ex);
            }
            
        }

        private static void GrabarError(Exception ex)
        {
            StreamWriter writer = File.AppendText(@ConfigurationManager.AppSettings["pathError"] + "errorProceso.txt");
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
