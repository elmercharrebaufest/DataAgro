using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecio
    {
        [Key]
        public int FijacionId { get; set; }
        public int? MaterialId { get; set; }
        public decimal? Precio { get; set; }
        public DateTime? Fecha { get; set; }
        public int? ProveedorId { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        
        public FijacionDePrecio()
        {
            Precio = null;
            Fecha = DateTime.Now;
        }
    }
}
   
