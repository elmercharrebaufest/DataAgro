using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class LimiteCupo
    {
        [Key]
        public int Id { get; set; }
        public int ConfiguracionCupoId { get; set; }
        public int ZonaCupoId { get; set; }
        public int CantidadCupo { get; set; }
        
        public int? LimiteAnterior { get; set; }

        [ForeignKey("ConfiguracionCupoId")]
        public virtual ConfiguracionCupo ConfiguracionCupo { get; set; }
        [ForeignKey("ZonaCupoId")]
        public virtual ZonaCupo ZonaCupo { get; set; }
    }
}