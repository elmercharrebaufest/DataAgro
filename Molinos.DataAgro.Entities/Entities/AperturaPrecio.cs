using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AperturaPrecio
    {
        public int Id { get; set; }
        public int? ContratoId { get; set; }
        public int? FijacionId { get; set; }
        public int ConceptoAperturaPrecioId { get; set; }
        public decimal Importe { get; set; }
        public decimal Porcentaje { get; set; }
        public string MonedaId { get; set; }

        [ForeignKey("ConceptoAperturaPrecioId")]
        public virtual ConceptoAperturaPrecio ConceptoAperturaPrecio { get; set; }

        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }

        [ForeignKey("FijacionId")]
        public virtual FijacionDePrecioContrato FijacionDePrecioContrato { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

    }

}



