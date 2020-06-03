using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("CD Warrant")]
    public class CriterioCDWarrant : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
