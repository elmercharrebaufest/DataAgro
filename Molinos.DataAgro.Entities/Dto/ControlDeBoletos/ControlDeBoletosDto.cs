using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosDto
    {
        public int Id { get; set; }
        public int NegocioId { get; set; }
        public int ControlDeBoletosEstadoId { get; set; }
        public bool EsConfirma { get; set; }
        public int? AltaIdLoteConfirma { get; set; }
        public int? AltaIdDocumentoConfirma { get; set; }
        public int? IdentificadorConfirma { get; set; }
        public int? AltaIdLoteConfirmaAnterior { get; set; }
        public int? AltaIdDocumentoConfirmaAnterior { get; set; }
        public DateTime? FechaAnulacionConfirma { get; set; }



        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        // Campos BIT para control de procesos
        public bool ControlIniciado { get; set; }
        public bool ControlFinalizado { get; set; }
        public bool CertificacionCompletada { get; set; }
        public bool RegistroDatosOblea { get; set; }

        // Campos DATETIME para fechas de los procesos
        public DateTime? FechaControlIniciado { get; set; }
        public DateTime? FechaControlFinalizado { get; set; }
        public DateTime? FechaCertificacionCompletada { get; set; }
        public DateTime? FechaRegistroDatosOblea { get; set; }
    }
}
