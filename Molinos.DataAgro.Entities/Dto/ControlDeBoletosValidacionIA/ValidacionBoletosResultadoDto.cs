using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA
{
    public class ValidacionBoletosResultadoDto
    {
        public int Id { get; set; }

        public int ValidacionBoletosId { get; set; }
        public string Resultado { get; set; }

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public DateTime? FechaRechazo { get; set; }
    }
}
