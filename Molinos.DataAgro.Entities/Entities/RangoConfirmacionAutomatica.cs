using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class RangoConfirmacionAutomatica
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public int MaterialId { get; set; }
        public string MonedaId { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

        public DateTime FechaDesde { get; set; }
    }

}
   


