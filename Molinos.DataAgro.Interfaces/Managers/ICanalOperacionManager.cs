using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICanalOperacionManager
    {
        ResultIniCanalOperacion TraerTodoCanalOperacion();

        CanalOperacionDto TraerCanalOperacion(int intCanalOperacionId);

        Resultado GrabarCanalOperacion(CanalOperacion oCanalOperacion);

        Resultado EliminarCanalOperacion(int intCanalOperacionId);
    }
}


