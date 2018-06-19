
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class TipoTelefono : Entity
    {
        public int TipoTelefonoId { get; set; }
        public string Descripcion { get; set; }

        public TipoTelefono()
        {
            
        }
    }


}
   


