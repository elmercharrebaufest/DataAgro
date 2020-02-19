using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Espacio Dinamico")]

    public class CriterioEsEspacioDinamico : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
