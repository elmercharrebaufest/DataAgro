using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ConfiguracionDistribucionDto
    {
        public string PlantaCodigo { get; set; }
        public string PlantaNombre { get; set; }
        public int CupoKg { get; set; }

        [Range(0.0, 1.0)]
        public decimal CuitMaxPct { get; set; }

        public List<string> CosechasValidas { get; set; }
        public List<string> ClasesExcluidas { get; set; }
        public Dictionary<string, int> LimitesPredeterminadosPorMaterial { get; set; }
        public Dictionary<string, decimal> PreciosReferencia { get; set; }
        public Dictionary<string, int> CuotasPorOperador { get; set; }
        public Dictionary<string, int> CuotasPorClase { get; set; }
        public string UltimaActualizacion { get; set; }

        public ConfiguracionDistribucionDto()
        {
            CosechasValidas = new List<string>();
            ClasesExcluidas = new List<string>();
            LimitesPredeterminadosPorMaterial = new Dictionary<string, int>();
            PreciosReferencia = new Dictionary<string, decimal>();
            CuotasPorOperador = new Dictionary<string, int>();
            CuotasPorClase = new Dictionary<string, int>();
        }
    }
}
