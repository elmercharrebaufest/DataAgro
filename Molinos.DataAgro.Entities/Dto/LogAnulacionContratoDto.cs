using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class LogAnulacionContratoDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int NegocioId { get; set; }
        public string TipoNegocio { get; set; }
        public int ComercialId { get; set; }
        public string ContratoSAP { get; set; }
        public string FijacionSAP { get; set; }
        public double CantidadKilos { get; set; }
        public double KilosPendientes { get; set; }
    }
}
