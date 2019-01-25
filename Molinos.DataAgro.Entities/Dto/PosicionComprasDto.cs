using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PosicionComprasDto
    {
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public List<PosicionKilos> PosicionKilos { get; set; }
        public double Total { get; set; }
    }

    public class PosicionKilos
    {
        public EnumMeses Mes { get; set; }
        public double Kilos { get; set; }
        public int? Anio { get; set; }
    }
}
