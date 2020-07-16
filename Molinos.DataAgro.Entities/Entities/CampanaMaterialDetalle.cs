using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampanaMaterialDetalle
    {
        [Key]
        public int Id { get; set; }
        public int CampanaId { get; set; }           
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }

        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
   


