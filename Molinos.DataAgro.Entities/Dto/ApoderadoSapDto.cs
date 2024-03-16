using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ApoderadoSapDto
    {
        public string Nombres { get; set; }
        public string Apellido { get; set; }
        public string Puesto { get; set; }
        public string CuitApoderado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}