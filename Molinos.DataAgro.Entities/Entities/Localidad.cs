
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class Localidad : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int LocalidadId { get; set; }

        public string CodLocalidad { get; set; }
        public string Nombre { get; set; }
        public int ProvinciaId { get; set; }


        public Localidad()
        {
            this.LocalidadId = 0;
            this.CodLocalidad = "";
            this.Nombre = "";
            this.ProvinciaId = 0;
  
        }
    }
}
   



