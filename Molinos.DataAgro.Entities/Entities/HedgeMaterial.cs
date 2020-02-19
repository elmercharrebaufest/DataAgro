using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HedgeMaterial
    {
        [Key]
        public int Id { get; set; } // ContratoId (Primary key)
        public int MaterialId { get; set; } // MaterialId
        public decimal Cantidad { get; set; } // Cantidad
        public int TipoHedgeMaterialId { get; set; } 
        public DateTime Fecha { get; set; } // Fecha
        public int? ComercialId { get; set; } // ComercialId

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; } // MaterialId
        [ForeignKey("TipoHedgeMaterialId")]
        public virtual TipoHedgeMaterial TipoHedgeMaterial { get; set; } // TipoHedgeMaterial
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
    }
}



