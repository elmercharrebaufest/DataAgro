
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class AreaInfluencia : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AreaInfluenciaId { get; set; }

        public string Descripcion { get; set; }


        public AreaInfluencia()
        {
            this.AreaInfluenciaId = 0;
            this.Descripcion = "";
  
        }
    }
}
   



