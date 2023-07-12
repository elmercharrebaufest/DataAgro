using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Con Descarga")]
    public class CriterioConDescarga : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}