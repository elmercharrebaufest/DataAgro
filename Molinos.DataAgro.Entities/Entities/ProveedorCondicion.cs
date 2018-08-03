using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorCondicion
    {
        [Key]
        public int ContactoCondicionId { get; set; }
        public int ProveedorId { get; set; }        
        public int NroItem { get; set; }
        public int CondicionId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("CondicionId")]
        public virtual Condicion Condicion { get; set; }
    }
}
   


