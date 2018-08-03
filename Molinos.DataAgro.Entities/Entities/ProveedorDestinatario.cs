using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ProveedorDestinatario
    {
        [Key]
        public int ContactoDestinatarioId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int DestinatarioId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("DestinatarioId")]
        public virtual Destinatario Destinatario { get; set; }
    }
}
   


