using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class RegistroDatosCertificacionControlDeBoletosDto
    {
        public string Contrato { get; set; }
        public string Fecha { get; set; }
        public string Fijacion { get; set; }
        public string Hora { get; set; }
        public string Usuario { get; set; }
        public List<RegistroDatosCertificacionControlDeBoletosDetalleDto> Detalle { get; set; }
    }
    public class RegistroDatosCertificacionControlDeBoletosDetalleDto
    {
        public string Tipo { get; set; }
        public string Oblea { get; set; }
        public string Bolsa { get; set; }
        public string FeCertificacion { get; set; }
        public string FeVencCerti { get; set; }
        public string Rechazado { get; set; }
    }
}
