using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class AltaMasivaCupoDto
    {
        public string ContratoSAP { get; set; }
        public DateTime FechaSugerida { get; set; }
        public int CantidadDeCupos { get; set; }
        public int MaterialId { get; set; }
        public DateTime FechaHasta { get; set; }
        public DateTime? FechaHastaOriginal { get; set; }
    }
}
