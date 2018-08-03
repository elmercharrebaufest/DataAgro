using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AcopioMaterial
    {
        [Key]
        public int AcopioMaterialId { get; set; }
        public int AcopioId { get; set; }
        public int NroItem { get; set; }
        public double? Toneladas { get; set; }
        public int CampañaId { get; set; }
        public int MaterialId { get; set; }

        [ForeignKey("AcopioId")]
        public virtual Acopio Acopio { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
   


