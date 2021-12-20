using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class MensajeCupoDto
    {
        public int ProveedorId { get; set; }
        public string Descripcion { get; set; }

        public DateTime Fecha { get; set; }
        public string Proveedor { get; set; }
        public List<string> Solicitudes { get; set; }
        public List<ErrorMessage> Error { get; set; }
        public List<string> Cupos { get; set; }
    }
}
