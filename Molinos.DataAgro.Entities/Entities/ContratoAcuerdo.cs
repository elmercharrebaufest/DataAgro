using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContratoAcuerdo : Negocio
    {
       
        [InverseProperty("ContratoAcuerdo")]
        public virtual ICollection<Calidad> Calidad { get; set; }

        [InverseProperty("ContratoAcuerdo")]
        public virtual ICollection<PrecioPactado> PrecioPactado { get; set; }
    }
}
