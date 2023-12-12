using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchTipoCarga
    {
        [Key]
        public int TipoCargaId { get; set; }
        public string Descripcion { get; set; }
    }
}
