using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface IRangoConfirmacionAutomaticaManager
    {
        DatosIniAbmRangoConfirmacionAutomatica TraerDatosIniciales();
        ResultIniRangoConfirmacionAutomatica TraerTodoRango();
        RangoConfirmacionAutomaticaDto TraerRango(int id);
        Resultado GrabarRangoConfirmacionAutomatica(RangoConfirmacionAutomatica oRango);
        Resultado EliminarRangoConfirmacionAutomatica(int id);
    }
}