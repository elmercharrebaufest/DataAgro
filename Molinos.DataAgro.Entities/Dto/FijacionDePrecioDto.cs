using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FijacionDePrecioDto
    {
        public int FijacionId { get; set; }
        public int? MaterialId { get; set; }
        public decimal? Precio { get; set; }
        public DateTime? Fecha { get; set; }
        public int? ProveedorId { get; set; }
    }
}
   
