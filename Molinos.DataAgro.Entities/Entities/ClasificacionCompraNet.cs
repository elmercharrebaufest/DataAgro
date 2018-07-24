
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class ClasificacionCompraNet : Entity
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }

        public ClasificacionCompraNet()
        {
            
        }
    }


}
   


