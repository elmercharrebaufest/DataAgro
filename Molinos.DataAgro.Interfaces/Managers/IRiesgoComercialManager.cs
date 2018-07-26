using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IRiesgoComercialManager
    {        
        Task<EntityErrors> ActualizacionDeRiesgoComercialAsync(RiesgoComercial oParam);
    }
}
