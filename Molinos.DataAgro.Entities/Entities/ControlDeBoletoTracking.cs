using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ControlDeBoletoTracking
    {
        public int Id { get; set; }
        public int ControlDeBoletosId { get; set; }
        public int? EstadoDocumentoId { get; set; }
        public int? CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Acciones { get; set; }
        public DateTime FechaCreacion { get; set; }
        public virtual ControlDeBoletos ControlDeBoletos { get; set; }
    }
}
