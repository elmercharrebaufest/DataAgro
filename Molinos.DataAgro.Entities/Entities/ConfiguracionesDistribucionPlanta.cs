using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ConfiguracionesDistribucionPlanta
    {
        [Key]
        public int Id { get; set; }
        public string PlantaCodigo { get; set; }
        public string PlantaNombre { get; set; }
        public int CupoKg { get; set; }
        public decimal CuitMaxPct { get; set; }
        public string CosechasValidas { get; set; }
        public string ClasesExcluidas { get; set; }
        public string LimitesPredeterminados { get; set; }
        public string PreciosReferencia { get; set; }
        public string CuotasPorOperador { get; set; }
        public string CuotasPorClase { get; set; }
        public DateTime? UltimaActualizacion { get; set; }
    }
}
