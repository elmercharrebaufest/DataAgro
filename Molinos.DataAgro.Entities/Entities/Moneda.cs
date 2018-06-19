
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Moneda : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MonedaId { get; set; }
        public string Descripcion { get; set; }
       

        public Moneda()
        {
            
        }
    }


}
   


