using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICampañaActualManager
    {
        Task<EntityErrors> ActualizacionCampañaActualAsync(CampañaActual oParam);
    }
}
