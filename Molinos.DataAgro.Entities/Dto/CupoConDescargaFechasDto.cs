using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CupoConDescargaFechasDto
    {
        public DateTime Fecha { get; set; }
        public int CantidadCupo { get; set; }
        public int CantidadFlete { get; set; }
    }
}
