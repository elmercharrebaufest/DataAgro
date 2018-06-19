using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report
{
    internal class Varios
    {
        //--------------------------------------------------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------------------------------------------------

        public static string GetPathFiles()
        {
            return ConfigurationManager.AppSettings["PathFiles"];
        }


        public static string GetPDFName()
        {
            DateTime oNow = DateTime.Now;

            string strFechaHora = oNow.ToString("yyyyMMddHHmmss");

            string strTicks = oNow.Ticks.ToString();

            return strFechaHora + "_" + strTicks + ".PDF";
        }


        public static string GetFullPathPDF(string strFile)
        {
            return Path.Combine(GetPathFiles(), strFile);
        }


        public static string GetIdentif()
        {
            DateTime oNow = DateTime.Now;

            string strFechaHora = oNow.ToString("yyyyMMddHHmmss");

            string strTicks = oNow.Ticks.ToString();

            string strIdent = strFechaHora + strTicks;

            return strIdent;
        }


    }

}
