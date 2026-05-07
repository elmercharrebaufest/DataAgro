using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletoTrackingDto
    {
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public int? EstadoDocumentoId { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Acciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
