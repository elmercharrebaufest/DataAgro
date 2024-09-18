using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfirmaGeneradoDto
    {
        public string ContratoSAP { get; set; }
        public int ClaseNegocioId { get; set; }
        public int NegocioId { get; set; }
        public int ComercialId { get; set; }
        public bool Generado { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string FechaGeneracionFormateada { get { return FechaGeneracion == default(DateTime) ? "" : FechaGeneracion.ToShortDateString(); } }
        public bool IsWebService { get; set; }
        public string Mensaje { get; set; }
        public string Version { get; set; }
        public string FijacionSAP { get; set; }
        public int TipoBoletoId { get; set; }
        public string NegocioSAP { get; set; }
    }
}