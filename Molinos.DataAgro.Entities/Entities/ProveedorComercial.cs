using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorComercial
    {
        [Key]
        public int ProveedorComercialId { get; set; }
        public int ProveedorId { get; set; }        
        public int NroItem { get; set; }
        public int ComercialId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }
}
   


