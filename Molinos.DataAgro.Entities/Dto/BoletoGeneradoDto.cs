using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class BoletoGeneradoDto
    {
        public string ContratoSAP { get; set; }
        public int TipoNegocioId { get; set; }
        public int TipoNegocioDetalleId { get; set; }
        public int TipoBoletoId { get; set; }
        public int NegocioId { get; set; }
        public int ComercialId { get; set; }
        public int Version { get; set; }
        public bool Generado { get; set; }
        public bool Mail { get; set; }
        public string Mensaje { get; set; }
        public string FijacionSAP { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public bool IsConfirma { get; set; }
    }
}