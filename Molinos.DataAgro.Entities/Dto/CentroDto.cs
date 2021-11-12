namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CentroDto
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
        public bool Acopio { get; set; }
        public bool ValidaRedespacho { get; set; }
        public int? LocalidadId { get; set; }
        public string Localidad { get; set; }
        public string CodigoPostal { get; set; }
        public string Direccion { get; set; }
        public bool Comision { get; set; }
    }

}



