using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.ServiceModel;

namespace Molinos.DataAgro.Interfaces.Clausulas
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioClausulas
    {
        [OperationContract]
        ResultadoClausula DevolverClausulas(Clausula comando);
    }
}
