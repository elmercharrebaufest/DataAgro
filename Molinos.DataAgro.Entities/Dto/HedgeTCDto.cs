using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class HedgeTCDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public decimal TC { get; set; }
        public decimal HedgePesos { get; set; }
        public string Comercial { get; set; }
    }
}



