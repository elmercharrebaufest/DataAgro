using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class DescuentoBonificacion
    {
        public int Id { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public decimal Porcentaje { get; set; }
        public int TipoDBId { get; set; }
        public int TipoPeriodoDBId { get; set; }
        public int ContratoId { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("TipoDBId")]
        public virtual TipoDB TipoDB { get; set; }
        [ForeignKey("TipoPeriodoDBId")]
        public virtual TipoPeriodoDB TipoPeriodoDB { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
    }

}
   


