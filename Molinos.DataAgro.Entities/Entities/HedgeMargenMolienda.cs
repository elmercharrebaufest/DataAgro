using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HedgeMargenMolienda
    {
        [Key]
        public int Id { get; set; }
        public int MargenMolienda { get; set; }
        public DateTime Fecha { get; set; }
        public int ComercialId { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }
}



