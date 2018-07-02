
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICondicionManager
    {
        Task<ResultIniCondicion> TraerTodoCondicionAsync();

        Task<Condicion> TraerCondicionAsync(int intCondicionId);

        Task<EntityErrors> GrabarCondicionAsync(Condicion oCondicion);

        Task<EntityErrors> EliminarCondicionAsync(int intCondicionId);
    }
}


