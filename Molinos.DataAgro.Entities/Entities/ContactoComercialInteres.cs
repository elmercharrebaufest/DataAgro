using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContactoComercialInteres
    {
        [Key]
        public int ContactoComercialInteresId { get; set; }
        public int ContactoComercialId { get; set; }
        public int NroItem { get; set; }
        public int InteresId { get; set; }

        [ForeignKey("ContactoComercialId")]
        public virtual ContactoComercial ContactoComercial { get; set; }
        [ForeignKey("InteresId")]
        public virtual Interes Interes { get; set; }
    }


}
   


