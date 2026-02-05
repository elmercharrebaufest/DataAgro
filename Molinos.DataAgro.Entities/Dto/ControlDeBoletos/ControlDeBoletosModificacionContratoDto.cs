using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosModificacionContratoDto
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

    public class ControlDeBoletosRegistrarAccionesDto
    {
        public List<int> ControlDeBoletoIds { get; set; }
        public int AccionControlDeBoletos { get; set; }
    }

}
