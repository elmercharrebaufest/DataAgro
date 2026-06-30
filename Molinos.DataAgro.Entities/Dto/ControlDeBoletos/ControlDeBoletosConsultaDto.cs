using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosConsultaDto
    {
        public int Id { get; set; }
        public int? SeguimientoBoletoId { get; set; }

        public int NegocioId { get; set; }
        public int ControlDeBoletosEstadoId { get; set; }
        public string ControlDeBoletosEstado { get; set; }
        public int? EstadoConfirmaId { get; set; }
        public string EstadoConfirma { get; set; }
        public bool EsConfirma { get; set; }
        public string TipoAltaConfirma{ get; set; }
        public int? AltaIdLoteConfirma { get; set; }
        public int? IdentificadorConfirma { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        
        public string TipoBoleto { get; set; }

        // Campos Contrato
        public string ContratoSAP { get; set; }
        public int? ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public int? ComercialId { get; set; }
        public string Comercial { get; set; }
        public int? MaterialId { get; set; }
        public string Material { get; set; }
        public int? BolsaCompraNetId { get; set; }
        public string BolsaCompraNet { get; set; }

        //Versiones Boleto
        public int? Version { get; set; }
        public DateTime? FechaGeneracion { get; set; }

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
