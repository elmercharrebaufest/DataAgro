using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ToneladasGranoTipoDto
    {
        public string Material { get; set; }
        public double DispAFijar { get; set; }
        public double DispAPrecio { get; set; }
        public double DispFijac { get; set; }
        public double FrwAFijar { get; set; }
        public double FrwAPrecio { get; set; }
        public double FrwFijac { get; set; }
        public double NewAFijar { get; set; }
        public double NewAPrecio { get; set; }
        public double NewFijac { get; set; }
        public double Total { get; set; }
    }
}
