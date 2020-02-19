using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class AperturaPrecioDto
    {
        public int Id { get; set; }
        public int? contratoId { get; set; }
        public int? FijacionId { get; set; }
        public int ConceptoAperturaPrecioId { get; set; }
        public decimal Importe { get; set; }
        public decimal Porcentaje { get; set; }
        public string MonedaId { get; set; }
        public string ConceptoAperturaPrecio { get; set; }
        public string Moneda { get; set; }

    }
}
