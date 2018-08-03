using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampañaMaterial
    {
        [Key]
        public int CampañaMaterialId { get; set; }
        public int CampañaId { get; set; }        
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public double ToneladasCompradas { get; set; }

        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
   


