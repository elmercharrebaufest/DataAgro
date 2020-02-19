using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorCanalOperacion
    {
        [Key]
        public int ContactoCanalOperacionId { get; set; }
        public int ProveedorId { get; set; }
        public string NroItem { get; set; }
        public int CanalOperacionId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("CanalOperacionId")]
        public virtual CanalOperacion CanalOperacion { get; set; }
    }
}
   


