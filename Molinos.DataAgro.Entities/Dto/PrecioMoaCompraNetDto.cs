using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioMoaCompraNetDto
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public int TipoNegocioId { get; set; }
        public string TipoNegocio { get; set; }
        public bool Retirado { get; set; }
        public bool Pizarra { get; set; }
        public DateTime? DesdeEntrega { get; set; }
        public DateTime? HastaEntrega { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public int? DestinoId { get; set; }
        public string Destino { get; set; }
    }
}
