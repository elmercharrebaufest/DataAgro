using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ContratoTerceroDto
    {
        public int Id { get; set; }
        public int? TipoId { get; set; }
        public string CuitProveedor { get; set; }
        public int ComercialId { get; set; }
        public string Rol { get; set; }
        public string CuitCorredor { get; set; }
    }
}
