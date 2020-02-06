using Molinos.DataAgro.Entities.Entities;
using System.ServiceModel;

namespace Molinos.DataAgro.Interfaces.Criterios
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioCriterios
    {
        [OperationContract]
        decimal Calcular(Criterio comando);
    }
}
