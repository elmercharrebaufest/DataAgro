namespace Molinos.DataAgro.Entities.Dto
{

    public class BoletoCompraNetProvinciaDto
    {
        public int Id { get; set; }
        public int BoletoCompraNetId { get; set; }
        public int ProvinciaId { get; set; }
        public string BoletoDescripcion { get; set; }
        public string ProvinciaNombre { get; set; }
    }
}