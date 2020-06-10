using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DatosModificadosLogDataAgroDto
    {
        public LogDataAgroDto LogActual { get; set; }
        public LogDataAgroDto LogAnterior { get; set; }
        public IList<string> CamposCambiados { get; set; }
    }
}