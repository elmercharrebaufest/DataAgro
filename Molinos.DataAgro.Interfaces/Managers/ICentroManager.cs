using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICentroManager
    {
        DatosIniAbmCentro TraerDatosIniciales();
        ResultIniCentro TraerTodoCentro();
        Centro TraerCentro(int id);
        Resultado GrabarCentro(Centro oCentro);
        Resultado EliminarCentro(int id);
    }
}