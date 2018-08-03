using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercialProduccion
    {
        [Key]
        public int InformeComerciaProduccionId { get; set; }
        public int InformeComercialId { get; set; }
        public int? MaterialId { get; set; }
        public int? Hectareas { get; set; }
        public decimal? Toneladas { get; set; }
        public int? LocalidadId { get; set; }
        public bool? Propio { get; set; }
        public bool? Alquilado { get; set; }
        public bool? RtaOkSap { get; set; }
        public string MensajeSap { get; set; }

        [ForeignKey("InformeComercialId")]
        public virtual InformeComercial InformeComercial { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
    }
}
