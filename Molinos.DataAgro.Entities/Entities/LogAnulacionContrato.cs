using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class LogAnulacionContrato
    {
        [Key]
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
