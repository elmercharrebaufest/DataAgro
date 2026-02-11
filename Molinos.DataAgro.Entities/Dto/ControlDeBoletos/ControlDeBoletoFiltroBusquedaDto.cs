using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletoFiltroBusquedaDto
    {
        // Filtros de búsqueda
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

        // Propiedades de paginación de Kendo Grid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public List<SortDescriptor> Sort { get; set; }
    }

    public class SortDescriptor
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }
}
