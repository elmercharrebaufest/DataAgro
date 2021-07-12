using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionVirtualSap
    {
        [Key]
        public int Id { get; set; }
        public int FijacionCanjeId { get; set; }
        public int? FijacionVirtualId { get; set; }
        public string FijacionVirtualNro { get; set; }
        public double Cantidad { get; set; }

        [ForeignKey("FijacionCanjeId")]
        public virtual Negocio FijacionCanje { get; set; }
        [ForeignKey("FijacionVirtualId")]
        public virtual Negocio FijacionVirtual { get; set; }

    }


}
   


