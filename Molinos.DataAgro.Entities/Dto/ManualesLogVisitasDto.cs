using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ManualesLogVisitasDto
    {
        public int Id { get; set; }
        public int ManualId { get; set; }
        public int ComercialId { get; set; }
        public DateTime FechaVisita { get; set; }
    }
}
