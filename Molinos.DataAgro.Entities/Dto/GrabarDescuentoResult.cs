using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarDescuentoResult
    {
        public List<ErrorMessage> Errores { get; set; }
        public int? ContratoId { get; set; }


        public GrabarDescuentoResult()
        {
            Errores = new List<ErrorMessage>();
        }
    }
}
