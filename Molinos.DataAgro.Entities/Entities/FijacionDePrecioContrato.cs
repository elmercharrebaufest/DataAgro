using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato
    {
        [Key]
        public int FijacionDePrecioContratoId { get; set; }
        public int ContratoId { get; set; }
        public int ProveedorId { get; set; }
        public int? MaterialId { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }
        
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public int? Ampliaciones { get; set; }
        public int EstadoId { get; set; }
        public string Observacion { get; set; }

        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }
}

