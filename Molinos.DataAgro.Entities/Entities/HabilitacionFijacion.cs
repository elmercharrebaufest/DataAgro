using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HabilitacionFijacion
    {
        [Key]
        public int Id { get; set; }
        public DateTime Dia{ get; set; }
        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }

    }
}

