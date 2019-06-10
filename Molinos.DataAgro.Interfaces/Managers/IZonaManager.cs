using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IZonaManager
    {
        DatosIniAbmZona TraerDatosIniciales();
        ResultIniZona TraerTodoZona();
        ZonaDto TraerZona(int id);
        Resultado GrabarZona(Zona oZona);
        Resultado EliminarZona(int id);
    }
}