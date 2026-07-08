using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosParaModificarDto
    {
        public int ControlDeBoletosId { get; set; }
        public int NegocioId { get; set; }
        public int ProveedorId { get; set; }
        public string CUITProveedor { get; set; }
        public int CorredorId { get; set; }
        public string CUITCorredor { get; set; }
        public bool EsCorredor { get; set; }
        public string TipoBoleto { get; set; }
        public bool EsCartaOferta { get; set; }
        public bool EsSinBoleto { get; set; }

        public bool OperaSinOblea { get; set; }
        public string ContratoSAP { get; set; }

        // 🔹 PRECERTIFICACION
        public int? PreCertificacionId { get; set; }
        public string Oblea { get; set; }
        public int? PreCertificacionBolsaCompraNetId { get; set; }
        public string PreCertificacionBolsa { get; set; }

        public DateTime? FechaCertificacion { get; set; }
        public DateTime? FechaVencimientoCertificacion { get; set; }

        // 🔹 SEGUIMIENTO
        public int? BolsaCompraNetId { get; set; }
        public string BolsaSellado { get; set; }
        public int? SeguimientoBoletoId { get; set; }
        public DateTime? FechaRecepBoleto { get; set; }
        public DateTime? FechaEnviadoFirma { get; set; }
        public DateTime? FechaEnvioBolsa { get; set; }
        public DateTime? FechaEnvioAfip { get; set; }
        public DateTime? FechaRecibFirma { get; set; }
        public DateTime? FechaVueltaBolsa { get; set; }
        public DateTime? FechaVueltaAfip { get; set; }
        public DateTime? FechaEnvioSellado { get; set; }
    }
}
