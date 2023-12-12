using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ResearchCoeficienteCultivoDto
    {
        public int CoeficienteCultivoId { get; set; }
        public int MaterialId { get; set; }
        public decimal Coeficiente { get; set; }
    }
}
