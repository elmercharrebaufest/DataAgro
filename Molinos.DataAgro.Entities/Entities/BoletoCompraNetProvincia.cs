using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class BoletoCompraNetProvincia
    {
        [Key]
        public int Id { get; set; }
        public int BoletoCompraNetId { get; set; }
        public int ProvinciaId { get; set; }

        [ForeignKey("BoletoCompraNetId")]
        public virtual BoletoCompraNet BoletoCompraNet { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }

    }
}
   


