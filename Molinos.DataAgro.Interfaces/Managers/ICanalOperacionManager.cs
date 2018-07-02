using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

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


