namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ResultadoContratoDistribucionDto
    {
        public string NumeroSAP { get; set; }
        public string Proveedor { get; set; }
        public string Cuit { get; set; }
        public string Material { get; set; }
        public string OpType { get; set; }
        public string Clase { get; set; }
        public decimal KgContrato { get; set; }
        public decimal CcppDescontado { get; set; }
        public decimal KgEfectivo { get; set; }
        public int CuposNecesarios { get; set; }
        public int CuposAsignados { get; set; }
        public string Estado { get; set; }
        public bool IsSust { get; set; }
        public decimal PricePt { get; set; }
    }
}
