using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMaterialManager
    {
        ResultIniMaterial TraerFiltroMaterial(ParamAbmMaterial oParam);

        Material TraerMaterial(int intMaterialId);

        Resultado GrabarMaterial(Material oMaterial);

        Resultado EliminarMaterial(int intMaterialId);
    }
}


