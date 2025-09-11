using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Clausulas
{
    [ServiceContract(Namespace = "http://scato.molinos.com.ar")]
    public interface IServicioClausulasCartaOferta
    {
        [OperationContract]
        ResultadoClausula DevolverClausulas(ClausulaCartaOferta comando);
    }
}
