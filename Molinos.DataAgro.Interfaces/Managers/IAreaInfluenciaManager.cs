using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAreaInfluenciaManager
    {        
        ResultIniAreaInfluencia TraerTodoAreaInfluencia();

        AreaInfluencia TraerAreaInfluencia(int intAreaInfluenciaId);

        Resultado GrabarAreaInfluencia(AreaInfluencia oAreaInfluencia);

        Resultado EliminarAreaInfluencia(int intAreaInfluenciaId);
    }
}


