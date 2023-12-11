using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchCoeficienteCultivo
    {
        [Key]
        public int CoeficienteCultivoId { get; set; }
        public int MaterialId { get; set; }
        public decimal Coeficiente { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
