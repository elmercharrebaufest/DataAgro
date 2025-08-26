using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Score Cupos")]
    public class CriterioScoreCupos : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
