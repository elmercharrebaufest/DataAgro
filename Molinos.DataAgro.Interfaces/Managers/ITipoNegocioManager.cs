using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoNegocioManager
    {
        TipoNegocioDto TraerTipoNegociod(int TipoNegocioId);
        List<TipoNegocioDto> TraerTodoTipoNegocio();
    }
}


