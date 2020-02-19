
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Mastersoft.Framework.DataRepository;
using System.IO;
using System.Configuration;
using System.Xml.Serialization;
using System.Runtime.Serialization.Json;

namespace Molinos.DataAgro.ServiceHost
{
    public static class Util
    {
        //--------------------------------------------------
        //  Constantes Privadas
        //--------------------------------------------------

        private const string DefaultCNPrefix = "DataAgro";
        
        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public static MSContext GetMSContext()
        {
            var oMSContext = new MSContext()
            {
                CN = "",
                CNPrefix = DefaultCNPrefix,
                DBProvider = "SQLServer",
                EmpresaId = 1,
                UsuarioId = 1,
                Usuario = "",
                PerfilId = 0
            };

            return oMSContext;
        }

        public static void GarbarArchivo(String Archivo, string InfoAGrabar )
        {
            if (ConfigurationManager.AppSettings["LogFile"] == "1")
            {
                StreamWriter writer = File.AppendText(@ConfigurationManager.AppSettings["pathArchivo"] + Archivo);
                writer.WriteLine("Hora Inicio " + DateTime.Now);
                writer.WriteLine("Detalle");
                writer.WriteLine( InfoAGrabar);
                writer.WriteLine("Fin");
                writer.Close();
            }
        }
    }
}