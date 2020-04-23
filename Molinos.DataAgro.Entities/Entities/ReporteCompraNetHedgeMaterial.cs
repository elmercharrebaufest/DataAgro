using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReporteCompraNetHedgeMaterial
    {
        public int Id { get; set; }

        public string Material { get; set; }

        public decimal Disponible { get; set; }

        public decimal Forward { get; set; }

        public decimal NewCrop { get; set; }

    }
}
