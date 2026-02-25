using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosReporteSeguimientoConsultaDto
    {
        public int Id { get; set; }
        public int NegocioId { get; set; }
        public int ControlDeBoletosEstadoId { get; set; }
        public string ControlDeBoletosEstado { get; set; }
        public bool EsConfirma { get; set; }

        public string AltaIdLoteConfirma { get; set; }
        public string IdentificadorConfirma { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public int? EstadoConfirmaId { get; set; }

        public DateTime? FechaControlIniciado { get; set; }
        public DateTime? FechaControlFinalizado { get; set; }
        public DateTime? FechaCertificacionCompletada { get; set; }
        public DateTime? FechaRegistroDatosOblea { get; set; }

        public int MaterialId { get; set; }
        public string Material { get; set; }

        public int BolsaCompraNetId { get; set; }
        public string BolsaCompraNet { get; set; }

        public int ComercialId { get; set; }
        public string Comercial { get; set; }
        public string TipoBoleto { get; set; }

        public string ContratoSAP { get; set; }

        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }

        public int? PreCertificacionId { get; set; }
        public int? SeguimientoBoletoId { get; set; }

        // 🔹 PRECERTIFICACION
        public DateTime? FechaCertificacion { get; set; }
        public DateTime? FechaVencimientoCertificacion { get; set; }

        // 🔹 SEGUIMIENTO
        public DateTime? FechaEnviadoFirma { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public DateTime? FechaEnvioAfip { get; set; }
        public DateTime? FechaEnvioBolsa { get; set; }
        public DateTime? FechaRecepBoleto { get; set; }
        public DateTime? FechaRecibFirma { get; set; }
        public DateTime? FechaVueltaAfip { get; set; }
        public DateTime? FechaVueltaBolsa { get; set; }
        public DateTime? FechaAcopio { get; set; }
    }
}
