using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class SustentableDto
    {
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal? Importe { get; set; }
        public string MonedaId { get; set; } 
    }
}
