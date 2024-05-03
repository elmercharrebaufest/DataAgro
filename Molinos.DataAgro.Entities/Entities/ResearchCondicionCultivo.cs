using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchCondicionCultivo
    {
        [Key]
        public int CondicionCultivoId { get; set; }
        public int MaterialId { get; set; }
        public int CondicionId { get; set; }
        public int Valor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CondicionId")]
        public virtual ResearchCondicion ResearchCondicion { get; set; }
    }
}
