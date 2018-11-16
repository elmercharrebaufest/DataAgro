using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CorredorProveedor
    {
        [Key]
        public int Id { get; set; }
        public int CorredorId { get; set; }        
        public int ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }
    }
}
   


