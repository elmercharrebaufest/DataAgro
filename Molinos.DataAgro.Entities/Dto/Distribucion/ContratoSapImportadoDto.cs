using System;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ContratoSapImportadoDto
    {
        public string Numero { get; set; }
        public string NumeroSAP { get; set; }
        public string DescCl { get; set; }
        public string PrioLabel { get; set; }
        public int Rank { get; set; }
        public string Material { get; set; }
        public decimal Kg { get; set; }
        public string Cosecha { get; set; }
        public string FechaContrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string FechaHastaContra { get; set; }
        public string Cuit { get; set; }
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string Clasificacion { get; set; }
        public string OpType { get; set; }
        public string OpLabel { get; set; }
        public bool IsSust { get; set; }
        public decimal Valor { get; set; }
        public string Moneda { get; set; }
        public decimal PricePt { get; set; }
        public int PriceRank { get; set; }
    }
}
