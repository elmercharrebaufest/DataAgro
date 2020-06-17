
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosCompraNetDto
    {
        public int ProveedorId { get; set; }
        public int? LocalidadId { get; set; }
        public string Localidad { get; set; }
        public int? ProvinciaId { get; set; }
        public string Provincia { get; set; }
        public int? ClasificacionCompraNetId { get; set; }
        public bool? Consignatario { get; set; }
        public int? BoletoCompraNetId { get; set; }
        public int? BolsaCompraNetId { get; set; }
        public decimal? ComisionPorcentaje { get; set; }
        public bool? PlanCanje { get; set; }
    }
}



