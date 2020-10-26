using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HabilitacionPizarra
    {
        [Key]
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public DateTime DesdeVigencia { get; set; }
        public DateTime HastaVigencia { get; set; }
        public int TipoNegocioId { get; set; }
        public DateTime? DesdeEntrega { get; set; }
        public DateTime? HastaEntrega { get; set; }
        public int MaterialId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
    }
}
