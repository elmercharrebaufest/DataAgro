using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Acopio
    {
        [Key]
        public int AcopioId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int? LocalidadId { get; set; }
        public string KMZnombre { get; set; }
        public string KMZfile { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
    }


}
   


