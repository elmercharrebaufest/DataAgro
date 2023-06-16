using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IEnviarCapacidadProductivaSAPAgent
    {
        string EnviarCapacidadProductivaSAP(List<EnviarCapacidadProductivaSAPDto> capProd);
    }
}