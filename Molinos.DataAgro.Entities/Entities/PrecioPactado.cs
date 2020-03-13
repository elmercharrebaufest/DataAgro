using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class PrecioPactado
    {
        public int Id { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal Precio { get; set; }
        public string MonedaPactadoId { get; set; }
        public decimal? ImportePactado { get; set; }
        public string MonedaImportePactadoId { get; set; }
        public decimal? Porcentaje { get; set; }
        public int ContratoId { get; set; }

        [ForeignKey("MonedaPactadoId")]
        public virtual Moneda MonedaPactado { get; set; }
        [ForeignKey("MonedaImportePactadoId")]
        public virtual Moneda MonedaImportePactado { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
    }

}


