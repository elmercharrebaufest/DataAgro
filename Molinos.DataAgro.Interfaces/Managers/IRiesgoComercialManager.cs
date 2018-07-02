using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IRiesgoComercialManager
    {        
        Task<EntityErrors> ActualizacionDeRiesgoComercialAsync(RiesgoComercial oParam);
    }
}
