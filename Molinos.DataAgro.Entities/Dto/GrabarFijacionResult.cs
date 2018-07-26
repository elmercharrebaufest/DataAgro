using Mastersoft.Framework.Standard;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarFijacionResult
    {
        public List<ErrorMessage> Errores { get; set; }
        public int? FijacionDePrecioContratoId { get; set; }

        public GrabarFijacionResult()
        {
            Errores = new List<ErrorMessage>();
        }
    }
}
