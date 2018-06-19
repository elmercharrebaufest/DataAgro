
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ProveedorDestinatario : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ContactoDestinatarioId { get; set; }
        public int ProveedorId { get; set; }
        public int NroItem { get; set; }
        public int DestinatarioId { get; set; }

        public ProveedorDestinatario()
        {
            
        }
    }


}
   


