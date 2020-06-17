using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CupoSapDto
    {
        public string Codigo { get; set; }
        public string FechaIngreso { get; set; }
        public string Material { get; set; }
        public string Proveedor { get; set; }
        public string Planta { get; set; }
        public string Zona { get; set; }
        public string Observaciones { get; set; }
        public string Destinatario { get; set; }
        public string FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public string Comercial { get; set; }
        public string Borrado { get; set; }
    }
}
