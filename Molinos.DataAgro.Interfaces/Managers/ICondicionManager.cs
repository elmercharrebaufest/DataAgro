using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICondicionManager
    {
        ResultIniCondicion TraerTodoCondicion();

        CondicionDto TraerCondicion(int intCondicionId);

        Resultado GrabarCondicion(Condicion oCondicion);

        Resultado EliminarCondicion(int intCondicionId);
    }
}


