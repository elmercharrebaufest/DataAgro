using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosModificacionContratoResultDto
    {
        public string Mensaje { get; set; }
        public List<string> Errores { get; set; } = new List<string>();

    }
}
