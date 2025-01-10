using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ComercialEmpleadorACargoDto
    {
        public int ComercialId { get; set; }
        public int? EmpleadorACargoId { get; set; }
        public string EmpleadorACargo { get; set; }
    }
}
