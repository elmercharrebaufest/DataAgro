using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class AceptarSugerenciaCupoDto
    {
        public int nro { get; set; }
        public int cantidad { get; set; }
        public int idSugerencia { get; set; }
        public DateTime? fecha { get; set; }
        public DateTime? fechaOriginal { get; set; }
        public int proveedorId { get; set; }
        public string razonSocial { get; set; }
        public int cantidadFleteProcedencia { get; set; }
        public int? materialId { get; set; }
        public int comercialId { get; set; }
        public string centroId { get; set; }
    }
}
