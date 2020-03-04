using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AperturaPrecio
    {
        public int Id { get; set; }
        public int? NegocioId { get; set; }
        public int ConceptoAperturaPrecioId { get; set; }
        public decimal Importe { get; set; }
        public decimal Porcentaje { get; set; }
        public string MonedaId { get; set; }

        [ForeignKey("ConceptoAperturaPrecioId")]
        public virtual ConceptoAperturaPrecio ConceptoAperturaPrecio { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

    }

}



