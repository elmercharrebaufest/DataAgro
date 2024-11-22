using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteCompraNetDto
    {
        public List<ToneladasGranoTipoDto> ToneladasGranoTipo { get; set; }
        public ReporteSojaSustDto SojaSustentable { get; set; }
        public ReporteSojaEPAyEUDRDto SojaEPA { get; set; }
        public List<PosicionComprasDto> PosicionCompras { get; set; }
    }
}