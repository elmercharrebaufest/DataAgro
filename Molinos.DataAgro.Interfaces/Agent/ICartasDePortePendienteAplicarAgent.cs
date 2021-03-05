using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICartasDePortePendienteAplicarAgent
    {
        List<CcPpPerndienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req);
    }
}