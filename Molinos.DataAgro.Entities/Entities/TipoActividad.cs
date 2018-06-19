
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class TipoActividad : Entity
    {
        public int TipoActividadId { get; set; }
        public string Descripcion { get; set; }
        public int? CamposExtra { get; set; }

        public TipoActividad()
        {
            
        }
    }


}
   


