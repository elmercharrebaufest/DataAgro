
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ProveedorComercial : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProveedorComercialId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int ComercialId { get; set; }

        public ProveedorComercial()
        {
            
        }
    }


}
   


