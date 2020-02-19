using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Delta de Precio")]
    public  class CriterioDeltaDePrecio : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
