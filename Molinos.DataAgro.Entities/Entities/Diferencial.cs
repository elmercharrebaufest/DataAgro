using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Diferencial
    {
        [Key]
        public int Id { get; set; } // 
        public int DiferencialDefault { get; set; } 
        public DateTime Fecha { get; set; } // Fecha
        public int? ComercialId { get; set; } // ComercialId
        public int TipoNegocioId { get; set; } 
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; } 
    }
}



