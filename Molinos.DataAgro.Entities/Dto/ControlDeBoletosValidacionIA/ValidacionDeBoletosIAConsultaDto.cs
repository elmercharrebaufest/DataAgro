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
        public string Proveedor { get; set; }
        public string Material { get; set; }
        public string BolsaCompraNet { get; set; }

        // Validación
        public int? ValidacionBoletosEstadoId { get; set; }
        public string ValidacionBoletosEstado { get; set; }
        public DateTime? FechaValidacion { get; set; }

    }
}
