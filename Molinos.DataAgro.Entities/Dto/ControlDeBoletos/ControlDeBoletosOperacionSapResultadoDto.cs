using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    [DataContract]
    public class ControlDeBoletosOperacionSapResultadoDto
    {
        public List<ErroresControlDeBoletosOperacionSapDto> Errores { get; set; } = new List<ErroresControlDeBoletosOperacionSapDto>();

        [DataMember]
        public bool EjecutadoCorrectamente
        {
            get { return Errores.Count == 0; }
            set { }
        }

        [DataMember]
        public List<ErroresControlDeBoletosOperacionSapDto> ListaErrores
        {
            get { return Errores; }
            set { Errores = value ?? new List<ErroresControlDeBoletosOperacionSapDto>(); }
        }
    }
    [DataContract]

    public class ErroresControlDeBoletosOperacionSapDto
    {
        private string mstrMessage;

        [DataMember]
        public string Message
        {
            get
            {
                return this.mstrMessage;
            }
            set
            {
                this.mstrMessage = value;
            }
        }
    }
}
