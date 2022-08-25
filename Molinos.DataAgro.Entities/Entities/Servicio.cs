using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Servicio
    {
        public int Id { get; set; }
        public int? NegocioId { get; set; }
        public int ServicioValorId { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public bool Modificado { get; set; }
        public decimal Desde { get; set; }
        public decimal Hasta { get; set; }

        [ForeignKey("ServicioValorId")]
        public virtual ServicioValor ServicioValor { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

    }

}



