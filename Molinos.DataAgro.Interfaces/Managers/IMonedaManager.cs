using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IMonedaManager
    {
        ResultIniMoneda TraerTodo();

        Moneda TraerMonedad(string MonedaId);

        Resultado GrabarMoneda(Moneda oMoneda);

        Resultado EliminarMoneda(string MonedaId);
    }
}


