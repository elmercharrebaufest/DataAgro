using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosModificadosLogDataAgroDto
    {
        public LogDataAgroDto LogActual { get; set; }
        public LogDataAgroDto LogAnterior { get; set; }
        public IList<DatoModificadosLogDataAgroDto> CamposCambiados { get; set; }
    }
    public class DatoModificadosLogDataAgroDto
    {
        public string Actual { get; set; }
        public string Anterior { get; set; }
        public string Campo { get; set; }
    }
}