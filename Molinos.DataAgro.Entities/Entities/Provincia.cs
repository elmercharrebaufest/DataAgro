
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Provincia : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProvinciaId { get; set; }

        public string Nombre { get; set; }


        public Provincia()
        {
            this.ProvinciaId = 0;
            this.Nombre = "";
  
        }
    }
}
   



