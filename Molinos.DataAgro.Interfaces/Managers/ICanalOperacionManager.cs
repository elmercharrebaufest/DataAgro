using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICanalOperacionManager
    {        
        Task<ResultIniCanalOperacion> TraerTodoCanalOperacionAsync();

        Task<CanalOperacion> TraerCanalOperacionAsync(int intCanalOperacionId);

        Task<EntityErrors> GrabarCanalOperacionAsync(CanalOperacion oCanalOperacion);

        Task<EntityErrors> EliminarCanalOperacionAsync(int intCanalOperacionId);
    }
}


