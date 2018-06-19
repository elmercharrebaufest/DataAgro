
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
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
   


