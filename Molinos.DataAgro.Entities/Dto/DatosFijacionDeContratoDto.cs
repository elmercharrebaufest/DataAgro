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
        public string DesdeEntrega { get; set; }
        public string HastaEntrega { get; set; }
        public string Posicion { get; set; }
        public bool? Calidad { get; set; }
        public string Campana { get; set; }
        public bool? PagoDiferido { get; set; }

    }
}

