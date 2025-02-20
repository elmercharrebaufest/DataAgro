using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Operador
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoPrimary { get; set; }
        public int? ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
    }
}