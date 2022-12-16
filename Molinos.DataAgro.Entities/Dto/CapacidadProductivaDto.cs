

using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CapacidadProductivaDto
    {
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public int CampaniaId { get; set; }
        public string Material { get; set; }
        public string Campania { get; set; }
        public decimal Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public decimal Porcentaje { get; set; }
        public DateTime? FechaActualizacion { get; set; }
    }
}
