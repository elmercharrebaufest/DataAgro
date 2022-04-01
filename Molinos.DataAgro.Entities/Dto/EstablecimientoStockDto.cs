using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class EstablecimientoStockDto
    {
        public string Establecimiento { get; set; }

        public decimal Cantidad { get; set; }

        public string Proveedor { get; set; }

        public string Cosecha { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
    }
}
