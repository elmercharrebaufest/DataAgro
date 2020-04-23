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
        public string Latitud { get; set; }
        public string KMZnombre { get; set; }
        public string KMZfile { get; set; }
        public string Longitud { get; set; }
        public string Nombre { get; set; }
        public int? ComercialId { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }


}
   


