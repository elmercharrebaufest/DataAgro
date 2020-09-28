using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICentroManager
    {
        DatosIniAbmCentro TraerDatosIniciales();
        ResultIniCentro TraerTodoCentro();
        CentroDto TraerCentro(int id);
        Resultado GrabarCentro(Centro oCentro);
        Resultado EliminarCentro(int id);

        CentroDto ObtenerCentroPorCodigoSap(string codigoSap);
    }
}