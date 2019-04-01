using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PosicionPorMaterial
    {
        public int Id { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
