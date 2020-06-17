using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HabilitacionCupo
    {
        [Key]
        public int Id { get; set; }
        public int ZonaCupoId { get; set; }

        public int MaterialId {get; set;}

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        [ForeignKey("ZonaCupoId")]
        public virtual ZonaCupo ZonaCupo { get; set; }


        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
