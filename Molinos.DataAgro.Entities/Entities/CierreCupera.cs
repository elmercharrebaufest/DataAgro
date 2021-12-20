using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CierreCupera
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public bool Cierre { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
   


