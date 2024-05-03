using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ContratoAcuerdo : Negocio
    {
        public ContratoAcuerdo() : base()
        {
            Base = false;
            ImporteSustentable = 0;
            NoInformaSio = false;
            CantidadCamiones = 0;
        }

        [InverseProperty("ContratoAcuerdo")]
        public virtual ICollection<Calidad> Calidad { get; set; }

        [InverseProperty("ContratoAcuerdo")]
        public virtual ICollection<PrecioPactado> PrecioPactado { get; set; }
    }
}
