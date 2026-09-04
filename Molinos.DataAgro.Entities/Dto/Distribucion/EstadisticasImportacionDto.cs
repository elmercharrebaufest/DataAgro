using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class EstadisticasImportacionDto
    {
        public List<Dictionary<string, object>> PorTipoOperador { get; set; }
        public List<Dictionary<string, object>> PorClase { get; set; }
        public int Sustentables { get; set; }
        public string FechaDesdeMin { get; set; }
        public string FechaDesdeMax { get; set; }
        public string FechaHastaMin { get; set; }
        public string FechaHastaMax { get; set; }

        public EstadisticasImportacionDto()
        {
            PorTipoOperador = new List<Dictionary<string, object>>();
            PorClase = new List<Dictionary<string, object>>();
        }
    }
}
