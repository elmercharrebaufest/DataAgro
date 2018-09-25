using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioCantidadDto
    {
        public string Moneda { get; set; }
        public double? Cantidad { get; set; }
        public PrecioCantidadDto()
        {
            Moneda = "ARP ";
            Cantidad = 0;
        }
    }
}
