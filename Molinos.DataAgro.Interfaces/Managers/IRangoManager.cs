using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IRangoManager
    {
        DatosIniAbmRango TraerDatosIniciales();
        ResultIniRango TraerTodoRango();
        RangoPrecioDto TraerRango(int id);
        Resultado GrabarRango(RangoPrecio oRango);
        Resultado EliminarRango(int id);
    }
}