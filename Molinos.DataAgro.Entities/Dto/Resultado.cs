using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    [KnownType("TiposDeResultados")]
    public class Resultado
    {
        public List<ErrorMessage> Errores { get; set; } = new List<ErrorMessage>();

        [DataMember]
        public List<ErrorMessage> ListaErrores { get { return Errores; } }

        [DataMember]
        public bool HayError
        {
            get { return Errores.Count != 0; }
        }

        public bool HayErrores
        {
            get { return Errores.Count != 0; }
        }

        public void Error(string clave, string descripcion)
        {
            Errores.Add(new ErrorMessage(descripcion, clave));
        }


        /// <summary>
        /// Este metodo es para que cuando se exponga la clase Resultado por WCF, se expongan tambien todas las subclases
        /// </summary>
        /// <returns>La lista de subclases de Resultado que hay en el assembly</returns>
        public static Type[] TiposDeResultados()
        {
            var tipoResultado = typeof(Resultado);
            return tipoResultado.Assembly.GetTypes().Where(tipoResultado.IsAssignableFrom).ToArray();
        }
    }
}
