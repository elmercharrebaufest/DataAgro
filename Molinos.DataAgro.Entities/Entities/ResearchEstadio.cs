using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchEstadio
    {
        [Key]
        public int EstadioId { get; set; }
        public string Descripcion { get; set; }
    }
}
