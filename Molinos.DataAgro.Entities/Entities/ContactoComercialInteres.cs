using Mastersoft.Framework.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContactoComercialInteres : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoComercialInteresId { get; set; }
        public int ContactoComercialId { get; set; }
        public int NroItem { get; set; }
        public int InteresId { get; set; }

        public ContactoComercialInteres()
        {
            
        }
    }


}
   


