using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Actividad
    {
        [Key]
        public int ActividadId { get; set; }
        public int TipoActividadId { get; set; }
        public string Detalle { get; set; }
        public int ProveedorId { get; set; }
        public DateTime FechaHoraActividad { get; set; }
        public DateTime? FechaHoraRecordatorio { get; set; }
        public int? ComercialId { get; set; }
        public int? ContactoComercialId { get; set; }        
        public string asunto { get; set; }
        public DateTime? FechaHoraRecordatorioFin { get; set; }

        [ForeignKey("TipoActividadId")]
        public virtual TipoActividad TipoActividad { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ContactoComercialId")]
        public virtual ContactoComercial ContactoComercial { get; set; }

    }
}
   


