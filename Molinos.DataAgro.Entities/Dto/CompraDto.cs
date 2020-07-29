using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CompraDto
    {
        public string Material { get; set; }
        public string Campana { get; set; }

        public CompraDetalleDto ConCorredor { get; set; }
        public CompraDetalleDto DirectoAcopiador { get; set; }
        public CompraDetalleDto DirectoProductor { get; set; }
    }
}
