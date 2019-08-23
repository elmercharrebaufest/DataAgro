using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ConfiguracionCupo
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int CentroId { get; set; }
        public DateTime Fecha { get; set; }
        public int LimiteCupo { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [InverseProperty("ConfiguracionCupo")]
        public ICollection<LimiteCupo> CantidadCupo { get; set; }
    }
}
