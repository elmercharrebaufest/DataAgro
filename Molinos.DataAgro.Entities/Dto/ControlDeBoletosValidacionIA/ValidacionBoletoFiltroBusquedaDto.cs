using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA
{
    public class ValidacionBoletoFiltroBusquedaDto
    {
        // Filtros de búsqueda
        public string NegocioSAP { get; set; }
        public int? MaterialId { get; set; }
        public int? EstadoValidacionId { get; set; }
        public DateTime? FechaValidacionDesde { get; set; }
        public DateTime? FechaValidacionHasta { get; set; }
        public int? ProveedorId { get; set; }
        public int? BolsaId { get; set; }

        // Propiedades de paginación de Kendo Grid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public List<SortDescriptor> Sort { get; set; }
    }
}
