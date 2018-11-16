using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosFijacionDeContratoDto
    {
        public int ContratoId { get; set; }
        public double KilosAplicados { get; set; }
        public double KilosPendiente { get; set; }
        public double KilosContrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Filtro { get; set; }

    }
}

