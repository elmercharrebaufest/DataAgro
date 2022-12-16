using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;

namespace Molinos.DataAgro.Entities.Entities
{
    public class CapacidadProductiva
    {
        [Key]
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public int CampaniaId { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        [ForeignKey("CampaniaId")]
        public virtual Campaña Campania { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
