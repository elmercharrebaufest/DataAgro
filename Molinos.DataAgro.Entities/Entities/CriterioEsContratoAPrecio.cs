using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Contrato A Precio")]
    public class CriterioEsContratoAPrecio : Criterio
    {
        public bool Concreta { get { return true; } }
    }
}
