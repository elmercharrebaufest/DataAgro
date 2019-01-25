using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HedgeTC
    {
        [Key]
        public int Id { get; set; } // 
        public decimal TipoCambio { get; set; } 
        public decimal HedgePesos { get; set; }
        public DateTime Fecha { get; set; } // Fecha
        public int? ComercialId { get; set; } // ComercialId
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
    }
}



