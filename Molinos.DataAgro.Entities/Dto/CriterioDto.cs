using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CriterioDto
    {
        public int Id { get; set; }
        public int Prioridad { get; set; }
        public int? PadreId { get; set; }
        //public int FormulaId { get; set; }
        //public Formula Formula { get; set; }
    }
}
