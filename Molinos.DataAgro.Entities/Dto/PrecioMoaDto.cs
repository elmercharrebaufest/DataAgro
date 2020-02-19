using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioMoaDto
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime DesdeVigencia { get; set; }
        public DateTime HastaVigencia { get; set; }
    }
}
