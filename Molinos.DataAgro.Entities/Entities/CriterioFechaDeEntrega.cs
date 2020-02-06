using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Fecha de Entrega")]
    public  class CriterioFechaDeEntrega : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
