namespace Molinos.DataAgro.Entities.Dto
{
    public partial class BusquedaHome
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public string Cuit { get; set; }
        public string Corredor { get; set; }
        public string Filtro { get; set; }
        public string Alias { get; set; }
        public int? ClasificacionId { get; set; }
        public string RiesgoComercialSap { get; set; }
        public string Estado { get; set; }
        public bool? Deshabilitado { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public bool Deshabilitar { get; set; }
        public string Color { get; set; }
    }
}
