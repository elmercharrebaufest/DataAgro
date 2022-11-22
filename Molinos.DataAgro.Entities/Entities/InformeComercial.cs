using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercial
    {
        [Key]
        public int InformeComercialId { get; set; }
        public int ProveedorId { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? ComercialId { get; set; }
        public bool? EmplRelDep { get; set; }
        public string EmplRelDepCant { get; set; }
        public int? Rodados { get; set; }
        public string RodadosOtros { get; set; }
        public int? Chacra { get; set; }
        public string ChacraOtros { get; set; }
        public string AntigActividad { get; set; }
        public string ActuacionProd { get; set; }
        public string ClienteAnt { get; set; }
        public string Comentarios { get; set; }
        public string RespuestaSap { get; set; }
        public string DomicilioReal { get; set; }
        public int? CampañaId { get; set; }
        public int? EstadoId { get; set; }
        public DateTime? FechaDescarga { get; set; }
        public bool? OrigenDA { get; set; }

        [InverseProperty("InformeComercial")]
        public virtual List<InformeComercialAlmacenamiento> InformeComercialAlmacenamiento { get; set; }
        [InverseProperty("InformeComercial")]
        public virtual List<InformeComercialProduccion> InformeComercialProduccion { get; set; }

        [ForeignKey("EstadoId")]
        public virtual InformeComercialEstado Estado { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
    }
}
