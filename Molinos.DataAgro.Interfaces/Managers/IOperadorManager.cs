using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IOperadorManager
    {
        DatosIniAbmOperador TraerDatosIniciales();
        ResultIniOperador TraerTodoOperador();
        OperadorDto TraerOperador(int id);
        Resultado GrabarOperador(Operador oCentro);
        Resultado EliminarOperador(int id);
    }
}