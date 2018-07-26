
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Threading.Tasks;

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


