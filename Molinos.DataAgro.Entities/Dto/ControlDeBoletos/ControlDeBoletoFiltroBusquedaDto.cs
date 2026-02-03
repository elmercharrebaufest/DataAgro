using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletoFiltroBusquedaDto
    {
        public string ContratoSAPDesde { get; set; }
        public string ContratoSAPHasta { get; set; }
        public int? MaterialId { get; set; }
        public int? EstadoControlId { get; set; }
        public bool EsConfirma { get; set; }
        public DateTime? FechaCargaDesde { get; set; }
        public DateTime? FechaCargaHasta { get; set; }
        public int? Proveedor { get; set; }
        public int? BolsaId { get; set; }
        public int? ComercialId { get; set; }
    }
}
