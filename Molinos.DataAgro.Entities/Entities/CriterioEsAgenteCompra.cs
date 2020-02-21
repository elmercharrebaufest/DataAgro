using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Agente de Compra")]
    public class CriterioEsAgenteCompra : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
