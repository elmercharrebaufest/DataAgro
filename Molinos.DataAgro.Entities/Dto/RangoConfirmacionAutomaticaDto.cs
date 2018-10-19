namespace Molinos.DataAgro.Entities.Dto
{
    public partial class RangoConfirmacionAutomaticaDto
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public string Moneda { get; set; }
        public string MonedaId { get; set; }
    }
}
   


