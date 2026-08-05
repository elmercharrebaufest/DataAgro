using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA
{
    public class ValidacionDeBoletosIAConsultaDto
    {
        public int Id { get; set; }

        // Campos Contrato
        public string TipoBoleto { get; set; }
        public string ContratoSAP { get; set; }
        //Versiones Boleto
        public int? Version { get; set; }
        public DateTime? FechaGeneracion { get; set; }

        // Información Comercial
        public string BolsaCompraNet { get; set; }
        public string Material { get; set; }
        public string Proveedor { get; set; }
        // Validación
        public string ValidacionBoletosEstado { get; set; }
        public int? ValidacionBoletosEstadoId { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public string RequestId { get; set; }
        public string EstadoValidacionAgente { get; set; }
        public string AccionesRecomendadas { get; set; }
        public string Observacion { get; set; }


    }
}
