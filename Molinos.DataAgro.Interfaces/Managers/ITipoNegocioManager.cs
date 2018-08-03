using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoNegocioManager
    {
        ResultIniTipoNegocio TraerTodo();

        TipoNegocio TraerTipoNegociod(int TipoNegocioId);

        Resultado GrabarTipoNegocio(TipoNegocio oTipoNegocio);

        Resultado EliminarTipoNegocio(int TipoNegocioId);
    }
}


