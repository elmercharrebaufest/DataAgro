namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CentroHabilitadoSinBoletoDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int TipoNegocioId { get; set; }
        public string TipoNegocio { get; set; }
    }
}
