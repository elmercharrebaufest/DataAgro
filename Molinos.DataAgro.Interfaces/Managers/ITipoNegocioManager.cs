using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ITipoNegocioManager
    {
        Task<ResultIniTipoNegocio> TraerTodoAsync();

        Task<TipoNegocio> TraerTipoNegociodAsync(int TipoNegocioId);

        Task<EntityErrors> GrabarTipoNegocioAsync(TipoNegocio oTipoNegocio);

        Task<EntityErrors> EliminarTipoNegocioAsync(int TipoNegocioId);
    }
}


