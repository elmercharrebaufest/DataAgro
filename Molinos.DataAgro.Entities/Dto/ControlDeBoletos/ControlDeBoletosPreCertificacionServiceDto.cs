using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosPreCertificacionServiceDto
    {
        public string ContratoSAP { get; set; }
        public List<ControlDeBoletosDatosPreCertificacionServiceDto> Detalle { get; set; }

    }
    public class ControlDeBoletosDatosPreCertificacionServiceDto
    {
        public string TipoOblea { get; set; }
        public string Oblea { get; set; }
        public string Bolsa { get; set; }
        public string FechaCertificacion { get; set; }
        public string FechaVencimiento { get; set; }
        public string Rechazado { get; set; }
    }
}
