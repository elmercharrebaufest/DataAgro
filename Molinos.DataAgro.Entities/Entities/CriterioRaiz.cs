using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;
using System.Linq;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(false)]
    [DisplayName("Criterios")]
    public class CriterioRaiz : Criterio
    {
        public override bool Concreta { get { return false; } }
    }
}
