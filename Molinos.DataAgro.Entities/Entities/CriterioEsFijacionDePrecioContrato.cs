using Molinos.DataAgro.Entities.CustomAtributte;
using System.ComponentModel;

namespace Molinos.DataAgro.Entities.Entities
{
    [Concreta(true)]
    [DisplayName("Es Fijación de Precio Contrato")]
    public class CriterioEsFijacionDePrecioContrato : Criterio
    {
        public override bool Concreta { get { return true; } }
    }
}
