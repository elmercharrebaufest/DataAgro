using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Primer Negocio")]
    public class CriterioEsPrimerNegocio : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
