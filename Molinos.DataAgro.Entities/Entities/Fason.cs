using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Fason
    {
        [Key]
        public int Id { get; set; }
        public int TipoFasonId { get; set; }
        public int FasoneroId { get; set; }
        public int MaterialId { get; set; }
        public int CampanaId { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public double Cantidad { get; set; }
        public string Posicion { get; set; }        
        public DateTime Fecha { get; set; }
        public int ComercialId { get; set; }
        public int EstadoId { get; set; }
        public double? Ampliaciones { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; } // CampañaId
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("FasoneroId")]
        public virtual Proveedor Fasonero { get; set; }
        [ForeignKey("TipoFasonId")]
        public virtual TipoFason TipoFason { get; set; }
    }
}

