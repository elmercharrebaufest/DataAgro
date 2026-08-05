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

        public string Campo { get; set; }

        public string ValorDocumento { get; set; }

        public string ValorSistema { get; set; }

        public string Resultado { get; set; }

        public string Severidad { get; set; }

        public string Mensaje { get; set; }

        public string TipoCoincidencia { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
