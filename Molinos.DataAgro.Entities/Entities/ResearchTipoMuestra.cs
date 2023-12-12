using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchTipoMuestra
    {
        [Key]
        public int TipoMuestraId { get; set; }
        public string Descripcion { get; set; }
    }
}
