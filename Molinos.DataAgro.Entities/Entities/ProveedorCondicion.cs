
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ProveedorCondicion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoCondicionId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int CondicionId { get; set; }

        public ProveedorCondicion()
        {
            
        }
    }


}
   


