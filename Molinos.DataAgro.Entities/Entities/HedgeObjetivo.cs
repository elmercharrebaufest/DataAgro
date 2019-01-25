using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HedgeObjetivo
    {
        [Key]
        public int Id { get; set; } // ContratoId (Primary key)
        public int MaterialId { get; set; } // MaterialId
        public decimal Cantidad { get; set; } // Cantidad
        public int TipoObjetivoId { get; set; } 
        public DateTime Fecha { get; set; } // Fecha
        public int? ComercialId { get; set; } // ComercialId

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; } // MaterialId
        [ForeignKey("TipoObjetivoId")]
        public virtual TipoObjetivo TipoObjetivo { get; set; } // TipoHedgeMaterial
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
    }
}



