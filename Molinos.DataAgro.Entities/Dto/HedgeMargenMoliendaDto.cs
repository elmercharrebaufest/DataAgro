using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class HedgeMargenMoliendaDto
    {
        public int Id { get; set; }
        public int MargenMolienda { get; set; }
        public DateTime Fecha { get; set; }
        public int ComercialId { get; set; }
        public string Comercial { get; set; }
    }
}



