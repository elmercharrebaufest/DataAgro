using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class GrabarAcuerdoResult : Resultado
    {
        public int? AcuerdoId { get; set; }
        public List<string> ListaCupos { get; set; } = new List<string>();
    }
}