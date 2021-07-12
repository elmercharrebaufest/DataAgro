using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultReportePagosDiferidos {
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
        public List<ReportePagosDiferidos> Contratos { get; set; } = new List<ReportePagosDiferidos>();
    }
    public class ReportePagosDiferidos
    {
        public string ContratoSAP { get; set; }
        public string Vendedor { get; set; }
        public string VendedorCUIT { get; set; }
        public int? VendedorID { get; set; }
        public string Corredor { get; set; }
        public string CorredorCUIT { get; set; }
        public int? CorredorId { get; set; }
        public string Estado { get; set; }
        public double Tn { get; set; }
        public decimal Precio { get; set; }
        public decimal PrecioUSD { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Capital { get; set; }
        public decimal TNA { get; set; }
        public decimal TEA { get; set; }
        public DateTime Toma { get; set; }
        public int Plazo { get; set; }
        public DateTime Vencimiento { get; set; }
        public int AlVencimiento { get; set; }
        public decimal CapitalMasIntereses { get; set; }
        public decimal InteresesTotales { get; set; }
        public decimal InteresesPorDia { get; set; }
        public decimal AcumuladoMesAnterior { get; set; }
        public decimal M2MMes { get; set; }
        public decimal DevengadoMes { get; set; }
        public List<ReportePagosDiferidosDia> Dias { get; set; } = new List<ReportePagosDiferidosDia>();

    }

    public class ReportePagosDiferidosDia
    {
        public DateTime Dia { get; set; }
        public decimal Importe { get; set; }
    }
}
