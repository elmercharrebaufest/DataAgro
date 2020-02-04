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
        public bool Retirado { get; set; }
        public bool Pizarra { get; set; }
    }
}
