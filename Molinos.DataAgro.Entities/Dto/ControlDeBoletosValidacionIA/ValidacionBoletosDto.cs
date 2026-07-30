using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA
{
    public class ValidacionBoletosDto
    {
        public int Id { get; set; }

        public int ControlDeBoletosId { get; set; }

        public int ValidacionBoletosEstadoId { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public DateTime? FechaRechazo { get; set; }

        public string MotivoRechazo { get; set; }
    }
}
