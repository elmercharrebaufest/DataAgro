using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Cumplimiento Cupo")]
    public class CriterioCumplimientoCupo : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
