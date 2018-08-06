using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoNegocioManager
    {
        ResultIniTipoNegocio TraerTodo();

        TipoNegocioDto TraerTipoNegociod(int TipoNegocioId);

        Resultado GrabarTipoNegocio(TipoNegocio oTipoNegocio);

        Resultado EliminarTipoNegocio(int TipoNegocioId);
    }
}


