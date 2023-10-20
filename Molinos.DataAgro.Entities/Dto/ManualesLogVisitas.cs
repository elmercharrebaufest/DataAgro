using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ManualesLogVisitas
    {
        public int Id { get; set; }
        public int ManualId { get; set; }
        public int ComercialId { get; set; }
        public DateTime FechaVisita { get; set; }
    }
}
