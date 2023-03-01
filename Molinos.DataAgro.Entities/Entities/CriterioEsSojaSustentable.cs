using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Soja Sustentable")]
    public class CriterioEsSojaSustentable : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
