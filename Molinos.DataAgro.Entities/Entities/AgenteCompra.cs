using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AgenteCompra
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int OperadorId { get; set; }
        public int TipoAgenteCompraId { get; set; }
        public string Posicion { get; set; }        
        public DateTime Fecha { get; set; }
        public int ComercialId { get; set; }
        public int EstadoId { get; set; }
        public double? Ampliaciones { get; set; }
        public int? ComercialCreadorId { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("OperadorId")]
        public virtual Operador Operador { get; set; }

        [ForeignKey("TipoAgenteCompraId")]
        public virtual TipoAgenteCompra TipoAgenteCompra { get; set; }
        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial ComercialCreador { get; set; }
    }
}

