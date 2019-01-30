using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class HedgeObjetivoDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public decimal Cantidad { get; set; }
        public int TipoObjetivoId { get; set; }
        public string TipoHedgeMaterialDesc { get; set; }
        public DateTime Fecha { get; set; }
        public string Comercial { get; set; }
        public string MaterialDesc { get; set; }
    }
}
   


