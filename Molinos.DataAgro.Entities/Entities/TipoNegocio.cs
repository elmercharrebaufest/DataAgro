
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Interfaces;

namespace Molinos.DataAgro.Entities
{
    public partial class TipoNegocio : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoNegocioId { get; set; }
        public string Descripcion { get; set; }


        public TipoNegocio()
        {
            
        }
    }


}
   


