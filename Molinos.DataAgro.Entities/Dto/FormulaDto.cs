using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FormulaDto
    {

        public int Id { get; set; }
        public Criterio Criterio { get; set; }
        public int Inicio { get; set; }
        public int CantDias { get; set; }


    }
}
