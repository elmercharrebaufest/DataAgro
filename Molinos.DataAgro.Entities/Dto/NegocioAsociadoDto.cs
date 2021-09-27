namespace Molinos.DataAgro.Entities.Dto
{
    public partial class NegocioAsociadoDto
    {
        public int Id { get; set; }
        public string TipoNegocioDesc { get; set; }
        public decimal Precio { get; set; }
        public string MonedaDesc { get; set; }
        public string MaterialDesc { get; set; }
        public double Cantidad { get; set; }
        public string Campania { get; set; }
        public string Posicion { get; set; }
        public string Color { get; set; }

        public string Identificador { get; set; }
        public string Texto { get; set; }
        public string Contrato { get; set; }
        public string ContratoSap { get; set; }
    }
}
   



