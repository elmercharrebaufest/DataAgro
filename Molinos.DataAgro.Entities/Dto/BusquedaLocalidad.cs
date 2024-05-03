namespace Molinos.DataAgro.Entities.Dto
{
    public partial class BusquedaLocalidad
    {
        public int Id { get; set; }
        public string Localidad { get; set; }
        public string CodLocalidad { get; set; }
        public string Provincia { get; set; }
        public string Partido { get; set; }
        public string Descripcion { get; set; }
        public int PartidoId { get; set; }

        public int ProvinciaId { get; set; }
        public string Filtro { get; set; }
    }
}
