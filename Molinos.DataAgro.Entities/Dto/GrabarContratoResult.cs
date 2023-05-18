using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarContratoResult : Resultado
    {
        public int? ContratoId { get; set; }
        public List<string> ListaCupos { get; set; } = new List<string>();
    }
}
