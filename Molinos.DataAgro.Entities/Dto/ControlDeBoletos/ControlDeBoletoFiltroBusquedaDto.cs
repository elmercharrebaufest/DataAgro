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
        public string NegocioSAP { get; set; }
        public int? MaterialId { get; set; }
        public int? EstadoControlId { get; set; }
        public bool EsConfirma { get; set; }
        public bool EsBoletoFisico { get; set; }
        public bool EsCartaOferta { get; set; }
        public bool EsSinBoleto { get; set; }
        public bool EsNinguno { get; set; }

        public DateTime? FechaCargaDesde { get; set; }
        public DateTime? FechaCargaHasta { get; set; }
        public int? Proveedor { get; set; }
        public int? BolsaId { get; set; }
        public int? ComercialId { get; set; }

        // Propiedades de paginación de Kendo Grid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
        public List<SortDescriptor> Sort { get; set; }
    }

    public class SortDescriptor
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }
}
