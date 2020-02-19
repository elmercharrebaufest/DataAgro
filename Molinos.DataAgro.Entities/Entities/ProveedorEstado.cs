using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorEstado
    {
        [Key]
        public int ProveedorEstadoId { get; set; }
        public int ProveedorId { get; set; }
        public int EstadoId { get; set; }
        public int ComercialId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("EstadoId")]
        public virtual Estado Estado { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }
}



