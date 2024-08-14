namespace Molinos.DataAgro.Entities.Dto
{
    public partial class LocalidadDto
    {
        public int LocalidadId { get; set; }
        public string Nombre { get; set; }
        public string CodLocalidad { get; set; }
        public string CodigoPostal { get; set; }
        public string SubCodigoPostal { get; set; }
        public int ProvinciaId { get; set; }
        public string Provincia_Nombre { get; set; }
        public string Partido_Nombre { get; set; }
        public int? PartidoId { get; set; }
        public string CodigoConfirma { get; set; }
    }
}
   



