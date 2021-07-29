using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class EstadoPrecioMOA
    {
        [Key]
        public int Id { get; set; }       
        public int MaterialId { get; set; }
        public int TipoNegocioId { get; set; }
        public bool? Habilitado { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
    }
}

