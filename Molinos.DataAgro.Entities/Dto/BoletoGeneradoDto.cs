using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class BoletoGeneradoDto
    {
        public string ContratoSAP { get; set; }
        public int TipoNegocioId { get; set; }
        public int Version { get; set; }
        public bool Generado { get; set; }
        public bool Mail { get; set; }
        public string Mensaje { get; set; }
    }
}
