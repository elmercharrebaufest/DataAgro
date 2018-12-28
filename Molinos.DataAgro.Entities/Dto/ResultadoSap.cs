using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoSap
    {
        public List<ErrorMessage> ListaErrores { get; set; }
        
        public bool HayError
        {
            get { return ListaErrores.Count != 0; }
        }
    }
}
