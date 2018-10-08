using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    [KnownType("TiposDeResultados")]
    public class Resultado
    {
        [DataMember]
        public List<ErrorMessage> Errores { get; set; } = new List<ErrorMessage>();

        public bool HayErrores
        {
            get { return Errores.Count != 0; }
        }

        public void Error(string clave, string descripcion)
        {
            Errores.Add(new ErrorMessage(descripcion, clave));
        }
    }
}
