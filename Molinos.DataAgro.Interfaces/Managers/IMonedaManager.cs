
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMonedaManager
    {
        Task<ResultIniMoneda> TraerTodoAsync();

        Task<Moneda> TraerMonedadAsync(string MonedaId);

        Task<EntityErrors> GrabarMonedaAsync(Moneda oMoneda);

        Task<EntityErrors> EliminarMonedaAsync(string MonedaId);
    }
}


