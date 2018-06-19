
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Zona : Entity
    {
        public int ZonaId { get; set; }
        public string Descripcion { get; set; }

        public Zona()
        {
            
        }
    }


}
   


