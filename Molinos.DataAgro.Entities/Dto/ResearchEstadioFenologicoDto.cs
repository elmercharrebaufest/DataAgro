using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ResearchEstadioFenologicoDto
    {
        public int EstadioFenologicoId { get; set; }
        public int MaterialId { get; set; }
        public int EstadioId { get; set; }
        public bool ConRendimiento { get; set; }
    }
}
