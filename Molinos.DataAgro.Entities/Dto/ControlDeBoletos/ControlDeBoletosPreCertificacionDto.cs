using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosPreCertificacionDto
    {
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public string Oblea { get; set; } 
        public int BolsaCompraNetId { get; set; }
        public int TipoObleaId { get; set; }
        public DateTime FechaCertificacion { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string Rechazado { get; set; }

    }
}
