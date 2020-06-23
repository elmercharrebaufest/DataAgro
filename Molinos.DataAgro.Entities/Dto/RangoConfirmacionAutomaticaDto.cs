using System;

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
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int ZonaId { get; set; }
        public string Zona { get; set; }
        public int Cantidad { get; set; }
        public int DesdeMes { get; set; }
        public int DesdeAnio { get; set; }
        public int HastaMes { get; set; }
        public int HastaAnio { get; set; }
        public int TipoNegocioId { get; set; }
        public string TipoNegocio { get; set; }

        public int? UsuarioCreadorId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public string UsuarioCreador { get; set; }
    }
}
   


