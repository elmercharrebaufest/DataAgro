using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Fason")]
    public class CriterioEsFason : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
