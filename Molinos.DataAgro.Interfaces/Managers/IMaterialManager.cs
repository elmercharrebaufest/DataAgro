using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

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


