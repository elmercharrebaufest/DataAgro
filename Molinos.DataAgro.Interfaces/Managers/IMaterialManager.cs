using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMaterialManager
    {
        Task<ResultIniMaterial> TraerFiltroMaterialAsync(ParamAbmMaterial oParam);

        Task<Material> TraerMaterialAsync(int intMaterialId);

        Task<EntityErrors> GrabarMaterialAsync(Material oMaterial);

        Task<EntityErrors> EliminarMaterialAsync(int intMaterialId);
    }
}


