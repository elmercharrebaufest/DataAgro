using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampoMaterial
    {
        [Key]
        public int CampoMaterialid { get; set; }
        public int? CampoId { get; set; }
        public int NroItem { get; set; }
        public int MaterialId { get; set; }
        public int? Hectareas { get; set; }
        public int? Toneladas { get; set; }
        public int CampañaId { get; set; }

        [ForeignKey("CampoId")]
        public virtual Campo Campo { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }

    }
}
   


