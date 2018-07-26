using Mastersoft.Framework.Standard;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarContratoResult
    {
        public List<ErrorMessage> Errores { get; set; }
        public int? ContratoId { get; set; }


        public GrabarContratoResult()
        {
            Errores = new List<ErrorMessage>();
        }

      

    }
}
