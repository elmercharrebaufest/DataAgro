using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosFijacionDeContratoDto
    {
        public string ContratoId { get; set; }
        public string KilosAplicados { get; set; }
        public string KilosPendiente { get; set; }
        public string KilosContrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Filtro { get; set; }

    }
}

