using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FinDelDia
    {
        [Key]
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public bool Cerrado { get; set; }
        public int ComercialId { get; set; }
        public int? ReabrioComercialId { get; set; }
        public double? Diferencial { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ReabrioComercialId")]
        public virtual Comercial ReabrioComercial { get; set; }
    }
}
   


