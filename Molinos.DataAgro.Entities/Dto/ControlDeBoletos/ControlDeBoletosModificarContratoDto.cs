using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosModificarContratoDto
    {
        public string Clasificacion { get; set; }
        public string Contrato { get; set; }
        public string Cosecha { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string Procedencia { get; set; }
        public string Provincia { get; set; }
        public string Usuario { get; set; }
    }
}
