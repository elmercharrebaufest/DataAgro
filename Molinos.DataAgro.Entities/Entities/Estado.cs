
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Estado : Entity
    {
        public int EstadoId { get; set; }
        public string Descripcion { get; set; }

        public Estado()
        {
            
        }
    }
}
   
