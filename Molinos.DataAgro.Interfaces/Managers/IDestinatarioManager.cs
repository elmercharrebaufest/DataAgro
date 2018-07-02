using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDestinatarioManager
    {        
        Task<ResultIniDestinatario> TraerTodoDestinatarioAsync();

        Task<Destinatario> TraerDestinatarioAsync(int intDestinatarioId);

        Task<EntityErrors> GrabarDestinatarioAsync(Destinatario oDestinatario);

        Task<EntityErrors> EliminarDestinatarioAsync(int intDestinatarioId);

    }
}


