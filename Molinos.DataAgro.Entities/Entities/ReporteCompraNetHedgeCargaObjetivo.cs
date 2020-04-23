using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReporteCompraNetHedgeCargaObjetivo
    {
        public int Id { get; set; }

        public decimal PricingCumplido { get; set; }

        public decimal RemitirCumplido { get; set; }

        public decimal PricingObjetivo { get; set; }

        public decimal RemitirObjetivo { get; set; }

    }
}
