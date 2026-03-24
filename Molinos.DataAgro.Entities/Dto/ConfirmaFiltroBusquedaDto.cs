using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaFiltroBusquedaDto
    {
        // Filtros de búsqueda
        public string NegocioSAP { get; set; }
        public DateTime? FechaConfirmacionDesde { get; set; }
        public DateTime? FechaConfirmacionHasta { get; set; }
        public DateTime? FechaEnvioDesde { get; set; }
        public DateTime? FechaEnvioHasta { get; set; }
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public int? BolsaCompraNetId { get; set; }
        public int? MaterialId { get; set; }
        public bool EsSoloPendientes { get; set; }

        // Propiedades de paginación de Kendo Grid
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 50;
        public List<SortDescriptor> Sort { get; set; }
    }
}
