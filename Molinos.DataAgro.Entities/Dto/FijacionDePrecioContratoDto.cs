using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FijacionDePrecioContratoDto
    {
        public int FijacionDePrecioContratoId { get; set; }
        public int ContratoId { get; set; }
        public int ProveedorId { get; set; }
        public int? MaterialId { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }        
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public int? Ampliaciones { get; set; }
        public int EstadoId { get; set; }
        public string Observacion { get; set; }
    }
}

