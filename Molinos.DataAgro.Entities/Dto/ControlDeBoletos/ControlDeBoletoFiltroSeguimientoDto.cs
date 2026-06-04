using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletoFiltroSeguimientoDto
    {
        // Filtros de búsqueda
        public string ContratoSAP { get; set; }
        public int? MaterialId { get; set; }
        public DateTime? FechaCertificacionDesde { get; set; }
        public DateTime? FechaCertificacionHasta { get; set; }
        public DateTime? FechaVencimientoCertificacionDesde { get; set; }
        public DateTime? FechaVencimientoCertificacionHasta { get; set; }
        public DateTime? FechaRecepBoletoDesde { get; set; }
        public DateTime? FechaRecepBoletoHasta { get; set; }
        public DateTime? FechaEnviadoFirmaDesde { get; set; }
        public DateTime? FechaEnviadoFirmaHasta { get; set; }
        public DateTime? FechaRecibFirmaDesde { get; set; }
        public DateTime? FechaRecibFirmaHasta { get; set; }
        public DateTime? FechaEnvioBolsaDesde { get; set; }
        public DateTime? FechaEnvioBolsaHasta { get; set; }
        public DateTime? FechaVueltaBolsaDesde { get; set; }
        public DateTime? FechaVueltaBolsaHasta { get; set; }
        public DateTime? FechaEnvioAfipDesde { get; set; }
        public DateTime? FechaEnvioAfipHasta { get; set; }
        public DateTime? FechaVueltaAfipDesde { get; set; }
        public DateTime? FechaVueltaAfipHasta { get; set; }
        public int? Proveedor { get; set; }
        public int? BolsaId { get; set; }

        // Propiedades de paginación de Kendo Grid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public List<SortSeguimientoDescriptor> Sort { get; set; }
    }

    public class SortSeguimientoDescriptor
    {
        public string Field { get; set; }
        public string Dir { get; set; }
    }
}
