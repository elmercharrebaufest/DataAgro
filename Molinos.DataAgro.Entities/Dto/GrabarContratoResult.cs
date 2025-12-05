using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class GrabarContratoResult : Resultado
    {
        [DataMember]

        public int? ContratoId { get; set; }
        [DataMember]
        public List<string> ListaCupos { get; set; } = new List<string>();
    }

    [DataContract]
    public class GrabarContratoResultDto
    {
        [DataMember]
        public int? ContratoId { get; set; }
        [DataMember]
        public int? FijacionDePrecioContratoId { get; set; }

        [DataMember]
        public List<string> ListaCupos { get; set; } = new List<string>();

        [DataMember]
        public List<string> Errores { get; set; } = new List<string>();


        //[DataMember]
        //public bool HayError
        //{
        //    get { return Errores.Count != 0; }
        //}
    }

    [DataContract]
    public class ErrorMessageDtos
    {
        [DataMember]

        public string Message { get; set; }
        [DataMember]
        public string Source { get; set; }

    }
}
