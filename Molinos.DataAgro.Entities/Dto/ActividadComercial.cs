using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ActividadComercial
    {
        public string Comercial { get; set; }
        public string Proveedor { get; set; }
        public string CUIT { get; set; }
        public DateTime? Negocio { get; set; }
        public DateTime? ModificacionProveedor { get; set; }
        public DateTime? InformeComercial { get; set; }
        public DateTime? Agenda { get; set; }
        public DateTime? Cupo { get; set; }
        public DateTime? FechaAlta { get; set; }
    }
}