using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchEstadioFenologico
    {
        [Key]
        public int EstadioFenologicoId { get; set; }
        public int MaterialId { get; set; }
        public int EstadioId { get; set; }
        public bool ConRendimiento { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("EstadioId")]
        public virtual ResearchEstadio ResearchEstadio { get; set; }
    }
}
