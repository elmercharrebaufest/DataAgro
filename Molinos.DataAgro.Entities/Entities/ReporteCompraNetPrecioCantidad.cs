using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReporteCompraNetPrecioCantidad
    {
        public int Id { get; set; }

        public string Moneda { get; set; }

        public double? Cantidad { get; set; }

    }
}
