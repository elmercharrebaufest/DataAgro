namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CapacidadProductivaDesactualizadaDto
    {
        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public string Cosecha { get; set; }
        public string RazonSocial { get; set; }
        public string NombreComercial { get; set; }
        public string SupervisorComercial { get; set; }
        public string Corredor { get; set; }
    }
}