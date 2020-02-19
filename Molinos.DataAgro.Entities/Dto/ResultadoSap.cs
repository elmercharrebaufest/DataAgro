using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoSap
    {
        public ResultadoSap()
        {
            ListaErrores = new List<ErrorMessage>();
        }

        public List<ErrorMessage> ListaErrores { get; set; }
        
        public bool HayError { get; set; }
    }
}
