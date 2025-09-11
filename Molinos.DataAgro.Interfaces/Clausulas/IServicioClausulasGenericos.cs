using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.ServiceModel;

namespace Molinos.DataAgro.Interfaces.Clausulas
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioClausulasGenericos
    {
        [OperationContract]
        ResultadoClausula DevolverClausulas(ClausulaGenericos comando);
    }
}
