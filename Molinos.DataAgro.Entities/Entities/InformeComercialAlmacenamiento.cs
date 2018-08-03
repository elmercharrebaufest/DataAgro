using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercialAlmacenamiento
    {
        [Key]
        public int InformeComercialAlmacenamientoId { get; set; }
        public int? InformeComercialId { get; set; }        
        public decimal? Toneladas { get; set; }
        public int? LocalidadId { get; set; }        
        public bool? Propia { get; set; }
        public bool? Alquilada { get; set; }

        [ForeignKey("InformeComercialId")]
        public virtual InformeComercial InformeComercial { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
    }
}
