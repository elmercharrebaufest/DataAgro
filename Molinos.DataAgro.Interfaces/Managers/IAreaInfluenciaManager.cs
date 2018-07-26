using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAreaInfluenciaManager
    {        
        Task<ResultIniAreaInfluencia> TraerTodoAreaInfluenciaAsync();

        Task<AreaInfluencia> TraerAreaInfluenciaAsync(int intAreaInfluenciaId);

        Task<EntityErrors> GrabarAreaInfluenciaAsync(AreaInfluencia oAreaInfluencia);

        Task<EntityErrors> EliminarAreaInfluenciaAsync(int intAreaInfluenciaId);
    }
}


