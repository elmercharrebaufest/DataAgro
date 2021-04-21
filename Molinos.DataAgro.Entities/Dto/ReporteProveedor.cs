namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteProveedor
    {
        public int ProveedorId { get; set; }
		public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public int? EstadoId { get; set; }
        public string Comerciales { get; set; }
        public string Estado { get; set; }

        public string Alias { get; set; }
        
        public ReporteProveedor()
        {
        }
    }
}

