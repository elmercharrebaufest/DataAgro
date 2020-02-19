using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMaterialManager
    {
        DatosIniAbmMaterial TraerDatosIniciales();

        ResultIniMaterial TraerFiltroMaterial(ParamAbmMaterial oParam);

        MaterialDto TraerMaterial(int intMaterialId);

        ResultIniMaterial TraerTodoMaterial();

        Resultado GrabarMaterial(Material oMaterial);

        Resultado EliminarMaterial(int intMaterialId);
    }
}


