using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class HedgeMaterialDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDesc { get; set; }
        public decimal Cantidad { get; set; }
        public int TipoHedgeMaterialId { get; set; }
        public string TipoHedgeMaterialDesc { get; set; }
        public DateTime Fecha { get; set; }
        public string Comercial { get; set; }
    }
}
   


